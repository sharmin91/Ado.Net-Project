using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookDetails_Project
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormStart { MdiParent = this }.Show();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddBook { MdiParent= this }.Show();
        }
    }
}
