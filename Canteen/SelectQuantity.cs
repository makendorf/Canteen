using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Canteen
{
    public partial class SelectQuantity : MetroForm
    {
        public string ReturnValue { get; set; }
        public SelectQuantity()
        {
            InitializeComponent();
        }
        public SelectQuantity(string str)
        {
            InitializeComponent();
            Text = str;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            ReturnValue = metroTextBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
