using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeThiHK1_2023
{
    public partial class Form1 : Form
    {
        public SqlConnection conn = new SqlConnection();
        Functions func = new Functions();

        public Form1()
        {
            InitializeComponent();

            // Fix hiển thị datagridview
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            func.KetNoi(conn);
            func.LoadComb(combHD, "select * from HoaDon", conn, "MaHD", "MaHD");
            func.LoadComb(combMatHang, "select * from MatHang", conn, "TenHang", "MaHang");
            func.HienthiDulieuDG(dataGridView1, "select hd.MaHD,  hd.KhachHang, hd.NgayLap, mh.TenHang, ct.SoLuong, ct.DonGia, (ct.SoLuong * ct.DonGia) as ThanhTien from MatHang mh, HoaDon hd, ChiTiet_HD ct where ct.Mahang = mh.Mahang and ct.MaHD = hd.MaHD", conn);

            groupBox1.Enabled = true;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
        }

        private void btnThemHang_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
        }

        private void btnChonLaiHD_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
        }

        private void btnResetMatHang_Click(object sender, EventArgs e)
        {
            combMatHang.Text = "Chọn mặt hàng";
            txtDonGia.Text = "";
            txtSL.Text = "";
        }

        private void combHD_SelectedValueChanged(object sender, EventArgs e)
        {
            string mahd = combHD.Text;
            string sql = "select * from HoaDon where MaHD = '" + mahd + "'";
            SqlCommand comd = new SqlCommand(sql, conn);
            SqlDataReader reader = comd.ExecuteReader();
            if (reader.Read())
            {
                string date = reader.GetValue(1).ToString();
                datetimeNgayLap.Text = String.Format("{0:MM/dd/yyyy}", date);
                txtNhanVien.Text = reader.GetValue(2).ToString(); ;
                txtKhachHang.Text = reader.GetValue(3).ToString();
            }
            reader.Close();

           // Tính tổng tiền từng hóa đơn
            string sql_tong = "select sum(SoLuong * DonGia) as tong from ChiTiet_HD where MaHD = '" + mahd + "'";
            SqlCommand cmd = new SqlCommand(sql_tong, conn);
            SqlDataReader reader_tong = cmd.ExecuteReader();
            if (reader_tong.Read())
            {
                labelTongTien.Text = "Tổng tiền: " + reader_tong.GetValue(0).ToString();
            }
            reader_tong.Close();

        }

        private void btnThemMatHang_Click(object sender, EventArgs e)
        {
            string sql = "insert into ChiTiet_HD values('" + combHD.Text + "', '" + combMatHang.SelectedValue.ToString() + "', '" + txtSL.Text + "', '" + txtDonGia.Text + "')";
            func.CapNhat(sql, conn);
            MessageBox.Show("Thêm hóa đơn bán háng thành công.");
            func.HienthiDulieuDG(dataGridView1, "select hd.MaHD, hd.KhachHang, hd.NgayLap, mh.TenHang, ct.SoLuong, ct.DonGia, (ct.SoLuong * ct.DonGia) as ThanhTien from MatHang mh, HoaDon hd, ChiTiet_HD ct where ct.Mahang = mh.Mahang and ct.MaHD = hd.MaHD", conn);
            
            // Hiển thị tổng tiền hóa đơn
            string sql_tong = "select sum(SoLuong * DonGia) as tong from ChiTiet_HD where MaHD = '" + combHD.Text + "'";
            SqlCommand cmd = new SqlCommand(sql_tong, conn);
            SqlDataReader reader_tong = cmd.ExecuteReader();
            if (reader_tong.Read())
            {
                labelTongTien.Text = "Tổng tiền: " + reader_tong.GetValue(0).ToString();
            }
            reader_tong.Close();
        }

        private void txtSL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtDonGia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
