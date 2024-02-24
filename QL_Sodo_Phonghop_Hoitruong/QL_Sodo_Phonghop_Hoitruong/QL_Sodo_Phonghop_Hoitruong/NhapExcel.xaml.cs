using Microsoft.Win32;
using System;
using OfficeOpenXml;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QL_Sodo_Phonghop_Hoitruong.Model;
using QL_Sodo_Phonghop_Hoitruong.ViewModel;

namespace QL_Sodo_Phonghop_Hoitruong
{
    /// <summary>
    /// Interaction logic for NhapExcel.xaml
    /// </summary>
    public partial class NhapExcel : Window
    {


        /*
                public NhapExcel(DataView defaultView)
                {
                    this.defaultView = defaultView;
                }*/

        public NhapExcel()
        {
            InitializeComponent();
        }
        private void btn_huyexcel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Đóng cửa sổ hiện tại
        }

        private void btn_OKexcel_Click(object sender, RoutedEventArgs e)
        {
            try {
                int idghe_so = 1;
                int sohang = dtg_okexcel.Items.Count;
                int soghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == Save_Data.Instance.idphienhop).Count();
                if (sohang == soghe)
                {
                    foreach (var item in dtg_okexcel.Items)
                    {

                        var ghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == Save_Data.Instance.idphienhop && x.IDGhe == "btn_ghe" + idghe_so.ToString()).SingleOrDefault();

                        DataRowView row = (DataRowView)item;

                        string hoTen = row["Họ tên"] != null ? row["Họ tên"].ToString() : null;

                        if (!string.IsNullOrEmpty(hoTen))
                        {
                            // Kiểm tra kí tự đầu của Họ tên nếu bắt đầu là số hoặc kí tự đặc biệt thì báo lỗi
                            char firstChar = hoTen[0];

                            if (char.IsDigit(firstChar) || char.IsPunctuation(firstChar))
                            {
                                MessageBox.Show("Vui lòng kiểm tra lại Họ tên", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                        string chucDanh = row["Chức danh"] != null ? row["Chức danh"].ToString() : null;

                        if (!string.IsNullOrEmpty(chucDanh))
                        {
                            // Kiểm tra kí tự đầu của Chức danh nếu bắt đầu là số hoặc kí tự đặc biệt thì báo lỗi
                            char firstChar = chucDanh[0];

                            if (char.IsDigit(firstChar) || char.IsPunctuation(firstChar))
                            {
                                MessageBox.Show("Vui lòng kiểm tra lại Chức danh", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                        string chucVu = row["Chức vụ"] != null ? row["Chức vụ"].ToString() : null;

                        if (!string.IsNullOrEmpty(chucVu))
                        {
                            // Kiểm tra kí tự đầu của Chức vụ nếu bắt đầu là số hoặc kí tự đặc biệt thì báo lỗi
                            char firstChar = chucVu[0];

                            if (char.IsDigit(firstChar) || char.IsPunctuation(firstChar))
                            {
                                MessageBox.Show("Vui lòng kiểm tra lại Chức vụ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                        ghe.HoTen = hoTen;
                        ghe.ChucVu = chucDanh;
                        ghe.ChucDanh = chucVu;

                        int doUuTien;
                        bool parseSuccess = int.TryParse(row["Độ ưu tiên"].ToString(), out doUuTien);
                        if (parseSuccess)
                        {
                            ghe.DoUuTien = doUuTien;
                        }
                        else
                        {
                            ghe.DoUuTien = 0;
                        }
                        idghe_so++;
                    }
                    DataProvider.Ins.DB.SaveChanges(); /*dữ liệu được lưu vào cơ sở dữ liệu thông qua phương thức SaveChanges*/
                    MessageBox.Show("Import thành công");
                    ((MainWindow)Application.Current.MainWindow).Update_ImportExcel(); /*cập nhật giao diện.*/
                }
                else
                {
                    MessageBox.Show("Số hàng không phù hợp với số ghế", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

                }

                this.Close();
            }
            catch { }
            
        }
    }
}
