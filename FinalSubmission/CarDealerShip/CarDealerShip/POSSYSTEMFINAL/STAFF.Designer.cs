namespace POSSYSTEMFINAL
{
    partial class STAFF
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(STAFF));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Adminbtn = new POSSYSTEMFINAL.UI_Designs.Roundbuttons();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.Employeebtn = new POSSYSTEMFINAL.UI_Designs.Roundbuttons();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(96, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(226, 226);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // Adminbtn
            // 
            this.Adminbtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(112)))), ((int)(((byte)(98)))));
            this.Adminbtn.FlatAppearance.BorderSize = 0;
            this.Adminbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Adminbtn.Font = new System.Drawing.Font("Monospac821 BT", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Adminbtn.ForeColor = System.Drawing.Color.Black;
            this.Adminbtn.Location = new System.Drawing.Point(33, 286);
            this.Adminbtn.Name = "Adminbtn";
            this.Adminbtn.Size = new System.Drawing.Size(359, 133);
            this.Adminbtn.TabIndex = 3;
            this.Adminbtn.Text = "Admin";
            this.Adminbtn.UseVisualStyleBackColor = false;
            this.Adminbtn.Click += new System.EventHandler(this.Adminbtn_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(522, 35);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(226, 226);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // Employeebtn
            // 
            this.Employeebtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(112)))), ((int)(((byte)(98)))));
            this.Employeebtn.FlatAppearance.BorderSize = 0;
            this.Employeebtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Employeebtn.Font = new System.Drawing.Font("Monospac821 BT", 22.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Employeebtn.ForeColor = System.Drawing.Color.Black;
            this.Employeebtn.Location = new System.Drawing.Point(475, 286);
            this.Employeebtn.Name = "Employeebtn";
            this.Employeebtn.Size = new System.Drawing.Size(359, 133);
            this.Employeebtn.TabIndex = 5;
            this.Employeebtn.Text = "Employee";
            this.Employeebtn.UseVisualStyleBackColor = false;
            this.Employeebtn.Click += new System.EventHandler(this.Employeebtn_Click);
            // 
            // STAFF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.ClientSize = new System.Drawing.Size(878, 450);
            this.Controls.Add(this.Employeebtn);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.Adminbtn);
            this.Controls.Add(this.pictureBox1);
            this.Name = "STAFF";
            this.Text = "STAFF";
            this.Load += new System.EventHandler(this.STAFF_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private UI_Designs.Roundbuttons Adminbtn;
        private System.Windows.Forms.PictureBox pictureBox2;
        private UI_Designs.Roundbuttons Employeebtn;
    }
}