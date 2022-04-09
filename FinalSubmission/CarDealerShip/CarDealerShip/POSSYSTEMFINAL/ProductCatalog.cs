using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.Timers;
using System.Linq;
using POSSYSTEMFINAL;
using System.Diagnostics;


public partial class ProductCatalog : Form
{
    private ShoppingCart shoppingCart;
    private List<Vehicle> vehicles = new List<Vehicle>();
    private List<int> deletedVehicleIds = new List<int>(); 
    private string onlineConnectionString = @"Data Source=34.105.253.161;Initial Catalog=Cardearlership;Persist Security Info=True;User ID=sqlserver;Password=Faris200510;Encrypt=False;";
    private string offlineConnectionString = @"Data Source=(localdb)\Local;Initial Catalog=CarDealer;Integrated Security=True;Encrypt=false;";
    private System.Timers.Timer syncTimer;
    private List<Vehicle> cartItems = new List<Vehicle>();

    public ProductCatalog()
    {
        InitializeComponent();
        try
        {
            SyncVehiclesFromOnlineToOffline();
            PopulateVehiclesFromDatabase();
            DisplayVehicles();
            InitializeSyncTimer();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
        }
        shoppingCart = new ShoppingCart(offlineConnectionString, deletedVehicleIds);

    }
    private void SyncVehiclesFromOnlineToOffline()
    {
        try
        {
            using (SqlConnection onlineConnection = new SqlConnection(onlineConnectionString))
            {
                onlineConnection.Open();
                string query = "SELECT V_ID, Type, Brand, Year, Price, Color, Mileage_, Model, ImagePath FROM Vehicle";
                SqlCommand command = new SqlCommand(query, onlineConnection);
                SqlDataReader reader = command.ExecuteReader();
                using (SqlConnection offlineConnection = new SqlConnection(offlineConnectionString))
                {
                    offlineConnection.Open();
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Vehicle WHERE V_ID = @Id", offlineConnection);
                        checkCommand.Parameters.AddWithValue("@Id", id);
                        int exists = (int)checkCommand.ExecuteScalar();
                        if (exists == 0)
                        {
                            string insertQuery = "INSERT INTO Vehicle (V_ID, Type, Brand, Year, Price, Color, Mileage_, Model, ImagePath) VALUES (@Id, @Type, @Brand, @Year, @Price, @Color, @Mileage_, @Model, @ImagePath)";
                            SqlCommand insertCommand = new SqlCommand(insertQuery, offlineConnection);
                            insertCommand.Parameters.AddWithValue("@Id", id);
                            insertCommand.Parameters.AddWithValue("@Type", reader["Type"]);
                            insertCommand.Parameters.AddWithValue("@Brand", reader["Brand"]);
                            insertCommand.Parameters.AddWithValue("@Year", reader["Year"]);
                            insertCommand.Parameters.AddWithValue("@Price", reader["Price"]);
                            insertCommand.Parameters.AddWithValue("@Color", reader["Color"]);
                            insertCommand.Parameters.AddWithValue("@Mileage_", reader["Mileage_"]);
                            insertCommand.Parameters.AddWithValue("@Model", reader["Model"]);
                            insertCommand.Parameters.AddWithValue("@ImagePath", reader["ImagePath"]);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
                reader.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Online Database connection could not be established. Error: " + ex.Message + "\nOnly data found in the offline database will be displayed without updating.");
        }
    }

    private Vehicle FindVehicleById(string id)
    {
        return vehicles.FirstOrDefault(v => v.Id.ToString() == id);
    }

    private void PopulateVehiclesFromDatabase(string brand = "", string vehicleType = "")
    {
        using (SqlConnection connection = new SqlConnection(offlineConnectionString))
        {
            string query = "SELECT V_ID, Type, Brand, Year, Price, Color, Mileage_, Model, ImagePath FROM Vehicle WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(brand))
                query += $" AND Brand = '{brand}'";
            if (!string.IsNullOrWhiteSpace(vehicleType))
                query += $" AND Type = '{vehicleType}'";

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0); 
                string type = reader.GetString(1);
                string model = reader.GetString(2);
                int year = reader.GetInt32(3);
                decimal price = reader.GetDecimal(4);
                string imagePath = reader.GetString(8); // ImagePath is at index 8
                vehicles.Add(new Vehicle(id, type, model, year, price, imagePath));
            }
            reader.Close();
        }
    }

    private void DisplayVehicles()
    {
        flowLayoutPanel.Controls.Clear();
        foreach (var vehicle in vehicles)
        {
            try
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Image = Image.FromFile(vehicle.ImagePath);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Width = 150;
                pictureBox.Height = 100;
                pictureBox.Click += (sender, e) => AddToCart(vehicle);

                Label label = new Label();
                label.Text = $"{vehicle.Type} {vehicle.Model} ({vehicle.Year}) - ${vehicle.Price}";

                flowLayoutPanel.Controls.Add(pictureBox);
                flowLayoutPanel.Controls.Add(label);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
        }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
        string brand = txtBrand.Text.Trim();
        string type = txtType.Text.Trim();
        try
        {
            vehicles.Clear();
            PopulateVehiclesFromDatabase(brand, type);
            DisplayVehicles();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching: {ex.Message}");
        }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
        txtBrand.Text = "";
        txtType.Text = "";
        vehicles.Clear();
        PopulateVehiclesFromDatabase();
        DisplayVehicles();
    }

    private void button1_Click(object sender, EventArgs e)
    {

    }

    private void flowLayoutPanel_Paint(object sender, PaintEventArgs e)
    {

    }

    private void txtBrand_TextChanged(object sender, EventArgs e)
    {

    }

    private void flowLayoutPanel_Paint_1(object sender, PaintEventArgs e)
    {

    }

    private bool IsDatabaseOnline()
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(onlineConnectionString))
            {
                connection.Open();
                Debug.WriteLine("Online database connection established.");
                return true;
            }
        }
        catch
        {
            Debug.WriteLine("Failed to connect to the online database.");
            return false;
        }
    }

    private void AddToCart(Vehicle vehicle)
    {
        shoppingCart.AddToCart(vehicle);
        MessageBox.Show($"Added {vehicle.Type} {vehicle.Model} to cart.");
        Console.WriteLine("TestTest123");
    }


    private void InitializeSyncTimer()
    {
        syncTimer = new System.Timers.Timer(30000); // Sync every hour
        syncTimer.Elapsed += OnTimedEvent;
        syncTimer.AutoReset = true;
        syncTimer.Enabled = true;
        Debug.WriteLine("Timer initialized and enabled");
    }

    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        SyncDataToOnline();
        Debug.WriteLine("Timer event occurred at {0}", e.SignalTime);
    }

    private void SyncDataToOnline()
    {
        if (!IsDatabaseOnline()) return;
        Debug.WriteLine("SyncDataToOnline called");
        Debug.WriteLine("Deleted vehicle IDs: " + string.Join(", ", deletedVehicleIds));

        try
        {
            using (SqlConnection offlineConnection = new SqlConnection(offlineConnectionString))
            {
                offlineConnection.Open();
                string query = "SELECT * FROM Vehicle";
                SqlCommand command = new SqlCommand(query, offlineConnection);
                SqlDataReader reader = command.ExecuteReader();
                List<int> syncedIds = new List<int>();
                using (SqlConnection onlineConnection = new SqlConnection(onlineConnectionString))
                {
                    onlineConnection.Open();
                    foreach (int id in deletedVehicleIds)
                    {
                        string deleteQuery = "DELETE FROM Vehicle WHERE V_ID = @Id";
                        SqlCommand deleteCommand = new SqlCommand(deleteQuery, onlineConnection);
                        deleteCommand.Parameters.AddWithValue("@Id", id);
                        deleteCommand.ExecuteNonQuery();

                        string financeInsertQuery = "INSERT INTO Finance (Name, Brand, Price) " + "VALUES (@Name, @Brand, @Price)";
                        SqlCommand financeInsertCommand = new SqlCommand(financeInsertQuery, onlineConnection);
                        Vehicle deletedVehicle = vehicles.FirstOrDefault(v => v.Id == id);
                        financeInsertCommand.Parameters.AddWithValue("@Name", $"{deletedVehicle.Type}");
                        financeInsertCommand.Parameters.AddWithValue("@Brand", deletedVehicle.Model);
                        financeInsertCommand.Parameters.AddWithValue("@Price", deletedVehicle.Price);
                        financeInsertCommand.ExecuteNonQuery();
                    }
                    Debug.WriteLine("Synced completed");
                    deletedVehicleIds.Clear();
                }
                reader.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error syncing data: {ex.Message}");
        }
    }
    private void button1_Click_2(object sender, EventArgs e)
    {
        shoppingCart.Show();
    }


}


public class Vehicle
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string ImagePath { get; set; }

    public Vehicle(int id, string type, string model, int year, decimal price, string imagePath)
    {
        Id = id;
        Type = type;
        Model = model;
        Year = year;
        Price = price;
        ImagePath = imagePath;
    }
}
