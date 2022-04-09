using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace POSSYSTEMFINAL
{
    public partial class VehicleManager : Form
    {
        private string connectionString;

        public VehicleManager(string connectionString)
        {
            this.connectionString = connectionString;
            InitializeComponent();
        }

        private void VehicleManager_Load(object sender, EventArgs e)
        {
            // Load data into DataGridView
            LoadVehicleData();
        }

        private void LoadVehicleData()
        {
            string query = "SELECT * FROM Vehicle";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Select the clicked row
                dataGridView1.Rows[e.RowIndex].Selected = true;

                // Enable/disable the remove button based on whether a row is selected
                button2.Enabled = dataGridView1.SelectedRows.Count > 0;
            }
        }







        private void button1_Click(object sender, EventArgs e)
        {
            // Create a SQL query to insert a new record into the "Vehicle" table
            string query = "INSERT INTO Vehicle (V_ID, Type, Brand, Model, Mileage_, Color, Year, Price) VALUES (@V_ID, @Type, @Brand, @Model, @Mileage_, @Color, @Year, @Price)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the database connection
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Set parameter values from textboxes
                    command.Parameters.AddWithValue("@V_ID", textBox1.Text);
                    command.Parameters.AddWithValue("@Type", textBox6.Text);
                    command.Parameters.AddWithValue("@Brand", textBox5.Text);
                    command.Parameters.AddWithValue("@Model", textBox4.Text);
                    command.Parameters.AddWithValue("@Mileage_", textBox3.Text);
                    command.Parameters.AddWithValue("@Color", textBox2.Text);
                    command.Parameters.AddWithValue("@Year", textBox7.Text);
                    command.Parameters.AddWithValue("@Price", textBox8.Text);

                    // Execute the query
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("New record added successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Failed to add new record!");
                    }
                }
            }
        }






        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Check if any row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the V_ID of the selected row
                string vId = dataGridView1.SelectedRows[0].Cells["V_ID"].Value.ToString();

                // Prompt the user for confirmation
                DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // If the user confirms, delete the record
                if (result == DialogResult.Yes)
                {
                    DeleteRecord(vId);
                }
            }
            else
            {
                MessageBox.Show("Please select a record to delete.");
            }
        }

        private void DeleteRecord(string vId)
        {
            string query = "DELETE FROM Vehicle WHERE V_ID = @V_ID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@V_ID", vId);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Record deleted successfully!");
                        LoadVehicleData(); // Refresh DataGridView
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete record!");
                    }
                }
            
            }
        }

    }
}
