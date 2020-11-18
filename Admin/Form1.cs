using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Admin
{
    public partial class Form1 : Form
    {
        SqlConnection conn;
        SqlDataReader reader;
        SqlCommand cmd;
        public int tagihan = 0;
        public string linkfoto;
        public Form1()
        {
            InitializeComponent();
        
            conn = new SqlConnection("server = localhost; data source = localhost\\SQLEXPRESS; database = DB_DATA; integrated security = SSPI");
            conn.Open();

            btnCetakSub.Enabled = false;
            nudHarga.Minimum = 5000;
            nudHarga.Maximum = 100000;
            nudHarga.Increment = 250;
            lblPenjualan.Text = "";
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            lblMeja1.BackColor = Color.Blue;
            lblMeja2.BackColor = Color.Blue;
            lblMeja3.BackColor = Color.Blue;
            lblMeja4.BackColor = Color.Blue;
            lblMeja5.BackColor = Color.Blue;
            lblMeja6.BackColor = Color.Blue;
            lblMeja7.BackColor = Color.Blue;
            lblMeja8.BackColor = Color.Blue;
            cmd = new SqlCommand("select TableNum from Meja where flag = 1 group by TableNum", conn);
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
                while (reader.Read())
                {
                    string temp = reader[0].ToString();
                    if (temp == "1")
                        lblMeja1.BackColor = Color.Yellow;
                    else if (temp == "2")
                        lblMeja2.BackColor = Color.Yellow;
                    else if (temp == "3")
                        lblMeja3.BackColor = Color.Yellow;
                    else if (temp == "4")
                        lblMeja4.BackColor = Color.Yellow;
                    else if (temp == "5")
                        lblMeja5.BackColor = Color.Yellow;
                    else if (temp == "6")
                        lblMeja6.BackColor = Color.Yellow;
                    else if (temp == "7")
                        lblMeja7.BackColor = Color.Yellow;
                    else if (temp == "8")
                        lblMeja8.BackColor = Color.Yellow;
                }
            reader.Close();
        }

        private void lblMeja1_Click(object sender, EventArgs e)
        {
            CetakBilling("1", lblMeja1.BackColor);
        }

        private void lblMeja2_Click(object sender, EventArgs e)
        {
            CetakBilling("2", lblMeja2.BackColor);
        }

        private void lblMeja3_Click(object sender, EventArgs e)
        {
            CetakBilling("3", lblMeja3.BackColor);
        }

        private void lblMeja4_Click(object sender, EventArgs e)
        {
            CetakBilling("4", lblMeja4.BackColor);
        }
        private void lblMeja5_Click(object sender, EventArgs e)
        {
            CetakBilling("5", lblMeja5.BackColor);
        }
        private void lblMeja6_Click(object sender, EventArgs e)
        {
            CetakBilling("6", lblMeja6.BackColor);
        }
        private void lblMeja7_Click(object sender, EventArgs e)
        {
            CetakBilling("7", lblMeja7.BackColor);
        }
        private void lblMeja8_Click(object sender, EventArgs e)
        {
            CetakBilling("8", lblMeja8.BackColor);
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            if (dgvMeja.Rows.Count > 1)
            {
                tagihan = 0;

                for (int i = 0; i < dgvMeja.Rows.Count; i++)
                    tagihan += Convert.ToInt32(dgvMeja.Rows[i].Cells[3].Value);

                string sql = String.Format("insert into Admin values('{0}',convert(varchar,cast({1} as money)),'{2}')", lblNoMeja.Text, tagihan, DateTimeOffset.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                PrintPreviewDialog ppd1 = new PrintPreviewDialog();
                ppd1.Document = printDocument1;
                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("tes", 285, 600);
                ppd1.ShowDialog();

                sql = String.Format("Update Meja set flag = 0 where TableNum = '{0}'", lblNoMeja.Text);
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                sql = String.Format("Delete from Meja where TableNum = '{0}'", lblNoMeja.Text);
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                dgvMeja.DataSource = null;

                Form1_Load(null, null);
            }
        }

        private void dtpTanggal_ValueChanged(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter($"select * from Admin where Tanggal = '{dtpTanggal.Value.ToString("yyyy/MM/dd")}'", conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "Rekap");
            dgvPenjualan.DataSource = ds.Tables["Rekap"];
            int sum = 0;
            for (int i = 0; i < dgvPenjualan.Rows.Count; i++)
                sum += Convert.ToInt32(dgvPenjualan.Rows[i].Cells[1].Value);
            lblPenjualan.Text = $"Rp.{sum:N}";
            if (dgvPenjualan.Rows.Count < 1)
                dgvPenjualan.DataSource = null;
        }

        private void txtNama_Leave(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            string sql = String.Format("select NamaMenu, GambarMenu, HargaMenu from Menu where NamaMenu = '{0}'", txtNama.Text);
            cmd = new SqlCommand(sql, conn);
            reader = cmd.ExecuteReader();
            btnSave.Text = "Save";
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    nudHarga.Value = Convert.ToDecimal(reader[2]);
                    pictureBox1.Image = Image.FromFile($"{reader[1].ToString()}");
                    linkfoto = reader[1].ToString();
                }

                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
            reader.Close();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Title = "Images";
            openFileDialog1.Filter = "All Images|*.jpg;*.png;*.bmp";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.ToString() != "")
            {
                linkfoto = openFileDialog1.FileName.ToString();
                pictureBox1.Image = Image.FromFile(linkfoto);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                if (btnSave.Text == "Update")
                {
                    string sql = String.Format("update Menu set HargaMenu = convert(money,{0}), GambarMenu = '{1}' where NamaMenu = '{2}'", nudHarga.Value, linkfoto, txtNama.Text);
                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                }
                else
                {
                    string sql = String.Format("Insert into Menu values('{0}','{1}',convert(money,{2}),0)", txtNama.Text, linkfoto, nudHarga.Value);
                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                Reset();
            }

        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (btnSave.Text == "Update")
            {
                string sql = String.Format("Delete from Menu where NamaMenu = '{0}'", txtNama.Text);
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                Reset();
            }

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("Mosh Garden", new Font("Arial", 20, FontStyle.Bold), Brushes.Black, new Point(50, 10));
            e.Graphics.DrawLine(Pens.Black, new Point(0, 50), new Point(285, 50));
            e.Graphics.DrawLine(Pens.Black, new Point(0, 53), new Point(285, 53));
            e.Graphics.DrawString($"{DateTimeOffset.Now.ToString("dd MMM yyyy hh:mm:ss")}", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(60, 60));

            string sql = String.Format("select m.NamaMenu,Qty, (HargaMenu * Qty) from Meja m inner join Menu n on m.NamaMenu = n.NamaMenu where TableNum = '{0}'", lblNoMeja.Text);
            cmd = new SqlCommand(sql, conn);
            reader = cmd.ExecuteReader();
            int inc = 0, bott = 110;
            e.Graphics.DrawString("Pesanan", new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(30, 90));
            e.Graphics.DrawString("@", new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(120, 90));
            e.Graphics.DrawString("Total", new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(150, 90));
            while (reader.Read())
            {
                e.Graphics.DrawString($"{reader[0].ToString()}", new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(30, bott + inc));
                e.Graphics.DrawString($"{reader[1].ToString()}", new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(120, bott + inc));
                e.Graphics.DrawString($"Rp.{reader[2].ToString()}", new Font("Arial", 7, FontStyle.Regular), Brushes.Black, new Point(150, bott + inc));
                inc += 15;
            }
            e.Graphics.DrawString($"Grand Total ....... Rp.{tagihan.ToString()}", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, new Point(48, 250));
            e.Graphics.DrawString("Come Again", new Font("Lucida Handwriting", 25, FontStyle.Bold), Brushes.Black, new Point(25, 270));
            reader.Close();
        }

        public void CetakBilling(string noMeja, Color bg)
        {
            SqlDataAdapter da = new SqlDataAdapter($"select t.NamaMenu, HargaMenu, Qty, HargaMenu*Qty Total, Tanggal from Menu m inner join Meja t on m.NamaMenu = t.NamaMenu where TableNum = {noMeja}", conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "Menu");
            dgvMeja.DataSource = ds.Tables["Menu"];
            lblNoMeja.Text = noMeja;
            if (bg == Color.Yellow)
                btnCetakSub.Enabled = true;
            else
                btnCetakSub.Enabled = false;
        }

        public void Reset()
        {
            btnSave.Text = "Save";
            txtNama.Text = "";
            nudHarga.Value = nudHarga.Minimum;
            pictureBox1.Image = null;
            linkfoto = "";
            btnSave.Enabled = false;
            btnDelete.Enabled = false;
        }
    }
}
