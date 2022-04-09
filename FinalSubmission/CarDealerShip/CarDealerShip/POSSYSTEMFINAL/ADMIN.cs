using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vehicles;
using POSSYSTEMFINAL;

namespace POSSYSTEMFINAL
{
    public partial class ADMIN : Form
                
    {
        public string connectionString;
        public ADMIN(string connectionString)
        {
            this.connectionString = connectionString;
            InitializeComponent();
        }

        private void ADMIN_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ADMIN vehicle = new ADMIN(connectionString);
            vehicle.ShowDialog();
            this.Hide();
        }

        private void Financebtn_Click(object sender, EventArgs e)
        {
            FINANCES fINANCES = new FINANCES(connectionString);
            fINANCES.ShowDialog();
            this.Hide();    
        }

        private void roundbuttons1_Click(object sender, EventArgs e)
        {
            EmployeesManage employee = new EmployeesManage(connectionString);
            employee.ShowDialog();
            this.Hide();
        }

        private void roundbuttons2_Click(object sender, EventArgs e)
        {
         VehicleManager vehicleManager = new VehicleManager(connectionString);
            vehicleManager.ShowDialog();
            this.Hide();
           
        }

        private void ManageCustomersbtn_Click(object sender, EventArgs e)
        {
            Customers customers = new Customers(connectionString);
            customers.ShowDialog();
            this.Hide();
        }
    }
}
