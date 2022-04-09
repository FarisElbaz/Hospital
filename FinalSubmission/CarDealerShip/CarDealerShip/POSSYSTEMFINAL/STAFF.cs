using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using POSSYSTEMFINAL;
namespace POSSYSTEMFINAL
{
    public partial class STAFF : Form
    {
        private string onlineConnectionString = @"Data Source=34.105.253.161;Initial Catalog=Cardearlership;Persist Security Info=True;User ID=sqlserver;Password=Faris200510;Encrypt=False;";


        public STAFF()
        {
            InitializeComponent();
        }

        private void STAFF_Load(object sender, EventArgs e)
        {

        }

        private void Adminbtn_Click(object sender, EventArgs e)
        {
            ADMIN aDMIN = new ADMIN(onlineConnectionString);
            DialogResult result = aDMIN.ShowDialog();
            if (result == DialogResult.OK)
            {
                aDMIN.Close();
            }
        }

        private void Employeebtn_Click(object sender, EventArgs e)
        {
            // Create an instance of ProductCatalog form
            ProductCatalog productCatalog = new ProductCatalog();

            // Show the ProductCatalog form as a dialog
            DialogResult result = productCatalog.ShowDialog();

            // After the dialog is closed, check the result
            if (result == DialogResult.OK)
            {
                // Close the ProductCatalog form if the result is OK
                productCatalog.Close();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
