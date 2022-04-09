using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace POSSYSTEMFINAL
{
    public partial class ShoppingCart : Form
    {
        public List<Vehicle> cartItems = new List<Vehicle>();
        private List<int> deletedVehicleIds = new List<int>(); // Track deleted vehicle IDs
        private string connectionString;

        public ShoppingCart(string connectionString, List<int> deletedVehicleIds)
        {
            InitializeComponent();
            this.connectionString = connectionString;
            this.deletedVehicleIds = deletedVehicleIds;
            this.FormClosing += ShoppingCart_FormClosing;
        }

        // Method to add an item to the cart
        public void AddToCart(Vehicle vehicle)
        {
            cartItems.Add(vehicle);
            DisplayCartItems();
        }

        // Method to display cart items
        public void DisplayCartItems()
        {
            // Clear existing items
            flowLayoutPanel.Controls.Clear();

            decimal totalCost = 0; // Initialize total cost

            foreach (var vehicle in cartItems)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Image = Image.FromFile(vehicle.ImagePath);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Width = 100;
                pictureBox.Height = 70;

                Button btnRemove = new Button();
                btnRemove.Text = "Remove";
                btnRemove.Tag = vehicle;
                btnRemove.Click += BtnRemove_Click;

                Label label = new Label();
                label.Text = $"{vehicle.Type} {vehicle.Model} ({vehicle.Year}) - ${vehicle.Price}";

                // Add controls to the flow layout panel
                flowLayoutPanel.Controls.Add(pictureBox);
                flowLayoutPanel.Controls.Add(label);
                flowLayoutPanel.Controls.Add(btnRemove);

                // Add the price of the vehicle to the total cost
                totalCost += vehicle.Price;
            }

            // show total cost in label1
            label1.Text = $"Total Cost: ${totalCost}";
        }
        private void BtnRemove_Click(object sender, EventArgs e)
        {
            Button btnRemove = (Button)sender;
            Vehicle vehicleToRemove = (Vehicle)btnRemove.Tag; // get the associated vehicle object

            // remove the item from the cart
            cartItems.Remove(vehicleToRemove);
            deletedVehicleIds.Add(vehicleToRemove.Id); // store the ID of the removed vehicle

            // update the cart itms
            DisplayCartItems();
        }

        private void flowLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        private void ShoppingCart_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide(); 
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var vehicle in cartItems)
                {
                    deletedVehicleIds.Add(vehicle.Id);
                    string query = "DELETE FROM Vehicle WHERE Type = @Type AND Price = @Price AND Year = @Year";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Type", vehicle.Type);
                    command.Parameters.AddWithValue("@Price", vehicle.Price);
                    command.Parameters.AddWithValue("@Year", vehicle.Year);

                    command.ExecuteNonQuery();
                }
                cartItems.Clear();
                DisplayCartItems();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cartItems.Clear();

            DisplayCartItems();

            deletedVehicleIds.Clear();

        }

    }
}
