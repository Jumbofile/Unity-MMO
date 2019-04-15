using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        //timer component calls this at fixed time intervals, to maintain
        private void timer1_Tick(object sender, EventArgs e)
        {
           Program.server.Update();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.usersTableAdapter.Fill(this.databaseDataSet.Users);
            string hostName = Dns.GetHostName(); // retrive the Name of HOST  
            // Get the IP  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            label1.Text = "Server's IP address is: " + myIP;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create a new instance of the Form2 class
            CreateUser registerForm = new CreateUser();

            // Show the settings form
            registerForm.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
