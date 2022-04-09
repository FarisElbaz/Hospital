using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POSSYSTEMFINAL
{
    
    public partial class Intro : Form
    {
        private Timer timer;
        
        public Intro()
        {
            InitializeComponent();
            //Initialize the timer and give time of 3 seconds
            timer = new Timer();
            timer.Interval = 2000;
            timer.Tick += timer1_Tick;

            // Start the timer when the intro form is shown
            this.Shown += (sender, e) =>
            {
                timer.Start();
            };
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
         
                // Stop the timer
                timer.Stop();

                // Open the STAFF form
                STAFF staffForm = new STAFF();
                DialogResult result =  staffForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                // Close the ProductCatalog form if the result is OK
                staffForm.Close(); 
            }

            // Hide the intro form

        }

        private void Intro_Load(object sender, EventArgs e)
        {
           
        }
    }
    }

