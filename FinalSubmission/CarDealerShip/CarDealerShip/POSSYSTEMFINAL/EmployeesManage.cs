using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace POSSYSTEMFINAL
{
    public partial class EmployeesManage : Form
    {
        private string connectionString;

        public EmployeesManage(string connectionString)
        {
            this.connectionString = connectionString;
            InitializeComponent();
            Load += Form4_Load;
        }

        public EmployeesManage()
        {
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM employees", connection);
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

        private void buttonBack_click_Click(object sender, EventArgs e)
        {
            this.Close();
            ADMIN form1 = new ADMIN(connectionString);
            form1.Show();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
            {
                // Populate the dropdown menu with employee IDs
                List<string> employeeIDs = new List<string>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["employee_ID"].Value != null)
                    {
                        employeeIDs.Add(row.Cells["employee_ID"].Value.ToString());
                    }
                }
                if (employeeIDs.Count > 0)
                {
                    // Create and display a form for ID selection
                    Form prompt = new Form();
                    prompt.Width = 300;
                    prompt.Height = 150;
                    prompt.Text = "Delete Employee";
                    Label labelID = new Label() { Text = "Select Employee ID:", Left = 10, Top = 20, Width = 100 };
                    ComboBox comboBoxEmployeeID = new ComboBox();
                    comboBoxEmployeeID.DataSource = employeeIDs;
                    comboBoxEmployeeID.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBoxEmployeeID.Location = new System.Drawing.Point(120, 20);
                    Button confirmation = new Button() { Text = "Ok", Left = 100, Width = 100, Top = 80 };
                    confirmation.Click += (sender2, e2) =>
                    {
                        string selectedEmployeeID = comboBoxEmployeeID.SelectedItem.ToString();

                        // Delete the employee from the database
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(this.connectionString))
                            {
                                connection.Open();
                                string query = $"DELETE FROM Employee WHERE employee_ID = @ID";
                                SqlCommand cmd = new SqlCommand(query, connection);
                                cmd.Parameters.AddWithValue("@ID", selectedEmployeeID);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Employee deleted successfully");
                                    LoadData(); // Refresh the data in dataGridView1 after deleting
                                }
                                else
                                {
                                    MessageBox.Show("Employee with specified ID not found");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting employee: " + ex.Message);
                        }

                        prompt.Close();
                    };

                    // Add controls to the form
                    prompt.Controls.Add(labelID);
                    prompt.Controls.Add(comboBoxEmployeeID);
                    prompt.Controls.Add(confirmation);
                    prompt.ShowDialog();
                }
                else
                {
                    MessageBox.Show("There are no employees to delete.");
                }
            }
            else
            {
                MessageBox.Show("There are no employees to delete.");
            }
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
            {
                // Populate the dropdown menu with employee IDs
                List<string> employeeIDs = new List<string>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["employee_ID"].Value != null)
                    {
                        employeeIDs.Add(row.Cells["employee_ID"].Value.ToString());
                    }
                }
                if (employeeIDs.Count > 0)
                {
                    // Create and display a form for ID, column, new value input
                    Form prompt = new Form();
                    prompt.Width = 300;
                    prompt.Height = 250;
                    prompt.Text = "Edit Employee Details";
                    Label labelID = new Label() { Text = "Select Employee ID:", Left = 10, Top = 20, Width = 100 };
                    ComboBox comboBoxEmployeeID = new ComboBox();
                    comboBoxEmployeeID.DataSource = employeeIDs;
                    comboBoxEmployeeID.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBoxEmployeeID.Location = new System.Drawing.Point(120, 20);
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
                        string selectedEmployeeID = comboBoxEmployeeID.SelectedItem.ToString();
                        string selectedColumn = comboBoxColumn.SelectedItem.ToString();
                        string newValue = textBoxNewValue.Text;

                        // Validate phone number format
                        if (selectedColumn == "phone_number" && (newValue.Length != 11 || !newValue.All(char.IsDigit)))
                        {
                            MessageBox.Show("Phone number must be 11 digits long.");
                            return;
                        }

                        // Validate email format
                        if (selectedColumn == "email" && !newValue.Contains("@"))
                        {
                            MessageBox.Show("Invalid email format. Must contain '@' symbol.");
                            return;
                        }

                        // Update DataGridView and database with the new value
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(this.connectionString))
                            {
                                connection.Open();
                                string query = $"UPDATE Employee SET {selectedColumn} = @Value WHERE employee_ID = @ID";
                                SqlCommand cmd = new SqlCommand(query, connection);
                                cmd.Parameters.AddWithValue("@Value", newValue);
                                cmd.Parameters.AddWithValue("@ID", selectedEmployeeID);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Employee details updated successfully");
                                    LoadData(); // Refresh the data in dataGridView1 after editing
                                }
                                else
                                {
                                    MessageBox.Show("Failed to update employee details");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error updating employee details: " + ex.Message);
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
                    prompt.Controls.Add(comboBoxEmployeeID);
                    prompt.Controls.Add(labelColumn);
                    prompt.Controls.Add(comboBoxColumn);
                    prompt.Controls.Add(labelNewValue);
                    prompt.Controls.Add(textBoxNewValue);
                    prompt.Controls.Add(confirmation);
                    prompt.ShowDialog();
                }
                else
                {
                    MessageBox.Show("There are no employees to edit.");
                }
            }
            else
            {
                MessageBox.Show("There are no employees to edit.");
            }
        }

        private void AddDisplay_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if any of the required fields are empty
                if (string.IsNullOrWhiteSpace(textBoxEmployeeID.Text) ||
                    string.IsNullOrWhiteSpace(textBoxName.Text) ||
                    string.IsNullOrWhiteSpace(textBoxPhoneNumber.Text) ||
                    string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                    string.IsNullOrWhiteSpace(textBoxPosition.Text) ||
                    string.IsNullOrWhiteSpace(textBoxYearOfJoining.Text))
                {
                    MessageBox.Show("Please fill out all fields.");
                    return;
                }

                // Check if phone number has maximum of 11 digits
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

                // Open the connection
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO Employee (employee_ID, name, phone_number, email, position, year_of_joining) " +
                                         "VALUES (@EmployeeID, @Name, @PhoneNumber, @Email, @Position, @YearOfJoining)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@EmployeeID", textBoxEmployeeID.Text);
                        insertCmd.Parameters.AddWithValue("@Name", textBoxName.Text);
                        insertCmd.Parameters.AddWithValue("@PhoneNumber", textBoxPhoneNumber.Text);
                        insertCmd.Parameters.AddWithValue("@Email", textBoxEmail.Text);
                        insertCmd.Parameters.AddWithValue("@Position", textBoxPosition.Text);
                        insertCmd.Parameters.AddWithValue("@YearOfJoining", textBoxYearOfJoining.Text);

                        int rowsAffected = insertCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Employee details added successfully");
                            LoadData();
                            ResetFields();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add employee details");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding employee details: " + ex.Message);
            }
        }

        private void ResetFields()
        {
            textBoxEmployeeID.Text = "";
            textBoxName.Text = "";
            textBoxPhoneNumber.Text = "";
            textBoxEmail.Text = "";
            textBoxPosition.Text = "";
            textBoxYearOfJoining.Text = "";
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            ResetFields();
        }

        private void textBoxPosition_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxEmployeeID_TextChanged(object sender, EventArgs e)
        {

        }

        private void Returnbtn_Click(object sender, EventArgs e)
        {
            this.Close();
            ADMIN form1 = new ADMIN(connectionString);
            form1.Show();
        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
            {
                // Populate the dropdown menu with employee IDs
                List<string> employeeIDs = new List<string>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["employee_ID"].Value != null)
                    {
                        employeeIDs.Add(row.Cells["employee_ID"].Value.ToString());
                    }
                }
                if (employeeIDs.Count > 0)
                {
                    // Create and display a form for ID selection
                    Form prompt = new Form();
                    prompt.Width = 300;
                    prompt.Height = 150;
                    prompt.Text = "Delete Employee";
                    Label labelID = new Label() { Text = "Select Employee ID:", Left = 10, Top = 20, Width = 100 };
                    ComboBox comboBoxEmployeeID = new ComboBox();
                    comboBoxEmployeeID.DataSource = employeeIDs;
                    comboBoxEmployeeID.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBoxEmployeeID.Location = new System.Drawing.Point(120, 20);
                    Button confirmation = new Button() { Text = "Ok", Left = 100, Width = 100, Top = 80 };
                    confirmation.Click += (sender2, e2) =>
                    {
                        string selectedEmployeeID = comboBoxEmployeeID.SelectedItem.ToString();

                        // Delete the employee from the database
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(this.connectionString))
                            {
                                connection.Open();
                                string query = $"DELETE FROM Employee WHERE employee_ID = @ID";
                                SqlCommand cmd = new SqlCommand(query, connection);
                                cmd.Parameters.AddWithValue("@ID", selectedEmployeeID);
                                int rowsAffected = cmd.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Employee deleted successfully");
                                    LoadData(); // Refresh the data in dataGridView1 after deleting
                                }
                                else
                                {
                                    MessageBox.Show("Employee with specified ID not found");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting employee: " + ex.Message);
                        }

                        prompt.Close();
                    };

                    // Add controls to the form
                    prompt.Controls.Add(labelID);
                    prompt.Controls.Add(comboBoxEmployeeID);
                    prompt.Controls.Add(confirmation);
                    prompt.ShowDialog();
                }
                else
                {
                    MessageBox.Show("There are no employees to delete.");
                }
            }
            else
            {
                MessageBox.Show("There are no employees to delete.");
            }
        }

        private void EmployeesManage_Load(object sender, EventArgs e)
        {

        }

        private void Editbtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1 != null && dataGridView1.Rows.Count > 0)
            {
                // Populate the dropdown menu with employee IDs
                List<string> employeeIDs = new List<string>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["employee_ID"].Value != null)
                    {
                        employeeIDs.Add(row.Cells["employee_ID"].Value.ToString());
                    }
                }
                if (employeeIDs.Count > 0)
                {
                    // Create and display a form for ID, column, new value input
                    Form prompt = new Form();
                    prompt.Width = 300;
                    prompt.Height = 250;
                    prompt.Text = "Edit Employee Details";
                    Label labelID = new Label() { Text = "Select Employee ID:", Left = 10, Top = 20, Width = 100 };
                    ComboBox comboBoxEmployeeID = new ComboBox();
                    comboBoxEmployeeID.DataSource = employeeIDs;
                    comboBoxEmployeeID.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBoxEmployeeID.Location = new System.Drawing.Point(120, 20);
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
                        string selectedEmployeeID = comboBoxEmployeeID.SelectedItem.ToString();
                        string selectedColumn = comboBoxColumn.SelectedItem.ToString();
                        string newValue = textBoxNewValue.Text;

                        // Validate phone number format
                        if (selectedColumn == "phone_number" && (newValue.Length != 11 || !newValue.All(char.IsDigit)))
                        {
                            MessageBox.Show("Phone number must be 11 digits long.");
                            return;
                        }

                        // Validate email format
                        if (selectedColumn == "email" && !newValue.Contains("@"))
                        {
                            MessageBox.Show("Invalid email format. Must contain '@' symbol.");
                            return;
                        }

                        // Update DataGridView and database with the new value
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(this.connectionString))
                            {
                                connection.Open();
                                string query = $"UPDATE Employee SET {selectedColumn} = @Value WHERE employee_ID = @ID";
                                SqlCommand cmd = new SqlCommand(query, connection);
                                cmd.Parameters.AddWithValue("@Value", newValue);
                                cmd.Parameters.AddWithValue("@ID", selectedEmployeeID);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Employee details updated successfully");
                                    LoadData(); // Refresh the data in dataGridView1 after editing
                                }
                                else
                                {
                                    MessageBox.Show("Failed to update employee details");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error updating employee details: " + ex.Message);
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
                    prompt.Controls.Add(comboBoxEmployeeID);
                    prompt.Controls.Add(labelColumn);
                    prompt.Controls.Add(comboBoxColumn);
                    prompt.Controls.Add(labelNewValue);
                    prompt.Controls.Add(textBoxNewValue);
                    prompt.Controls.Add(confirmation);
                    prompt.ShowDialog();
                }
                else
                {
                    MessageBox.Show("There are no employees to edit.");
                }
            }
            else
            {
                MessageBox.Show("There are no employees to edit.");
            }
        }

        private void Addbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if any of the required fields are empty
                if (string.IsNullOrWhiteSpace(textBoxEmployeeID.Text) ||
                    string.IsNullOrWhiteSpace(textBoxName.Text) ||
                    string.IsNullOrWhiteSpace(textBoxPhoneNumber.Text) ||
                    string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                    string.IsNullOrWhiteSpace(textBoxPosition.Text) ||
                    string.IsNullOrWhiteSpace(textBoxYearOfJoining.Text))
                {
                    MessageBox.Show("Please fill out all fields.");
                    return;
                }

                // Check if phone number has maximum of 11 digits
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

                // Open the connection
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO Employee (employee_ID, name, phone_number, email, position, year_of_joining) " +
                                         "VALUES (@EmployeeID, @Name, @PhoneNumber, @Email, @Position, @YearOfJoining)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@EmployeeID", textBoxEmployeeID.Text);
                        insertCmd.Parameters.AddWithValue("@Name", textBoxName.Text);
                        insertCmd.Parameters.AddWithValue("@PhoneNumber", textBoxPhoneNumber.Text);
                        insertCmd.Parameters.AddWithValue("@Email", textBoxEmail.Text);
                        insertCmd.Parameters.AddWithValue("@Position", textBoxPosition.Text);
                        insertCmd.Parameters.AddWithValue("@YearOfJoining", textBoxYearOfJoining.Text);

                        int rowsAffected = insertCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Employee details added successfully");
                            LoadData();
                            ResetFields();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add employee details");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding employee details: " + ex.Message);
            }
        }

        private void Resetbtn_Click(object sender, EventArgs e)
        {
            ResetFields();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
