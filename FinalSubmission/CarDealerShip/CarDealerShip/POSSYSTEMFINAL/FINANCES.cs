using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace POSSYSTEMFINAL
{
    public partial class FINANCES : Form
    {
        private string connectionString;

        public FINANCES(string connectionString)
        {
            this.connectionString = connectionString;
            InitializeComponent();
        }

        private void FINANCES_Load(object sender, EventArgs e)
        {
            // Load finance data into DataGridViews
            LoadFinanceData();
        }

        private void LoadFinanceData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Query to get finance data
                    string financeQuery = "SELECT * FROM Finance";

                    // Execute the query
                    SqlDataAdapter adapter = new SqlDataAdapter(financeQuery, connection);
                    DataTable financeTable = new DataTable();
                    adapter.Fill(financeTable);

                    // Bind data to DataGridView
                    dataGridView1.DataSource = financeTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

