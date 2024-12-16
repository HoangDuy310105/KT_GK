using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace KT_GK
{
    public partial class Form1 : Form
    {
        private Model1 _context;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtTimkiem.KeyDown += txtTimkiem_KeyDown;
            _context = new Model1();
            var lops = _context.Lops.ToList();
            cbLop.DataSource = lops;
            cbLop.DisplayMember = "TenLop";
            cbLop.ValueMember = "MaLop";
            LoadListView();
        }
        private void LoadListView()
        {
            lvSinhvien.Items.Clear();
            var Sinhviens = _context.Sinhviens.Include(s => s.Lop).ToList();
            foreach (var sv in Sinhviens)
            {
                var item = new ListViewItem(sv.MaSV);
                item.SubItems.Add(sv.HotenSV);
                item.SubItems.Add(sv.Ngaysinh.ToString("dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture));
                item.SubItems.Add(sv.Lop.TenLop);
                lvSinhvien.Items.Add(item);
            }
        }

        private void btnThêm_Click(object sender, EventArgs e)
        {
            var sv = new Sinhvien
            {
                MaSV = txtMaSV.Text,
                HotenSV = txtHoten.Text,
                Ngaysinh = dtNgaySinh.Value,
                Malop = cbLop.SelectedValue.ToString()
            };

            _context.Sinhviens.Add(sv);
            _context.SaveChanges();

            MessageBox.Show("Thêm sinh viên thành công!","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Question);
            LoadListView();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                var maSV = txtMaSV.Text;
                var sinhvien = _context.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);

                if (sinhvien == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần sửa!");
                    return;
                }

                sinhvien.HotenSV = txtHoten.Text; 
                sinhvien.Ngaysinh = dtNgaySinh.Value;
                sinhvien.Malop = cbLop.SelectedValue.ToString();

                _context.SaveChanges();

                MessageBox.Show("Cập nhật thông tin sinh viên thành công!");
                LoadListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                var maSV = txtMaSV.Text;
                var sv = _context.Sinhviens.FirstOrDefault(s => s.MaSV == maSV);

                if (sv == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên!");
                    return;
                }

                var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    _context.Sinhviens.Remove(sv);
                    _context.SaveChanges();

                    MessageBox.Show("Xóa sinh viên thành công!");
                    LoadListView();
                    ClearForm(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
        private void ClearForm()
        {
            txtMaSV.Clear();
            txtHoten.Clear();
            dtNgaySinh.Value = DateTime.Now;
            cbLop.SelectedIndex = -1; 
        }
        private void lvSinhvien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSinhvien.SelectedItems.Count > 0)
            {
                var selectedItem = lvSinhvien.SelectedItems[0];
                txtMaSV.Text = selectedItem.SubItems[0].Text; 
                txtHoten.Text = selectedItem.SubItems[1].Text;
                dtNgaySinh.Value = DateTime.ParseExact(selectedItem.SubItems[2].Text, "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                cbLop.Text = selectedItem.SubItems[3].Text; 
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            var keyword = txtTimkiem.Text.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadListView();
                return;
            }

            var Sinhviens = _context.Sinhviens
                .Include(s => s.Lop)
                .Where(s => s.HotenSV.Contains(keyword))
                .ToList();
            
            lvSinhvien.Items.Clear();
            if (Sinhviens.Count == 0)
            {
                MessageBox.Show("Không tìm thấy sinh viên nào có tên phù hợp!");
                return;
            }

            foreach (var sv in Sinhviens)
            {
                var item = new ListViewItem(sv.MaSV);
                item.SubItems.Add(sv.HotenSV);
                item.SubItems.Add(sv.Ngaysinh.ToString("dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture));
                item.SubItems.Add(sv.Lop.TenLop);
                lvSinhvien.Items.Add(item);
            }
        }

        private void txtTimkiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnTim.PerformClick(); 
            }

        }
    }
}
