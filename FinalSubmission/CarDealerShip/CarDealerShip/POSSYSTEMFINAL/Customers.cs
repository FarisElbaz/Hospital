using POSSYSTEMFINAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Vehicles
{
    public partial class Customers : Form
    {
        // Define a SqlConnection object
        private string connectionString;

        public Customers(string connectionString)
        {
            InitializeComponent();
            Load += Form5_Load;
            this.connectionString = connectionString;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Open the connection
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Customer", connection);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        // Validate phone number format
        private bool ValidatePhoneNumber(string phoneNumber)
        {
            return phoneNumber.Length == 11 && phoneNumber.All(char.IsDigit);
        }

        // Validate email format
        private bool ValidateEmail(string email)
        {
            return email.Contains("@");
        }

        private void ResetFields()
        {
            textBoxCustomerID.Text = "";
            textBoxName.Text = "";
            textBoxPhoneNumber.Text = "";
            textBoxEmail.Text = "";
            textBoxAge.Text = "";
        }

        private void Customers_Load(object sender, EventArgs e)
        {

        }

        private void Addbtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Open the connection

                    // Check if phone number has 11 digits
                    if (textBoxPhoneNumber.Text.Length != 11 || !textBoxPhoneNumber.Text.All(char.IsDigit))
                    {
                        MessageBox.Show("Phone number must be 11 digits long.");
                        return;
                    }

                    // Check if email contains '@'
                    if (!textBoxEmail.Text.Contains("@"))
                    {
                        MessageBox.Show("Invalid email format. Must contain '@' symbol.");
                        return;
                    }

                    string insertQuery = "INSERT INTO customer (customer_id, name, phone_number, email, age) " +
                                         "VALUES (@CustomerID, @Name, @PhoneNumber, @Email, @Age)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@CustomerID", textBoxCustomerID.Text);
                        insertCmd.Parameters.AddWithValue("@Name", textBoxName.Text);
                        insertCmd.Parameters.AddWithValue("@PhoneNumber", textBoxPhoneNumber.Text);
                        insertCmd.Parameters.AddWithValue("@Email", textBoxEmail.Text);
                        insertCmd.Parameters.AddWithValue("@Age", textBoxAge.Text);

                        int rowsAffected = insertCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Customer details added successfully");
                            LoadData();
                            ResetFields();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add customer details");
                        }
                    }
                } // Connection will automatically close at the end of using block
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding customer details: " + ex.Message);
            }
        }

        private void Resetbtn_Click(object sender, EventArgs e)
        {
            ResetFields();
        }

        private void Editbtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
            {
                // Populate the dropdown menu with customer IDs
                List<string> customerIDs = new List<string>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["customer_id"].Value != null)
                    {
                        customerIDs.Add(row.Cells["customer_id"].Value.ToString());
                    }
                }
                if (customerIDs.Count > 0)
                {
                    // Create and display a form for ID, column, new value input
                    Form prompt = new Form();
                    prompt.Width = 300;
                    prompt.Height = 250;
                    prompt.Text = "Edit Customer Details";
                    Label labelID = new Label() { Text = "Select Customer ID:", Left = 10, Top = 20, Width = 100 };
                    ComboBox comboBoxCustomerID = new ComboBox();
                    comboBoxCustomerID.DataSource = customerIDs;
                    comboBoxCustomerID.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBoxCustomerID.Location = new System.Drawing.Point(120, 20);
                    Label labelColumn = new Label() { Text = "Select Column:", Left = 10, Top = 50, Width = 100 };
                    ComboBox comboBoxColumn = new ComboBox();
                    comboBoxColumn.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBoxColumn.Location = new System.Drawing.Point(120, 50);
                    Label labelNewValue = new Label() { Text = "New Value:", Left = 10, Top = 80, Width = 100 };
                    TextBox textBoxNewValue = new TextBox(); // Default to TextBox for other columns
                    textBoxNewValue.Location = new System.Drawing.Point(120, 80);
                    Button confirmation = new Button() { Text = "Ok", Left = 100, Width = 100, Top = 150 };
                    confirmation.Click += (sender2, e2) =>
                    {
                        string selectedCustomerID = comboBoxCustomerID.SelectedItem.ToString();
                        string selectedColumn = comboBoxColumn.SelectedItem.ToString();
                        string newValue = textBoxNewValue.Text;

                        // Validate phone number format if the selected column is 'phone_number'
                        if (selectedColumn == "phone_number")
                        {
                            if (!ValidatePhoneNumber(newValue))
                            {
                                MessageBox.Show("Phone number must be 11 digits long.");
                                return;
                            }
                        }

                        // Validate email format if the selected column is 'email'
                        if (selectedColumn == "email")
                        {
                            if (!ValidateEmail(newValue))
                            {
                                MessageBox.Show("Invalid email format. Must contain '@' symbol.");
                                return;
                            }
                        }

                        // Update DataGridView and database with the new value
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open(); // Open the connection
                                string query = $"UPDATE customer SET {selectedColumn} = @Value WHERE customer_id = @ID";
                                SqlCommand cmd = new SqlCommand(query, connection);
                                cmd.Parameters.AddWithValue("@Value", newValue);
                                cmd.Parameters.AddWithValue("@ID", selectedCustomerID);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Customer details updated successfully");
                                    LoadData(); // Refresh the data in dataGridView1 after editing
                                }
                                else
                                {
                                    MessageBox.Show("Failed to update customer details");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error updating customer details: " + ex.Message);
                        }
                        finally
                        {
                            prompt.Close();
                        }
                    };

                    // Add columns as dropdown items
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        comboBoxColumn.Items.Add(column.HeaderText);
                    }

                    // Add controls to the form
                    prompt.Controls.Add(labelID);
                    prompt.Controls.Add(comboBoxCustomerID);
                    prompt.Controls.Add(labelColumn);
                    prompt.Controls.Add(comboBoxColumn);
                    prompt.Controls.Add(labelNewValue);
                    prompt.Controls.Add(textBoxNewValue);
                    prompt.Controls.Add(confirmation);
                    prompt.ShowDialog();
                }
                else
                {
                    MessageBox.Show("There are no customers to edit.");
                }
            }
            else
            {
                MessageBox.Show("There are no customers to edit.");
            }
        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
            {
                // Populate the dropdown menu with customer IDs
                List<string> customerIDs = new List<string>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["customer_id"].Value != null)
                    {
                        customerIDs.Add(row.Cells["customer_id"].Value.ToString());
                    }
                }
                if (customerIDs.Count > 0)
                {
                    // Create and display a form for ID selection
                    Form prompt = new Form();
                    prompt.Width = 300;
                    prompt.Height = 150;
                    prompt.Text = "Delete Customer";
                    Label labelID = new Label() { Text = "Select Customer ID:", Left = 10, Top = 20, Width = 100 };
                    ComboBox comboBoxCustomerID = new ComboBox();
                    comboBoxCustomerID.DataSource = customerIDs;
                    comboBoxCustomerID.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBoxCustomerID.Location = new System.Drawing.Point(120, 20);
                    Button confirmation = new Button() { Text = "Ok", Left = 100, Width = 100, Top = 80 };
                    confirmation.Click += (sender2, e2) =>
                    {
                        string selectedCustomerID = comboBoxCustomerID.SelectedItem.ToString();

                        // Delete the customer from the database
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open(); // Open the connection
                                string query = $"DELETE FROM customer WHERE customer_id = @ID";
                                SqlCommand cmd = new SqlCommand(query, connection);
                                cmd.Parameters.AddWithValue("@ID", selectedCustomerID);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Customer deleted successfully");
                                    LoadData(); // Refresh the data in dataGridView1 after deleting
                                }
                                else
                                {
                                    MessageBox.Show("Customer with specified ID not found");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting customer: " + ex.Message);
                        }

                        prompt.Close();
                    };

                    // Add controls to the form
                    prompt.Controls.Add(labelID);
                    prompt.Controls.Add(comboBoxCustomerID);
                    prompt.Controls.Add(confirmation);
                    prompt.ShowDialog();
                }
                else
                {
                    MessageBox.Show("There are no customers to delete.");
                }
            }
            else
            {
                MessageBox.Show("There are no customers to delete.");
            }
        }

        private void Returnbtn_Click(object sender, EventArgs e)
        {
            this.Close();
            ADMIN form1 = new ADMIN(connectionString);
            form1.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
