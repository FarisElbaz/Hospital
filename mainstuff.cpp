#include "mainstuff.h"
#include "ui_mainstuff.h"

mainstuff::mainstuff(Functions *db, QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::mainstuff),
    model(new QStandardItemModel(this)),
    db(db)
{
    ui->setupUi(this);

    connect(ui->LoadButton, &QPushButton::clicked, this, [=]() {
        on_LoadButton_clicked();
    });

    qDebug() << "UI setup complete";

    listView = findChild<QListView*>("listView");
    if(listView) {
        connect(listView, &QListView::clicked, this, &mainstuff::onItemClicked);
        qDebug() << "ListView found and connected";
    } else {
        qDebug() << "ListView not found in the UI!";
    }

    if (!db->connOpen()) {
        QMessageBox::critical(this, "Error", "Failed to connect to the Function");
        return;
    }
    qDebug() << "Database connection opened successfully";

    QMap<QString, QString> tableNamesMap = db->getTableNames();
    qDebug() << "Table Names Map: " << tableNamesMap;

    QStringList tableNames = tableNamesMap.keys();
    qDebug() << "Table Names List: " << tableNames;

    foreach (const QString &tableName, tableNames) {
        model->appendRow(new QStandardItem(tableName));
    }
    qDebug() << "Items added to model";

    listView->setModel(model);

    for (int row = 0; row < model->rowCount(); ++row) {
        QModelIndex index = model->index(row, 0);
        qDebug() << "Item:" << model->data(index).toString();
    }
}

mainstuff::~mainstuff()
{
    db->connClose();
    delete ui;
}


void mainstuff::onItemClicked(const QModelIndex &index) {
    selectedTableName = model->data(index).toString();
    qDebug() << selectedTableName;
}


void mainstuff::on_LoadButton_clicked() {
    SQLHDBC hdbc = db->getConnectionHandle();
    if (selectedTableName.isEmpty()) {
        QMessageBox::warning(this, "Warning", "Please select a table from the list first.");
        return;
    }
    qDebug() << selectedTableName;

    // Now load the table using the selected table name
    SQLCHAR query[512];
    snprintf((char*)query, sizeof(query), "SELECT * FROM %s", selectedTableName.toUtf8().constData());
    qDebug() << "Query:" << query;

    SQLHSTMT hstmt;
    SQLRETURN ret = SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt); // Allocate statement handle using the provided hdbc
    // if (ret != SQL_SUCCESS && ret != SQL_SUCCESS_WITH_INFO) {
    //     QMessageBox::critical(this, "Error", "Failed to allocate statement handle");
    //     return;
    // }
    if (ret == SQL_INVALID_HANDLE) {
        // Handle invalid handle error
        qDebug() << "Invalid handle error: hdbc is not valid.";
        // Additional error handling logic...
    } else if (ret == SQL_ERROR) {
        // Handle generic error
        qDebug() << "Generic error occurred.";
        // Use SQLGetDiagRec() to retrieve detailed error information
        // Additional error handling logic...
    } else {
        // Allocation successful
        qDebug() << "Statement handle allocated successfully.";
        // Continue with your code...
    }

    ret = SQLExecDirect(hstmt, query, SQL_NTS);
    if (ret != SQL_SUCCESS && ret != SQL_SUCCESS_WITH_INFO) {
        SQLSMALLINT rec = 0;
        SQLCHAR state[7];
        SQLINTEGER native;
        SQLCHAR msg[1024];
        while (SQLGetDiagRec(SQL_HANDLE_STMT, hstmt, ++rec, state, &native, msg, sizeof(msg), NULL) == SQL_SUCCESS) {
            QMessageBox::critical(this, "Error", QString::fromLocal8Bit(reinterpret_cast<char*>(state)) + ": " + QString::fromLocal8Bit(reinterpret_cast<char*>(msg)));
        }
        SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
        return;
    }

    // Search for the tableWidget widget
    QTableWidget *tableWidget = findChild<QTableWidget*>("tableWidget");
    if (!tableWidget) {
        QMessageBox::critical(this, "Error", "TableWidget widget not found.");
        SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
        return;
    }

    tableWidget->clearContents();
    tableWidget->setRowCount(0);

    SQLSMALLINT columnCount;
    ret = SQLNumResultCols(hstmt, &columnCount);
    if (ret != SQL_SUCCESS && ret != SQL_SUCCESS_WITH_INFO) {
        QMessageBox::critical(this, "Error", "Failed to get number of columns");
        SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
        return;
    }

    // Set the column headers
    tableWidget->setColumnCount(columnCount);

    // Retrieve the column names
    for (int i = 0; i < columnCount; i++) {
        SQLCHAR columnName[256];
        SQLSMALLINT nameLength;
        ret = SQLDescribeCol(hstmt, i + 1, columnName, sizeof(columnName), &nameLength, NULL, NULL, NULL, NULL);
        if (ret != SQL_SUCCESS && ret != SQL_SUCCESS_WITH_INFO) {
            QMessageBox::critical(this, "Error", "Failed to get column name");
            SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
            return;
        }
        QTableWidgetItem* headerItem = new QTableWidgetItem(QString::fromLocal8Bit(reinterpret_cast<char*>(columnName)).left(nameLength));
        tableWidget->setHorizontalHeaderItem(i, headerItem);
    }

    // Get the data rows
    while (SQLFetch(hstmt) == SQL_SUCCESS) {
        QList<QTableWidgetItem*> rowItems;
        for (int i = 0; i < columnCount; i++) {
            SQLCHAR data[256];
            SQLLEN dataLength;
            ret = SQLGetData(hstmt, i + 1, SQL_C_CHAR, data, sizeof(data), &dataLength);
            if (ret != SQL_SUCCESS && ret != SQL_SUCCESS_WITH_INFO) {
                QMessageBox::critical(this, "Error", "Failed to get row data");
                SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
                return;
            }
            QTableWidgetItem* item = new QTableWidgetItem(QString::fromLocal8Bit(reinterpret_cast<char*>(data)));
            rowItems << item;
        }
        tableWidget->insertRow(tableWidget->rowCount());
        for (int i = 0; i < columnCount; i++) {
            tableWidget->setItem(tableWidget->rowCount() - 1, i, rowItems[i]);
        }
    }

    // Clean up
    SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
}



void mainstuff::onDSclicked() {
    // Implementation for handling the DS button click
}

void mainstuff::onPSclicked() {
    // Implementation for handling the PS button click
}

void mainstuff::onAddclicked()
{
    // Implementation for handling the add button click
}

void mainstuff::onDeleteclicked() {
    // Implementation for handling the delete button click
}

void mainstuff::onUpdateclicked() {
    // Implementation for handling the update button click
}


