using OfficeOpenXml;
using QL_Sodo_Phonghop_Hoitruong.Model;
using QL_Sodo_Phonghop_Hoitruong.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
/*using System.Windows.Forms;*/

namespace QL_Sodo_Phonghop_Hoitruong
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        Boolean taosd = false;
        int seatCount = 0;
        int tt = 0;
        Boolean tc = false;


        /*button ghế được click*/
        Button clickedButton;
        Button keobutton;
        Button thabutton;
        public MainWindow()
        {
            InitializeComponent();
        }

        //XỬ LÝ SỰ KIỆN ĐƯỢC GỌI TỪ FORM chinhsua KHI NHẤN NÚT LƯU
        public void UpdateGhe()
        {
            try
            {
                tt = ChinhSuaVM.tt;

                // Cập nhật giao diện trên form main

                string idghe = ChinhSuaVM.ShareData;

                Button bt = (Button)FindName(idghe);

                var thongtinghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDGhe == idghe && x.IDPhienHop == Save_Data.Instance.idphienhop).SingleOrDefault();
                List<TextBlock> textblocks = FindVisualChildren<TextBlock>(clickedButton);
                foreach (TextBlock textblock in textblocks)
                {
                    if (textblock.Name == "txt_hoten" + clickedButton.Name)
                    {
                        textblock.Text = thongtinghe.HoTen;
                    }
                    if (textblock.Name == "txt_chucdanh" + clickedButton.Name)
                    {
                        textblock.Text = thongtinghe.ChucDanh;
                    }
                    if (textblock.Name == "txt_chucvu" + clickedButton.Name)
                    {
                        textblock.Text = thongtinghe.ChucVu;
                    }
                    if (textblock.Name == "txt_uutien" + clickedButton.Name)
                    {
                        if (thongtinghe.DoUuTien == 0)
                        {
                            textblock.Text = "";
                        }
                        else
                        {
                            textblock.Text = thongtinghe.DoUuTien.ToString();
                        }

                    }

                }
            }
            catch
            {

            }
           
        }
        public void Update_ImportExcel()
        {
            if (Save_Data.Instance.hinhthuc == "Phòng họp")
            {
                matrixBorder.Child = CreateMatrixWithBorders(Save_Data.Instance.length, Save_Data.Instance.width, Save_Data.Instance.idphienhop);
            }
            else if (Save_Data.Instance.hinhthuc == "Hội trường")
            {
                CreateMatrix_HT(Save_Data.Instance.soday, Save_Data.Instance.sohangmoiday, Save_Data.Instance.soghemoihang, Save_Data.Instance.idphienhop);
            }
        }


        //THUẬT TOÁN TẠO SƠ ĐỒ PHÒNG HỌP
        private Grid CreateMatrixWithBorders(int rows, int cols, int idphienhop = 1)
        {
            try
            {
                matrixWrapPanel.Children.Clear();
                var grid = new Grid();

                for (int row = 0; row < rows + 2; row++)
                {
                    grid.RowDefinitions.Add(new RowDefinition());
                }

                for (int col = 0; col < cols + 2; col++)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                seatCount = 0;

                for (int row = 0; row < rows + 2; row++)
                {
                    for (int col = 0; col < cols + 2; col++)
                    {
                        if ((row == 0 || row == rows + 1 || col == 0 || col == cols + 1) && !(row == 0 && col == 0) && !(row == 0 && col == cols + 1) && !(row == rows + 1 && col == 0) && !(row == rows + 1 && col == cols + 1))
                        {
                            seatCount++;
                            var button = new Button
                            {
                                Margin = new Thickness(2),
                                Width = 180,
                                Height = 120,
                                HorizontalContentAlignment = HorizontalAlignment.Center, // Căn giữa nội dung theo chiều ngang
                                VerticalContentAlignment = VerticalAlignment.Center, // Căn giữa nội dung theo chiều dọc
                                Name = $"btn_ghe{ GetSeatNumber(rows, cols, row, col) }",
                                BorderThickness = new Thickness(2),
                                BorderBrush = new SolidColorBrush(Colors.Black),
                                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#59101b")),
                                AllowDrop = true, //Kích hoạt thuộc tính cho phép kéo thả
                            };
                            //Sự kiện click chuột phải sửa ghế
                            button.MouseRightButtonUp += mainGrid_MouseRightButtonUp;

                            /* //Sự kiện hover
                             button.MouseEnter += Button_MouseEnter;
                             button.MouseLeave += Button_MouseLeave;*/

                            //sự kiện drag và drop
                            button.PreviewMouseLeftButtonDown += Button_Keo;
                            button.Drop += Button_Tha;

                            string idghe = $"btn_ghe{ GetSeatNumber(rows, cols, row, col) }";
                            int stt = GetSeatNumber(rows, cols, row, col);

                            var spn = new StackPanel()
                            {
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Top
                            };
                            var txt_tenghe = new TextBlock
                            {
                                FontSize = 11,
                                Text = $"Ghế {GetSeatNumber(rows, cols, row, col)}",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                TextWrapping = TextWrapping.Wrap,

                            };
                            string tenghe = $"Ghế {GetSeatNumber(rows, cols, row, col)}";
                            var txt_hoten = new TextBlock
                            {
                                Name = "txt_hoten" + idghe,
                                FontSize = 11,
                                Text = "",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                TextWrapping = TextWrapping.Wrap,
                                TextAlignment = TextAlignment.Center
                            };

                            var txt_chucdanh = new TextBlock
                            {
                                Name = "txt_chucdanh" + idghe,
                                FontSize = 11,
                                Text = "",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                TextAlignment = TextAlignment.Center
                            };
                            var txt_chucvu = new TextBlock
                            {
                                Name = "txt_chucvu" + idghe,
                                FontSize = 11,
                                Text = "",
                                HorizontalAlignment = HorizontalAlignment.Center,
                                TextAlignment = TextAlignment.Center
                            };
                            var txt_uutien = new TextBlock
                            {
                                Name = "txt_uutien" + idghe,
                                Text = "",
                                FontSize = 13,
                                FontWeight = FontWeights.Bold,
                                Foreground = new SolidColorBrush(Colors.Blue),
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Top,
                                Margin = new Thickness(0, -20, 0, 0),
                                TextWrapping = TextWrapping.Wrap,
                            };

                            spn.Children.Add(txt_uutien);
                            spn.Children.Add(txt_tenghe);
                            spn.Children.Add(txt_hoten);
                            spn.Children.Add(txt_chucdanh);
                            spn.Children.Add(txt_chucvu);
                            button.Content = spn;

                            Grid.SetRow(button, row);
                            Grid.SetColumn(button, col);
                            grid.Children.Add(button);

                            /*HIỂN THỊ THÔNG TIN GHẾ LÊN BUTTON*/
                            var ktra_existghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == Save_Data.Instance.idphienhop && x.HoTen != null);
                            if (ktra_existghe.Count() > 0) /*nếu nhấn gridview và phiên họp đó có thông tin ghế*/
                            {

                                var Ghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == Save_Data.Instance.idphienhop && x.IDGhe == idghe).SingleOrDefault();
                                txt_hoten.Text = Ghe.HoTen;
                                txt_chucdanh.Text = Ghe.ChucDanh;
                                txt_chucvu.Text = Ghe.ChucVu;
                                if (Ghe.DoUuTien == 0)
                                {
                                    txt_uutien.Text = "";
                                }
                                else
                                {
                                    txt_uutien.Text = Ghe.DoUuTien.ToString();
                                }
                            }

                            //THEM IDGHE, TENGHE VAO CSDL                 
                            if (taosd == true)
                            {

                                var ktra_ghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == idphienhop && x.IDGhe == idghe);
                                if (ktra_ghe.Count() == 0)
                                {

                                    var thongtinghe = new ThongTinGhe() { STT = stt, IDGhe = idghe, TenGhe = tenghe, IDPhienHop = idphienhop };
                                    DataProvider.Ins.DB.ThongTinGhes.Add(thongtinghe);
                                    DataProvider.Ins.DB.SaveChanges();
                                }
                            }
                        }
                        else if (row == 1 && col == 1)  /* Phần này để tạo bàn*/
                        {
                            var bigButton = new Button
                            {
                                Margin = new Thickness(20),
                                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C6E2FF")),
                                BorderThickness = new Thickness(2),
                                BorderBrush = System.Windows.Media.Brushes.DarkGray,
                                Width = double.NaN,
                                Height = double.NaN,
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center,

                            };

                            // Đoạn mã trên tạo một đối tượng Button mới và gán các thuộc tính cụ thể cho nó.

                            Grid.SetRow(bigButton, row);
                            Grid.SetColumn(bigButton, col);
                            Grid.SetRowSpan(bigButton, rows);
                            Grid.SetColumnSpan(bigButton, cols);
                            grid.Children.Add(bigButton);
                        }
                    }
                }

                return grid;
            }
            catch
            {
                return null;
            }
           
        }
        //
        //THUẬT TOÁN TẠO SƠ ĐỒ HỘI TRƯỜNG
        private void CreateMatrix_HT(int soday, int sohangmoiday, string soghemoihang, int idphienhop = 0)
        {
            try
            {
                int totalSeats = 1; // Biến đếm từ ghế 1

                matrixWrapPanel.Children.Clear();
                matrixBorder.Child = null;

                string[] widthValues = soghemoihang.Split(',');
                for (int m = 0; m < soday; m++)
                {
                    if (int.TryParse(widthValues[m], out int width))
                    {
                        var grid = new UniformGrid();
                        grid.Columns = width;

                        for (int row = 0; row < sohangmoiday; row++)
                        {
                            for (int col = 0; col < width; col++)
                            {
                                var button = new Button
                                {
                                    Margin = new Thickness(2),
                                    /* Width = 170,
                                     Height = 120,*/
                                    Width = 150,
                                    Height = 95,
                                    HorizontalContentAlignment = HorizontalAlignment.Center, // Căn giữa nội dung theo chiều ngang
                                    VerticalContentAlignment = VerticalAlignment.Center, // Căn giữa nội dung theo chiều dọc
                                    Name = "btn_ghe" + totalSeats,
                                    BorderThickness = new Thickness(2),
                                    BorderBrush = new SolidColorBrush(Colors.Black),
                                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#59101b")),
                                    AllowDrop = true, //Kích hoạt thuộc tính cho phép kéo thả

                                };
                                grid.Children.Add(button);


                                //Sự kiện click chuột phải sửa ghế
                                button.MouseRightButtonUp += mainGrid_MouseRightButtonUp;
                                /*
                                                            //Sự kiện hover
                                                            button.MouseEnter += Button_MouseEnter;
                                                            button.MouseLeave += Button_MouseLeave;*/

                                //sự kiện drag và drop
                                button.PreviewMouseLeftButtonDown += Button_Keo;
                                button.Drop += Button_Tha;

                                string idghe = "btn_ghe" + totalSeats;
                                int stt = totalSeats;

                                var spn = new StackPanel()
                                {
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Top
                                };
                                var txt_tenghe = new TextBlock
                                {
                                    FontSize = 10,
                                    Text = "Ghế " + totalSeats,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    TextWrapping = TextWrapping.Wrap,

                                };
                                string tenghe = "Ghế " + totalSeats;
                                var txt_hoten = new TextBlock
                                {
                                    Name = "txt_hoten" + idghe,
                                    FontSize = 10,
                                    Text = "",
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    TextWrapping = TextWrapping.Wrap,
                                    TextAlignment = TextAlignment.Center
                                };

                                var txt_chucdanh = new TextBlock
                                {
                                    Name = "txt_chucdanh" + idghe,
                                    FontSize = 10,
                                    Text = "",
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    TextAlignment = TextAlignment.Center
                                };
                                var txt_chucvu = new TextBlock
                                {
                                    Name = "txt_chucvu" + idghe,
                                    FontSize = 10,
                                    Text = "",
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    TextAlignment = TextAlignment.Center
                                };
                                var txt_uutien = new TextBlock
                                {
                                    Name = "txt_uutien" + idghe,
                                    Text = "",
                                    FontSize = 12,
                                    FontWeight = FontWeights.Bold,
                                    Foreground = new SolidColorBrush(Colors.Blue),
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Top,
                                    Margin = new Thickness(0, -20, 0, 0),
                                    TextWrapping = TextWrapping.Wrap,
                                };

                                totalSeats++; // Tăng biến đếm sau mỗi nút được thêm
                                spn.Children.Add(txt_uutien);
                                spn.Children.Add(txt_tenghe);
                                spn.Children.Add(txt_hoten);
                                spn.Children.Add(txt_chucdanh);
                                spn.Children.Add(txt_chucvu);
                                button.Content = spn;

                                /*HIỂN THỊ THÔNG TIN GHẾ LÊN BUTTON*/
                                var ktra_existghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == Save_Data.Instance.idphienhop && (x.HoTen != null || x.ChucDanh != null || x.ChucVu != null || x.DoUuTien != null));
                                if (ktra_existghe.Count() > 0) /*nếu nhấn gridview và phiên họp đó có thông tin ghế*/
                                {
                                    var Ghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == Save_Data.Instance.idphienhop && x.IDGhe == idghe).SingleOrDefault();
                                    txt_hoten.Text = Ghe.HoTen != null ? Ghe.HoTen : null;
                                    txt_chucdanh.Text = Ghe.ChucDanh != null ? Ghe.ChucDanh : null;
                                    txt_chucvu.Text = Ghe.ChucVu != null ? Ghe.ChucVu : null;

                                    if (Ghe.DoUuTien == 0)
                                    {
                                        txt_uutien.Text = "";
                                    }
                                    else
                                    {
                                        txt_uutien.Text = Ghe.DoUuTien.ToString();
                                    }
                                }

                                //THEM IDGHE, TENGHE VAO CSDL                 
                                if (taosd == true)
                                {
                                    var ktra_ghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == idphienhop && x.IDGhe == idghe);
                                    if (ktra_ghe.Count() == 0)
                                    {

                                        var thongtinghe = new ThongTinGhe() { STT = stt, IDGhe = idghe, TenGhe = tenghe, IDPhienHop = idphienhop };
                                        DataProvider.Ins.DB.ThongTinGhes.Add(thongtinghe);
                                        DataProvider.Ins.DB.SaveChanges();
                                    }
                                }
                            }
                        }
                        // Add margin to the grid to create spacing between matrices
                        grid.Margin = new Thickness(20, 30, 20, 0);
                        matrixWrapPanel.Children.Add(grid);
                    }
                }
            }
            catch
            {

            }
            

        }

        private int GetSeatNumber(int rows, int cols, int row, int col) /*Đánh số ghế*/
        {
            if (row == 0) return col;
            if (col == cols + 1) return cols + row;
            if (row == rows + 1) return cols + rows + cols - col + 1;
            if (col == 0) return cols + rows + cols + rows - row + 1;
            return -1;
        }


        //TẠO PHIÊN HỌP
        private void ButtonAddMeeting_Click(object sender, RoutedEventArgs e)
        {
            if (Save_Data.Instance.hinhthuc == "Phòng họp")
            {
                if (int.TryParse(txtLength.Text, out int length) && int.TryParse(txtWidth.Text, out int width))
                {
                    btn_taosd.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập số hợp lệ cho chiều dài và chiều rộng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else if (Save_Data.Instance.hinhthuc == "Hội trường")
            {
                /* Boolean ktra= KTra_KichThuoc_SDHT(txtSoDay.Text, txtbSoHangMoiDay.Text, txtSoGheMoiHang.Text);*/
                taosd = btn_taosd.IsEnabled = true;
            }
        }

        //TẠO SƠ ĐỒ
        private void btn_taosd_Click(object sender, RoutedEventArgs e)
        {
            if (taosd == true)
            {

                if (Save_Data.Instance.hinhthuc.Equals("Phòng họp"))
                {
                    matrixBorder.Child = CreateMatrixWithBorders(Save_Data.Instance.length, Save_Data.Instance.width, Save_Data.Instance.idphienhop);
                }
                else if (Save_Data.Instance.hinhthuc.Equals("Hội trường"))
                {
                    CreateMatrix_HT(Save_Data.Instance.soday, Save_Data.Instance.sohangmoiday, Save_Data.Instance.soghemoihang, Save_Data.Instance.idphienhop);
                }
            }
            btn_taosd.IsEnabled = taosd = false;
        }

        //XOÁ SƠ ĐỒ
        public void XoaSoDo()
        {
            matrixBorder.Child = null;
            matrixWrapPanel.Children.Clear();
        }

        //SỰ KIỆN KÉO BUTTON
        private void Button_Keo(object sender, MouseButtonEventArgs e)
        {
            try
            {
                keobutton = (Button)sender;
                string idghe = keobutton.Name;

                if (idghe != null)
                {
                    DragDrop.DoDragDrop(keobutton, idghe, DragDropEffects.Move);
                }
            }
            catch
            {}
            
        }

        //SỰ KIỆN THẢ BUTTON
        private void Button_Tha(object sender, DragEventArgs e)
        {
            try
            {
                string dropdata = e.Data.GetData(typeof(string)) as string;
                thabutton = (Button)sender;
                string ghe = thabutton.Name;
                if (dropdata != ghe)
                {
                    MessageBoxResult result = MessageBox.Show("Bạn chắc chắn muốn đổi vị trí 2 ghế ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (dropdata != null)
                        {
                            //Lấy thông tin của ghế kéo(drag) và lưu vào biến phụ 0
                            var data = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == Save_Data.Instance.idphienhop && x.IDGhe == ghe).SingleOrDefault();

                            string HoTen = data.HoTen;
                            string ChucDanh = data.ChucDanh;
                            string ChucVu = data.ChucVu;
                            string Douutien = data.DoUuTien.ToString();

                            //Lấy thông tin của ghế thả(drop) và lưu vào biến phụ 1
                            var data1 = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == Save_Data.Instance.idphienhop && x.IDGhe == dropdata).SingleOrDefault();

                            string HoTen1 = data1.HoTen;
                            string ChucDanh1 = data1.ChucDanh;
                            string ChucVu1 = data1.ChucVu;
                            string Douutien1 = data1.DoUuTien.ToString();

                            //Hoán đổi dữ liệu của 2 button
                            data.HoTen = HoTen1;
                            data.ChucDanh = ChucDanh1;
                            data.ChucVu = ChucVu1;
                            try
                            {
                                data.DoUuTien = Convert.ToInt32(Douutien1);
                            }
                            catch (Exception ex)
                            {
                                data.DoUuTien = 0;
                            }


                            data1.HoTen = HoTen;
                            data1.ChucDanh = ChucDanh;
                            data1.ChucVu = ChucVu;
                            try
                            {
                                data1.DoUuTien = Convert.ToInt32(Douutien);
                            }
                            catch (Exception ex)
                            {
                                data1.DoUuTien = 0;
                            }

                            DataProvider.Ins.DB.SaveChanges();

                            //Cập nhật thông tin button hiển thị trên ghế thả
                            var thongtinghe1 = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDGhe == ghe && x.IDPhienHop == Save_Data.Instance.idphienhop).SingleOrDefault();
                            List<TextBlock> textblocks1 = FindVisualChildren<TextBlock>(thabutton);
                            foreach (TextBlock textblock in textblocks1)
                            {
                                if (textblock.Name == "txt_hoten" + thabutton.Name)
                                {
                                    textblock.Text = thongtinghe1.HoTen;
                                }
                                if (textblock.Name == "txt_chucdanh" + thabutton.Name)
                                {
                                    textblock.Text = thongtinghe1.ChucDanh;
                                }
                                if (textblock.Name == "txt_chucvu" + thabutton.Name)
                                {
                                    textblock.Text = thongtinghe1.ChucVu;
                                }
                                if (textblock.Name == "txt_uutien" + thabutton.Name)
                                {
                                    if (thongtinghe1.DoUuTien == 0)
                                    {
                                        textblock.Text = "";
                                    }
                                    else
                                    {
                                        textblock.Text = thongtinghe1.DoUuTien.ToString();
                                    }
                                }
                            }
                            //Cập nhật thông tin button hiển thị trên ghế kéo
                            var thongtinghe2 = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDGhe == dropdata && x.IDPhienHop == Save_Data.Instance.idphienhop).SingleOrDefault();
                            List<TextBlock> textblocks2 = FindVisualChildren<TextBlock>(keobutton);
                            foreach (TextBlock textblock in textblocks2)
                            {
                                if (textblock.Name == "txt_hoten" + keobutton.Name)
                                {
                                    textblock.Text = thongtinghe2.HoTen;
                                }
                                if (textblock.Name == "txt_chucdanh" + keobutton.Name)
                                {
                                    textblock.Text = thongtinghe2.ChucDanh;
                                }
                                if (textblock.Name == "txt_chucvu" + keobutton.Name)
                                {
                                    textblock.Text = thongtinghe2.ChucVu;
                                }
                                if (textblock.Name == "txt_uutien" + keobutton.Name)
                                {
                                    if (thongtinghe2.DoUuTien == 0)
                                    {
                                        textblock.Text = "";
                                    }
                                    else
                                    {
                                        textblock.Text = thongtinghe2.DoUuTien.ToString();
                                    }
                                }

                            }
                        }

                    }

                }
            }
            catch
            {}
           
        }

        /*Tạo sự kiện phóng to button khi hover chuột qua*/
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {

            Button button = (Button)sender;
            button.Width += 30;
            button.Height += 25;
            List<TextBlock> textblocks = FindVisualChildren<TextBlock>(button);
            foreach (TextBlock textblock in textblocks)
            {
                // Thực hiện các thao tác với TextBox ở đây
                textblock.FontSize += 3;
            }
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Width -= 30;
            button.Height -= 25;
            List<TextBlock> textblocks = FindVisualChildren<TextBlock>(button);
            foreach (TextBlock textblock in textblocks)
            {
                // Thực hiện các thao tác với TextBox ở đây
                textblock.FontSize -= 3;
            }
        }


        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ToggleMaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var thongbao = MessageBox.Show("Bạn có muốn thoát ứng dụng?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (thongbao == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else { }
        }

        private void btn_slideshow_Click(object sender, RoutedEventArgs e)
        {
            
            if (matrixBorder.Child == null && matrixWrapPanel.Children.Count == 0)
            {
                MessageBox.Show("Không tìm thấy sơ đồ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                tc = true;

                //ĐÓNG CÁC CONTROL XUNG QUANH
                this.WindowState = WindowState.Maximized; // Đặt cửa sổ vào chế độ fullscreen
                contentText.SetValue(Grid.RowProperty, 0); // Di chuyển phần nội dung Grid lên hàng thứ nhất
                contentText.Margin = new Thickness(0); // Loại bỏ lề
                congcu.Visibility = Visibility.Collapsed;
                an.Visibility = Visibility.Collapsed;
                // Vô hiệu hóa sự tương tác cho Border
                matrixBorder.IsHitTestVisible = false;
                // Vô hiệu hóa sự tương tác cho WrapPanel
                matrixWrapPanel.IsHitTestVisible = false;

                //CHUẨN BỊ TRÌNH CHIẾU
                System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;

                if (screens.Length > 1)
                {
                    // Lấy kích thước màn hình
                    double screenWidth = SystemParameters.PrimaryScreenWidth;
                    double screenHeight = SystemParameters.PrimaryScreenHeight;

                    // Lấy kích thước và tỷ lệ của nội dung của bạn (ví dụ: một Grid có tên là myGrid)
                    double contentWidth = contentText.ActualWidth; // Kích thước thực tế của nội dung
                    double contentHeight = contentText.ActualHeight;

                    // Tính toán tỷ lệ thu nhỏ
                    double widthScale = screenWidth / contentWidth;
                    double heightScale = screenHeight / contentHeight;

                    // Chọn tỷ lệ nhỏ hơn để đảm bảo nội dung vừa màn hình
                    double scale = Math.Min(widthScale, heightScale);

                    // Áp dụng tỷ lệ cho nội dung
                    contentText.LayoutTransform = new ScaleTransform(scale, scale);

                    System.Windows.Forms.Screen secondaryScreen = screens[1];

                    // Tạo RenderTargetBitmap để chụp nội dung của màn hình chính
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                       (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight,
                      96, 96, PixelFormats.Pbgra32);
                    renderTargetBitmap.Render(Application.Current.MainWindow);


                    //*// Set the window position to the secondary screen
                    this.WindowStartupLocation = WindowStartupLocation.Manual;
                    this.Left = secondaryScreen.WorkingArea.Left;
                    this.Top = secondaryScreen.WorkingArea.Top;

                    // Set the window size to fit the secondary screen
                    this.Width = secondaryScreen.WorkingArea.Width;
                    this.Height = secondaryScreen.WorkingArea.Height;

                    var secondWindow = new Window();


                    // Đặt cửa sổ không có thanh tiêu đề và viền
                    secondWindow.WindowStyle = WindowStyle.None;
                    secondWindow.ResizeMode = ResizeMode.NoResize;

                    // Đặt cửa sổ có thể trong suốt
                    secondWindow.AllowsTransparency = true;
                    secondWindow.Background = Brushes.Transparent;

                    secondWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                    secondWindow.Left = secondaryScreen.WorkingArea.Left - 515;
                    secondWindow.Top = secondaryScreen.WorkingArea.Top;

                    // Đặt kích thước của cửa sổ bằng kích thước của màn hình thứ hai
                    secondWindow.Width = secondaryScreen.WorkingArea.Width - 45;
                    secondWindow.Height = secondaryScreen.WorkingArea.Height - 130;


                    // Hiển thị cửa sổ trên màn hình thứ hai
                    Image image = new Image();
                    image.Source = renderTargetBitmap;

                    //secondWindow.MouseLeftButtonUp += mainGrid_MouseRightButtonUp;

                    secondWindow.Content = image;
                    // Gắn sự kiện KeyUp vào secondWindow
                   
                    secondWindow.KeyUp += (sender1, ee) =>
                    {
                        if (ee.Key == Key.Escape)
                        {
                            // Đặt màn hình ứng dụng về trạng thái ban đầu (màn hình 1)
                            this.WindowState = WindowState.Normal; // Chuyển cửa sổ về trạng thái bình thường nếu nó đang ở trạng thái tối đa hóa.

                            // Thu nhỏ cửa sổ bằng cách đặt Width và Height
                            this.Width = this.Width - 200; // Điều chỉnh chiều rộng
                            this.Height = this.Height - 100; // Điều chỉnh chiều cao

                            this.Left = this.Left - 1800;

                            contentText.SetValue(Grid.RowProperty, 2);
                            contentText.Margin = new Thickness(30, 0, 20, 30);
                            congcu.Visibility = Visibility.Visible;
                            an.Visibility = Visibility.Visible;
                            matrixBorder.IsHitTestVisible = true;
                            matrixWrapPanel.IsHitTestVisible = true;
                            secondWindow.Close();
                        }
                    };
                    secondWindow.Show();

                }
                else
                {
                    MessageBox.Show("Không tìm thấy màn hình thứ 2", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        //SỰ KIỆN NHẤN CHUỘT PHẢI
        private void mainGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
               
                base.OnMouseRightButtonUp(e);

                // Hiển thị menu chọn
                ContextMenu contextMenu = new ContextMenu();

                // Thêm một MenuItem cho button thoát và ngừng trình chiếu
                MenuItem menuItemExit = new MenuItem();
                MenuItem menusua = new MenuItem();
                menuItemExit.Header = "Thoát và ngừng trình chiếu";
                menuItemExit.Click += MenuItemExit_Click;
                menusua.Header = "Sửa ghế";
                menusua.Click += Button_SuaTT_Click;

                //nếu button copy được nhấn
                if (tc == true)
                {
                    contextMenu.Items.Add(menuItemExit);
                    
                }
                //nếu chuột phải là 1 button
                else if (sender.ToString() == "System.Windows.Controls.Button")
                {
                    clickedButton = (Button)sender;
                    string ghe = clickedButton.Name;
                    ChinhSuaVM.ShareData2 = ghe;
                    contextMenu.Items.Add(menusua);
                }

                // Hiển thị menu chọn tại vị trí chuột phải
                contextMenu.IsOpen = true;
                e.Handled = true;
            }
            catch { }
            
        }


        //THOÁT TRÌNH CHIẾU BẰNG PHÍM ESC
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                // Đưa form trở lại trạng thái ban đầu
                this.WindowState = WindowState.Normal;
                contentText.SetValue(Grid.RowProperty, 2);
                contentText.Margin = new Thickness(30, 0, 20, 30);
                congcu.Visibility = Visibility.Visible;
                an.Visibility = Visibility.Visible;
                // Vô hiệu hóa sự tương tác cho Border
                matrixBorder.IsHitTestVisible = true;
                // Vô hiệu hóa sự tương tác cho WrapPanel
                matrixWrapPanel.IsHitTestVisible = true;

                tc = false;
            }
        }

        //THOÁT TRÌNH CHIẾU BẰNG CHUỘT PHẢI CHỌN THOÁT
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            // Đưa form trở lại trạng thái ban đầu
            this.WindowState = WindowState.Normal;
            contentText.SetValue(Grid.RowProperty, 2);
            contentText.Margin = new Thickness(30, 0, 20, 30);
            congcu.Visibility = Visibility.Visible;
            an.Visibility = Visibility.Visible;
            // Vô hiệu hóa sự tương tác cho Border
            matrixBorder.IsHitTestVisible = true;
            // Vô hiệu hóa sự tương tác cho WrapPanel
            matrixWrapPanel.IsHitTestVisible = true;

            tc = false;
        }



        //double click để SỬA THÔNG TIN GHẾ
        private void Button_SuaTT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string buttonID = ChinhSuaVM.ShareData2;
                ChinhSuaVM.ShareData = buttonID;
                Chinhsua chinhsua = new Chinhsua();
                chinhsua.ShowDialog();
            }
            catch { }
            

        }

        /*Thay đổi hint của 2 textbox khi thay đổi hình thức*/
        private void cbb_hinhthuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbb_hinhthuc.SelectedIndex == 0)
            {
                txtLength.Visibility = Visibility.Visible;
                txtWidth.Visibility = Visibility.Visible;

                txtbSoDay.Visibility = Visibility.Collapsed;
                txtbSoHangMoiDay.Visibility = Visibility.Collapsed;
                txtbSoGheMoiHang.Visibility = Visibility.Collapsed;

                txtSoDay.Visibility = Visibility.Collapsed;
                txtSoHangMoiDay.Visibility = Visibility.Collapsed;
                txtSoGheMoiHang.Visibility = Visibility.Collapsed;

                MaterialDesignThemes.Wpf.HintAssist.SetHint(txtLength, "Số ghế dọc");
                MaterialDesignThemes.Wpf.HintAssist.SetHint(txtWidth, "Số ghế ngang");
            }
            else if (cbb_hinhthuc.SelectedIndex == 1)
            {
                txtLength.Visibility = Visibility.Collapsed;
                txtWidth.Visibility = Visibility.Collapsed;

                //hiện tiêu đề
                txtbSoDay.Visibility = Visibility.Visible;
                txtbSoHangMoiDay.Visibility = Visibility.Visible;
                txtbSoGheMoiHang.Visibility = Visibility.Visible;

                //hiện textbox
                txtSoDay.Visibility = Visibility.Visible;
                txtSoHangMoiDay.Visibility = Visibility.Visible;
                txtSoGheMoiHang.Visibility = Visibility.Visible;


            }
        }


        /*SỰ KIỆN CLICK VÀO 1 HÀNG CỦA GRIDVIEW*/
        private void listViewMeetings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int length;
                int width;
                int IDPhienHop;
                PhienHop ph = (PhienHop)lv_phienhop.SelectedItem;

                if (ph != null)
                {
                    //thay đổi cbb hình thức
                    cbb_hinhthuc.Text = ph.HinhThuc;

                    //click vào lưu idphienhop và hình thức phiên họp
                    Save_Data.Instance.idphienhop = ph.IDPhienHop;
                    Save_Data.Instance.hinhthuc = ph.HinhThuc;

                    //kiểm tra phòng hợp đã có sơ đồ chưa
                    var ktra_idphtao = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == ph.IDPhienHop);

                    //Nếu click là phòng họp
                    if (ph.HinhThuc == "Phòng họp")
                    {
                        //lưu thông tin kích thước của phiên hợp được click
                        Save_Data.Instance.length = ph.ChieuDoc.Value;
                        Save_Data.Instance.width = ph.ChieuNgang.Value;

                        if (ktra_idphtao.Count() > 0)
                        {
                            IDPhienHop = ph.IDPhienHop; /*id của phiên hợp được nhấn*/
                            length = Convert.ToInt32(ph.ChieuDoc);
                            width = Convert.ToInt32(ph.ChieuNgang);

                            /*Gọi hàm tạo sơ đồ thì sơ đồ sẽ được hiển thị tương ứng với phiên họp*/
                            matrixBorder.Child = CreateMatrixWithBorders(length, width, IDPhienHop);
                        }
                    }
                    //Nếu click là Hội trường
                    else
                    {
                        //lưu thông tin kích thước của phiên hợp được click
                        Save_Data.Instance.soday = ph.SoDay.Value;
                        Save_Data.Instance.sohangmoiday = ph.SoHangMoiDay.Value;
                        Save_Data.Instance.soghemoihang = ph.SoGheMoiHang;

                        if (ktra_idphtao.Count() > 0)
                        {
                            CreateMatrix_HT(ph.SoDay.Value, ph.SoHangMoiDay.Value, ph.SoGheMoiHang, Save_Data.Instance.idphienhop);
                        }

                    }

                    if (ktra_idphtao.Count() == 0)
                    {
                        btn_taosd.IsEnabled = taosd = true;
                        matrixBorder.Child = null;
                        matrixWrapPanel.Children.Clear();
                    }
                    else
                    {
                        btn_taosd.IsEnabled = taosd = false;
                    }
                }
            }
            catch
            {}
            
        }
        //EXPORT EXCEL
        private void btn_taoexcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (matrixBorder.Child == null && matrixWrapPanel.Children.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sơ đồ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    XuatmauExcel xuatmau = new XuatmauExcel();
                    //Hiển thị các thông tin ghế lên datagrid
                    var data_ghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == Save_Data.Instance.idphienhop).Select(x => new
                    {
                        x.STT,
                        x.IDGhe,
                        x.TenGhe,
                        x.HoTen,
                        x.ChucDanh,
                        x.ChucVu,
                        x.DoUuTien
                    }).ToList();

                    //lưu csdl vào griddata
                    xuatmau.dtg_excel.ItemsSource = data_ghe;

                    //sắp xếp các ghế theo stt từ nhỏ đến lớn
                    xuatmau.dtg_excel.Items.SortDescriptions.Clear();
                    xuatmau.dtg_excel.Items.SortDescriptions.Add(new SortDescription("STT", ListSortDirection.Ascending));

                    xuatmau.ShowDialog();
                }
            }
            catch { }
            

        }

        //IMPORT EXCEL
        private void btn_nhapexcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (matrixBorder.Child == null && matrixWrapPanel.Children.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sơ đồ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        string filePath = openFileDialog.FileName;

                        using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(filePath)))
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                            DataTable dt = new DataTable();

                            foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                            {
                                dt.Columns.Add(firstRowCell.Text);
                            }

                            for (int rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                            {
                                var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                                var newRow = dt.Rows.Add();
                                foreach (var cell in row)
                                {
                                    newRow[cell.Start.Column - 1] = cell.Text;
                                }
                            }

                            // Chuyển dữ liệu sang trang Nhapexcel
                            NhapExcel nhapmau = new NhapExcel();

                            // Gán đường dẫn vào TextBlock trên trang Nhapexcel
                            nhapmau.txtFilePath.Text = filePath;
                            // Gán DataTable vào DataGrid trên trang NhapExcel
                            nhapmau.dtg_okexcel.ItemsSource = dt.DefaultView;

                            nhapmau.ShowDialog();
                        }
                    }
                }
            }
            catch { }
            

        }

        //xây dựng phương thức xác định children của object
        private List<T> FindVisualChildren<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            List<T> children = new List<T>();

            // Sử dụng hàm đệ quy để tìm kiếm các UIElement con
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);

                if (child is T typedChild)
                {
                    children.Add(typedChild);
                }

                List<T> childChildren = FindVisualChildren<T>(child);
                children.AddRange(childChildren);
            }

            return children;
        }

        //QUY ĐỊNH CHỈ NHẬP SỐ CHO CÁC TEXTBOX
        private void txtLength_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtWidth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtSoDay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtSoHangMoiDay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


    }

}

