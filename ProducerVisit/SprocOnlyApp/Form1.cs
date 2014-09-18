using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SprocOnlyApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // create a database connection:
        //NorthWind db = new NorthWind(@"c:\linqtest7\northwnd.mdf");
        NorthWind db = new NorthWind(@"Data Source=tcp:dl-sql2008-test.dairydata.local,1433;Initial Catalog=NorthWind;Integrated Security=True;");


        //public void CreateOleDbConnection()
        //{
        //    OleDbConnection connection = new OleDbConnection();
        //    connection.ConnectionString =
        //        "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Northwind.mdb";
        //    Console.WriteLine("Connection State: " + connection.State.ToString());
        //}

        public void CreateOleDbConnection()
        {
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString =
                "Data Source=tcp:dl-sql2008-test.dairydata.local,1433;Initial Catalog=NorthWind;Integrated Security=True;";
            Console.WriteLine("Connection State: " + connection.State.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Declare a variable to hold the contents of 
            // textBox1 as an argument for the stored 
            // procedure. 
            string param = textBox1.Text;

            // Declare a variable to hold the results 
            // returned by the stored procedure. 
            var custquery = db.CustOrdersDetail(Convert.ToInt32(param));

            // Execute the stored procedure and display the results. 
            string msg = "";
            foreach (CustOrdersDetailResult custOrdersDetail in custquery)
            {
                msg = msg + custOrdersDetail.ProductName + "\n";
            }
            if (msg == "")
                msg = "No results.";
            MessageBox.Show(msg);

            // Clear the variables before continuing.
            param = "";
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Comments in the code for button2 are the same 
            // as for button1. 
            string param = textBox2.Text;

            var custquery = db.CustOrderHist(param);

            string msg = "";
            foreach (CustOrderHistResult custOrdHist in custquery)
            {
                msg = msg + custOrdHist.ProductName + "\n";
            }
            MessageBox.Show(msg);

            param = "";
            textBox2.Text = "";
        }
    }
}
