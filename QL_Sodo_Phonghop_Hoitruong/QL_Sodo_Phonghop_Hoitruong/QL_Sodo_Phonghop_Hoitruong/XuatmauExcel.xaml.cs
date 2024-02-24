using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using QL_Sodo_Phonghop_Hoitruong.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.IO;
using System.IO.Packaging;
using QL_Sodo_Phonghop_Hoitruong.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace QL_Sodo_Phonghop_Hoitruong
{
    /// <summary>
    /// Interaction logic for XuatmauExcel.xaml
    /// </summary>
    public partial class XuatmauExcel : Window
    {


        public XuatmauExcel()
        {
            InitializeComponent();
        }


        private void btn_dongexcel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btn_OKexcel_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_huyexcel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void btn_xuatexcel_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "";
            // tạo SaveFileDialog để lưu file excel
            SaveFileDialog dialog = new SaveFileDialog();

            // chỉ lọc ra các file có định dạng Excel
            dialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

            // Nếu mở file và chọn nơi lưu file thành công sẽ lưu đường dẫn lại dùng
            if (dialog.ShowDialog() == true)
            {
                filePath = dialog.FileName;
            }
            // nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Đường dẫn file không hợp lệ");
                return;
            }

            try
            {
                using (ExcelPackage p = new ExcelPackage())
                {

                    p.Workbook.Worksheets.Add("export sheet");

                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets[0];

                    // đặt tên cho sheet
                    ws.Name = "export chairs";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 12;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Times New Roman";

                    // Tạo danh sách các column header
                    string[] arrColumnHeader = {
                                                "STT",
                                                "ID ghế",
                                                "Tên ghế",
                                                "Họ tên",
                                                "Chức danh",
                                                "Chức vụ",
                                                "Độ ưu tiên"
                                            };

                    // lấy ra số lượng cột cần dùng dựa vào số lượng header
                    var countColHeader = arrColumnHeader.Count();

                    int colIndex = 1;
                    int rowIndex = 2;

                    // tạo các header từ column header đã tạo từ bên trên
                    foreach (var item in arrColumnHeader)
                    {
                        var cell = ws.Cells[rowIndex - 1, colIndex];

                        // set màu thành nâu nhạt
                        var fill = cell.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(System.Drawing.Color.Wheat);

                        // Căn giữa header
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        // căn chỉnh các border
                        var border = cell.Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        // Thêm bộ lọc trên header
                        var range = ws.Cells[1, 1, 1, countColHeader];
                        range.AutoFilter = true;

                        // in đậm header
                        var font = cell.Style.Font;
                        font.Bold = true;
                        // gán giá trị
                        cell.Value = item;

                        // điều chỉnh chiều rộng của column
                        ws.Column(colIndex).Width = 30;

                        colIndex++;
                    }
                    // với mỗi item trong danh sách sẽ ghi trên 1 dòng
                    foreach (var item in dtg_excel.Items)
                    {

                        int? stt = item.GetType().GetProperty("STT")?.GetValue(item, null) as int?;
                        string idGhe = item.GetType().GetProperty("IDGhe")?.GetValue(item, null) as string;
                        string tenGhe = item.GetType().GetProperty("TenGhe")?.GetValue(item, null) as string;
                        string hoTen = item.GetType().GetProperty("HoTen")?.GetValue(item, null) as string;
                        string chucDanh = item.GetType().GetProperty("ChucDanh")?.GetValue(item, null) as string;
                        string chucVu = item.GetType().GetProperty("ChucVu")?.GetValue(item, null) as string;
                        int? doUuTien = item.GetType().GetProperty("DoUuTien")?.GetValue(item, null) as int?;

                        // gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, 1].Value = stt;
                        ws.Cells[rowIndex, 2].Value = idGhe;
                        ws.Cells[rowIndex, 3].Value = tenGhe;
                        ws.Cells[rowIndex, 4].Value = hoTen;
                        ws.Cells[rowIndex, 5].Value = chucDanh;
                        ws.Cells[rowIndex, 6].Value = chucVu;
                        ws.Cells[rowIndex, 7].Value = doUuTien;

                        // căn giữa các ô dữ liệu
                        ws.Cells[rowIndex, 1, rowIndex, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        rowIndex++;
                    }

                    // Lưu file lại
                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                    txtFilePath.Text = filePath;
                }
                MessageBox.Show("Xuất excel thành công!");
            }
            catch (Exception EE)
            {
                MessageBox.Show("Xuất file không thành công");
            }
        }
    }

}





