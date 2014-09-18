using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SprocOnlyApp
{
    public partial class ProducerSearchDialog : Form
    {
        public ProducerSearchDialog()
        {
            InitializeComponent();
            this.Text = "Producer Search";
        }

        Enterprise db = new Enterprise(@"Data Source=tcp:dl-sqlmilk-01.dairydata.local,1433;Initial Catalog=Enterprise;Integrated Security=True;");

        private void button1_Click(object sender, EventArgs e)
        {
            // Declare a variable to hold the contents of 
            // textBox1 as an argument for the stored 
            // procedure. 
            string param = textBox1.Text;

            // Declare a variable to hold the results 
            // returned by the stored procedure. 
            bool includeActive = true;
            bool includeInactive = false;

            var producerQuery = db.SearchProducer(param, includeActive, includeInactive);

            // Execute the stored procedure and display the results. 
            string msg = string.Empty;
            string producerInformation = string.Empty;
            int count = 0;

            foreach (SearchProducerResult producerResult in producerQuery)
            {
                producerInformation = producerResult.Producer_Division + producerResult.Producer_Number + ", " + producerResult.Farm_Name;
                msg = msg + producerInformation + "\n";
                count++;

                if (count > 19)
                {
                    break;
                }
            }

            if (msg == "")
                msg = "No results.";
            MessageBox.Show(msg, "Search result", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Clear the variables before continuing.
            param = "";
            textBox1.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
