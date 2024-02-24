
using QL_Sodo_Phonghop_Hoitruong.Model;
using QL_Sodo_Phonghop_Hoitruong.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QL_Sodo_Phonghop_Hoitruong
{
    /// <summary>
    /// Interaction logic for Chinhsua.xaml
    /// </summary>
    public partial class Chinhsua : Window
    {
        private int currentValue = 0;
        String idghe = ChinhSuaVM.ShareData;
        int idph = Save_Data.Instance.idphienhop;
        public Chinhsua()
        {

            InitializeComponent();
           
            Update(idghe, idph);
            UpdateTextBoxValue();
        }


        private void IncreaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentValue < 100)
            {
                currentValue++;
                UpdateTextBoxValue();
            }
        }

        private void DecreaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentValue > 0)
            {
                currentValue--;
                UpdateTextBoxValue();
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void UpdateTextBoxValue()
        {
            txtdouutien.Text = currentValue.ToString();
        }
        private void Update(String id, int idph)
        {
            try
            {
                var cs = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDGhe == idghe && x.IDPhienHop == idph).SingleOrDefault();
                if (cs != null)
                {
                    txtchucdanh.Text = cs.ChucDanh;
                    txtchucvu.Text = cs.ChucVu;
                    txthoten.Text = cs.HoTen;

                    int douutien;
                    bool parseSuccess = int.TryParse(cs.DoUuTien.ToString(), out douutien);
                    if (parseSuccess)
                    {
                        currentValue = douutien;
                    }
                    else
                    {
                        currentValue = 0;
                    }

                }
            }
            catch { }
            
            

        }
        private void btn_luu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cs = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDGhe == idghe && x.IDPhienHop == idph).SingleOrDefault();
                cs.ChucDanh = txtchucdanh.Text;
                cs.ChucVu = txtchucvu.Text;
                cs.HoTen = txthoten.Text;
                if (txtdouutien.Text != "0")
                {
                    cs.DoUuTien = Convert.ToInt32(txtdouutien.Text);
                }
                else if (Convert.ToInt32(txtdouutien.Text) < 0)
                {
                    MessageBox.Show("Vui lòng kiểm tra lại Độ ưu tiên");
                }
                else
                {
                    cs.DoUuTien = 0;
                }
                //kiem tra thong tin sua
                if (!String.IsNullOrEmpty(txtchucdanh.Text))
                {
                    string text = txtchucdanh.Text;
                    char firstChar = text.Length > 0 ? text[0] : '\0';

                    if (char.IsDigit(firstChar) || char.IsPunctuation(firstChar))
                    {
                        MessageBox.Show("Vui lòng kiểm tra lại Chức danh", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (!String.IsNullOrEmpty(txthoten.Text))
                {
                    string text = txthoten.Text;
                    char firstChar = text.Length > 0 ? text[0] : '\0';

                    if (char.IsDigit(firstChar) || char.IsPunctuation(firstChar))
                    {
                        MessageBox.Show("Vui lòng kiểm tra lại Họ tên", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (!String.IsNullOrEmpty(txtchucvu.Text))
                {
                    string text = txtchucvu.Text;
                    char firstChar = text.Length > 0 ? text[0] : '\0';

                    if (char.IsDigit(firstChar) || char.IsPunctuation(firstChar))
                    {
                        MessageBox.Show("Vui lòng kiểm tra lại Chức vụ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                DataProvider.Ins.DB.SaveChanges();

                /*thêm sự kiện updateGhe cho mainwindow*/
                ((MainWindow)Application.Current.MainWindow).UpdateGhe();

                ChinhSuaVM.ShareData = idghe;
                ChinhSuaVM.tt = 1;
                this.Close();
            }
            catch { }
            
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Buttonclose_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        private void Chinhsua_Closed(object sender, EventArgs e)
        {
            // Gọi phương thức cập nhật giao diện trên form main
            
        }

    }
}
