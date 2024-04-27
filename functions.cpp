#include "functions.h"

Functions::Functions() : henv(nullptr), hdbc(nullptr) {}

Functions::~Functions() {
    connClose();
}

bool Functions::connOpen() {
    // Allocate the environment handle
    SQLAllocHandle(SQL_HANDLE_ENV, SQL_NULL_HANDLE, &henv);
    SQLSetEnvAttr(henv, SQL_ATTR_ODBC_VERSION, (SQLPOINTER)SQL_OV_ODBC3, 0);

    // Allocate the connection handle
    SQLAllocHandle(SQL_HANDLE_DBC, henv, &hdbc);

    // Connection string
    // Replace the connection string with your actual connection details
    SQLCHAR* connectionString = (SQLCHAR*)"DRIVER={ODBC Driver 18 for SQL Server};SERVER=localhost;DATABASE=hospital;UID=sa;PWD=reallyStrongPwd123;Encrypt=no";

    // Connect to the SQL Server
    SQLRETURN ret = SQLDriverConnect(hdbc, NULL, connectionString, SQL_NTS, NULL, 0, NULL, SQL_DRIVER_COMPLETE);

    return ret == SQL_SUCCESS || ret == SQL_SUCCESS_WITH_INFO;
}

void Functions::connClose() {
    if (hdbc!= nullptr) {
        SQLDisconnect(hdbc);
        SQLFreeHandle(SQL_HANDLE_DBC, hdbc);
    }
    if (henv!= nullptr) {
        SQLFreeHandle(SQL_HANDLE_ENV, henv);
    }
}

QMap<QString, QString> Functions::getTableNames() {
    QMap<QString, QString> tableNames;
    SQLHSTMT hstmt;
    SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt);

    // SQLTablesA query to retrieve only user-created tables
    SQLCHAR query[] = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

    if (SQLExecDirectA(hstmt, query, SQL_NTS) != SQL_SUCCESS) {
        SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
        return tableNames;
    }

    SQLCHAR tableName[256];
    SQLLEN len;

    while (SQLFetch(hstmt) == SQL_SUCCESS) {
        SQLGetData(hstmt, 1, SQL_C_CHAR, tableName, sizeof(tableName), &len);
        QString name = QString::fromUtf8(reinterpret_cast<const char*>(tableName));
        qDebug() << "Table Name: " << name;
        tableNames[name] = "";
        // You can add a description here if needed
    }

    SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
    return tableNames;
}

bool Functions::addRecord(const QString& tableName, const QMap<QString, QVariant>& values) {
    SQLHSTMT hstmt;
    SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt);

    QString sql = "INSERT INTO " + tableName + " (";
    QStringList columns = values.keys();
    QStringList placeholders;
    for (const QString& col : columns) {
        placeholders << "?";
    }
    sql += columns.join(", ") + ") VALUES (" + placeholders.join(", ") + ")";

    SQLPrepare(hstmt, (SQLCHAR*)sql.toUtf8().data(), SQL_NTS);

    int index = 1;
    for (const QString& col : columns) {
        QVariant value = values.value(col);
        SQLBindParameter(hstmt, index++, SQL_PARAM_INPUT, SQL_C_CHAR, SQL_VARCHAR, 0, 0, (SQLPOINTER)value.toString().toUtf8().data(), 0, nullptr);
    }

    SQLRETURN ret = SQLExecute(hstmt);
    SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
    return (ret == SQL_SUCCESS || ret == SQL_SUCCESS_WITH_INFO);
}

bool Functions::removeRecord(const QString& tableName, const QString& condition) {
    SQLHSTMT hstmt;
    SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt);

    QString sql = "DELETE FROM " + tableName + " WHERE " + condition;
    SQLRETURN ret = SQLExecDirect(hstmt, (SQLCHAR*)sql.toUtf8().data(), SQL_NTS);
    SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
    return (ret == SQL_SUCCESS || ret == SQL_SUCCESS_WITH_INFO);
}

bool Functions::updateRecord(const QString& tableName, const QMap<QString, QVariant>& updates, const QString& condition) {
    SQLHSTMT hstmt;
    SQLAllocHandle(SQL_HANDLE_STMT, hdbc, &hstmt);

    QString sql = "UPDATE " + tableName + " SET ";
    QStringList setClauses;
    for (auto it = updates.begin(); it != updates.end(); ++it) {
        setClauses << it.key() + " = ?";
    }
    sql += setClauses.join(", ") + " WHERE " + condition;

    SQLPrepare(hstmt, (SQLCHAR*)sql.toUtf8().data(), SQL_NTS);

    int index = 1;
    for (auto it = updates.begin(); it != updates.end(); ++it) {
        QVariant value = it.value();
        SQLBindParameter(hstmt, index++, SQL_PARAM_INPUT, SQL_C_CHAR, SQL_VARCHAR, 0, 0, (SQLPOINTER)value.toString().toUtf8().data(), 0, nullptr);
    }

    SQLRETURN ret = SQLExecute(hstmt);
    SQLFreeHandle(SQL_HANDLE_STMT, hstmt);
    return (ret == SQL_SUCCESS || ret == SQL_SUCCESS_WITH_INFO);
}
