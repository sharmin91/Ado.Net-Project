using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookDetails_Project
{
    public partial class FormStart : Form, ICrossFromDataSync
    {
        DataSet ds;
        BindingSource bsBooks = new BindingSource();
        BindingSource bsTOCs = new BindingSource();
        public FormStart()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddBook() { FormToReload = this}.ShowDialog();
        }

        private void FormStart_Load(object sender, EventArgs e)
        {
            this.dataGridView1.AutoGenerateColumns = false;
            LoadData();
            BindData();
        }
        public void LoadData()
        {
            ds =   new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionUtility.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM books", con))
                {
                    da.Fill(ds, "books");
                    ds.Tables["books"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for(var i=0; i < ds.Tables["books"].Rows.Count; i++)
                    {
                        ds.Tables["books"].Rows[i]["image"] = File.ReadAllBytes( Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["books"].Rows[i]["coverpage"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM TOCs";
                    da.Fill(ds, "TOCs");
                    DataRelation rel = new DataRelation("FK_BOOK_TOC",
                        ds.Tables["books"].Columns["bookid"],
                        ds.Tables["TOCs"].Columns["bookid"]);
                    ds.Relations.Add(rel);
                    ds.AcceptChanges();
                }
            }
        }
        private void BindData()
        {
            bsBooks.DataSource = ds;
            bsBooks.DataMember = "books";
            bsTOCs.DataSource = bsBooks;
            bsTOCs.DataMember = "FK_BOOK_TOC";
            this.dataGridView1.DataSource = bsTOCs;
            lblTitle.DataBindings.Add(new Binding("Text", bsBooks, "title"));
            lblPub.DataBindings.Add(new Binding("Text", bsBooks, "publishdate"));
            lblPrice.DataBindings.Add(new Binding("Text", bsBooks, "price"));
            checkBox1.DataBindings.Add(new Binding("Checked", bsBooks, "available"));
            pictureBox1.DataBindings.Add(new Binding("Image", bsBooks, "image", true));
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if(bsBooks.Position < bsBooks.Count - 1)
            {
                bsBooks.MoveNext();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (bsBooks.Position > 0)
            {
                bsBooks.MovePrevious();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            bsBooks.MoveLast();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            bsBooks.MoveFirst();
        }

        public void ReloadData(List<Book> books)
        {

            foreach(var b in books)
            {
                DataRow dr = ds.Tables["books"].NewRow();
                dr[0] = b.BookId;
                dr["title"] = b.Title;
                dr["publishdate"] = b.PublishDate;
                dr["price"] = b.Price;
                dr["available"] = b.Available;
                dr["coverpage"] = b.CoverPage;
                dr["image"] =  File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), b.CoverPage));
                ds.Tables["books"].Rows.Add(dr);
                
            }
            ds.AcceptChanges();
            bsBooks.MoveLast();
            
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
           int id = (int)(this.bsBooks.Current as DataRowView).Row[0];
            new EditBook { BookToEditDelete = id,FormToReload = this  }.ShowDialog();
        }

        public void UpdateBook(Book b)
        {
            for(var i=0; i < ds.Tables["books"].Rows.Count; i++)
            {
                if ((int)ds.Tables["books"].Rows[i]["bookid"] == b.BookId)
                {
                    ds.Tables["books"].Rows[i]["title"] = b.Title;
                    ds.Tables["books"].Rows[i]["publishdate"] = b.PublishDate;
                    ds.Tables["books"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), b.CoverPage));
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemoveBook(int id)
        {
            for (var i = 0; i < ds.Tables["books"].Rows.Count; i++)
            {
                if ((int)ds.Tables["books"].Rows[i]["bookid"] == id)
                {
                    ds.Tables["books"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }

        private void booksTOCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormBookGroupRpt().ShowDialog();
        }

        private void booksToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new FormBookRpt().ShowDialog();
        }
    }
}
