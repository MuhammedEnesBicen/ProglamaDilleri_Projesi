using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prog1D
{
    public partial class Form3 : Form
    {
     
        public Form3()
        {
            InitializeComponent();
            
        }
       
        private void button1_Click(object sender, EventArgs e)
        {

            Form2 frm2 = new Form2();
            frm2.ShowDialog();
            Close();

        }
       
      
        private void button2_Click(object sender, EventArgs e)
        {

            Form4 frm4 = new Form4();
            frm4.ShowDialog();
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
