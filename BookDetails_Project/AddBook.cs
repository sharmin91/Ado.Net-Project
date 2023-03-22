using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookDetails_Project
{
    public partial class AddBook : Form
    {
        string filePath="";
        List<Book> books = new List<Book>();
        public AddBook()
        {
            InitializeComponent();
        }
        public ICrossFromDataSync FormToReload { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionUtility.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    
                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO books 
                                            (bookid, title, publishdate, price, available,categoryid, publisherid, coverpage) VALUES
                                            (@i, @t, @d, @p, @a,@ct, @pb, @cv)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@t", textBox2.Text);
                        cmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@p", decimal.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@a", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@ct", comboBox1.SelectedValue);
                        cmd.Parameters.AddWithValue("@pb", comboBox2.SelectedValue);
                        string ext = Path.GetExtension(this.filePath);
                        string fileName = $"{Guid.NewGuid()}{ext}";
                        string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                        File.Copy(filePath, savePath, true);
                        cmd.Parameters.AddWithValue("@cv", fileName);
                       
                        try
                        {
                            if(cmd.ExecuteNonQuery()> 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                books.Add(new Book
                                {
                                    BookId = int.Parse(textBox1.Text),
                                    Title = textBox2.Text,
                                    PublishDate = dateTimePicker1.Value,
                                    Price = decimal.Parse(textBox3.Text),
                                    Available = checkBox1.Checked,
                                    CoverPage = fileName
                                }); ;
                                tran.Commit();
                            }
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                       finally
                        {
                            if(con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }

                    }
                }
                
            }
        }

        private void AddBook_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewBookId().ToString();
            LoadComboBox();
        }

        private void LoadComboBox()
        {
            using (SqlConnection con = new SqlConnection(ConnectionUtility.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM categories", con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "categories");
                    da.SelectCommand.CommandText = "SELECT * FROM publishers";
                    da.Fill(ds, "publishers");
                    comboBox1.DataSource= ds.Tables["categories"];
                    comboBox2.DataSource = ds.Tables["publishers"];
                }
            }
        }

        private int GetNewBookId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionUtility.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(bookid), 0) FROM books", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label7.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void AddBook_FormClosing(object sender, FormClosingEventArgs e)
        {
           // this.FormToReload.ReloadData(this.books);
        }
    }
}
