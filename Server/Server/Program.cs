using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    static class Program
    {
        public static ServerAction server;
        public static Form1 form;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //use points for floats for easy compatibility with coordinates
            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            server = new ServerAction();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new Form1();
            Application.Run(form);

        }

    }

    public class ServerClient
    {
        public string clientName;
        public TcpClient tcp;
        public ServerClient(TcpClient tcp)
        {
            this.tcp = tcp;
        }
    }

    public class Unit
    {
        public string clientName;
        public int unitID;
        public float unitPositionX;
        public float unitPositionY;
        public float unitPositionZ;
    }
}
