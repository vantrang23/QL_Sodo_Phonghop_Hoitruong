using QL_Sodo_Phonghop_Hoitruong.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace QL_Sodo_Phonghop_Hoitruong.ViewModel
{
    public class PhienHopVM : BaseViewModel
    {

        /* tao bien va phuong thuc de luu danh sach phien hop  */
        private ObservableCollection<PhienHop> _List;
        public ObservableCollection<PhienHop> List { get => _List; set { _List = value; OnPropertyChanged(); } }


        /* tao cac bien giu SelectedItem khi nhan vao cac hang trong gridview */
        private string _TenPhienHop;
        public string TenPhienHop { get => _TenPhienHop; set { _TenPhienHop = value; OnPropertyChanged(); } }

        private string _Mota;
        public string Mota { get => _Mota; set { _Mota = value; OnPropertyChanged(); } }

        private string _HinhThuc;
        public string HinhThuc { get => _HinhThuc; set { _HinhThuc = value; OnPropertyChanged(); } }

        private string _HinhThucDisplay;
        public string HinhThucDisplay { get => _HinhThucDisplay; set { _HinhThucDisplay = value; OnPropertyChanged(); } }

        private DateTime _NgayBatDau;
        public DateTime NgayBatDau { get => _NgayBatDau; set { _NgayBatDau = value; OnPropertyChanged(); } }

        private DateTime _NgayKetThuc;
        public DateTime NgayKetThuc { get => _NgayKetThuc; set { _NgayKetThuc = value; OnPropertyChanged(); } }

        private string _ChieuNgang;
        public string ChieuNgang { get => _ChieuNgang; set { _ChieuNgang = value; OnPropertyChanged(); } }

        private string _ChieuDoc;
        public string ChieuDoc { get => _ChieuDoc; set { _ChieuDoc = value; OnPropertyChanged(); } }

        private string _SoDay;
        public string SoDay { get => _SoDay; set { _SoDay = value; OnPropertyChanged(); } }

        private string _SoHangMoiDay;
        public string SoHangMoiDay { get => _SoHangMoiDay; set { _SoHangMoiDay = value; OnPropertyChanged(); } }

        private string _SoGheMoiHang;
        public string SoGheMoiHang { get => _SoGheMoiHang; set { _SoGheMoiHang = value; OnPropertyChanged(); } }


        /* Xay dung phuong thuc luu cac selecteditem vao cac bien*/
        private PhienHop _SelectedItem;


        public PhienHop SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;

                OnPropertyChanged();  /*đồng bộ lên các control có bindding*/

                if (SelectedItem != null)
                {
                    TenPhienHop = SelectedItem.TenPhienHop;
                    Mota = SelectedItem.Mota;
                    HinhThucDisplay = SelectedItem.HinhThuc;
                    NgayBatDau = SelectedItem.NgayBatDau;
                    NgayKetThuc = SelectedItem.NgayKetThuc;
                    ChieuDoc = SelectedItem.ChieuDoc.ToString();
                    ChieuNgang = SelectedItem.ChieuNgang.ToString();
                    SoDay = SelectedItem.SoDay.ToString();
                    SoHangMoiDay = SelectedItem.SoHangMoiDay.ToString();
                    SoGheMoiHang = SelectedItem.SoGheMoiHang;
                }
            }
        }


        /*khoi tao 3 command cho 3 button*/
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand CreateCommand { get; set; }


        bool Tao_SD = false;

        public PhienHopVM()
        {
            
            NgayBatDau = NgayKetThuc = DateTime.Today;


            /* Hien thi danh sach vao gridview*/
            List = new ObservableCollection<PhienHop>(DataProvider.Ins.DB.PhienHops);


            //THÊM PHIÊN HỌP
            AddCommand = new RelayCommand<object>((p) =>  /*Nơi đây xử lý điều kiện trước khi thêm. nếu trả về false button không thể nhấn*/
            {

                if (string.IsNullOrEmpty(TenPhienHop) || string.IsNullOrEmpty(HinhThuc))
                    return false; /*Nếu không điền thông tin tên phiên hợp và hình thức sẽ không được thêm*/
                if (HinhThuc == "0" && (String.IsNullOrEmpty(ChieuDoc) || String.IsNullOrEmpty(ChieuNgang)))
                {
                    return false;
                }

                if (HinhThuc == "1" && (String.IsNullOrEmpty(SoDay) || String.IsNullOrEmpty(SoHangMoiDay) || String.IsNullOrEmpty(SoGheMoiHang)))
                {
                    return false;
                }

                /*Kiểm tra nếu tên phiên họp đã tồn tại thì không thể thêm*/
                var check_unique_tenph = DataProvider.Ins.DB.PhienHops.Where(x => x.TenPhienHop == TenPhienHop);
                if (check_unique_tenph == null || check_unique_tenph.Count() > 0)
                    return false;

                if (NgayBatDau > NgayKetThuc)
                {
                    return false;
                }

                return true;


            }, (p) =>  /*Nơi đây xử lý công việc khi button được nhấn*/
            {
                try
                {
                    /*Nếu ngày bắt đầu or ngày kết thúc không được chỉ định thì sẽ lấy date hiện tại*/
                    if (String.IsNullOrEmpty(NgayBatDau.ToString()) || String.IsNullOrEmpty(NgayKetThuc.ToString()))
                    {
                        NgayBatDau = NgayKetThuc = DateTime.Now;
                    }
                    String hinhthuc = "";
                    if (HinhThuc == "1")
                    {
                        hinhthuc = "Hội trường";
                    }
                    else if (HinhThuc == "0")
                    {
                        hinhthuc = "Phòng họp";
                    }
                    Save_Data.Instance.hinhthuc = hinhthuc;
                    /*   dữ liệu sẽ thêm mới*/
                    PhienHop PhienHop = null;
                    if (hinhthuc == "Phòng họp")
                    {
                        PhienHop = new PhienHop() { TenPhienHop = TenPhienHop, Mota = Mota, HinhThuc = hinhthuc, NgayBatDau = NgayBatDau, NgayKetThuc = NgayKetThuc, ChieuDoc = Convert.ToInt32(ChieuDoc), ChieuNgang = Convert.ToInt32(ChieuNgang) };

                        Save_Data.Instance.length = Convert.ToInt32(ChieuDoc);
                        Save_Data.Instance.width = Convert.ToInt32(ChieuNgang);

                        Tao_SD = true;
                    }
                    else
                    {
                        if (KTra_kichthuong_SDHT(Convert.ToInt32(SoDay), SoGheMoiHang) == true)
                        {
                            Save_Data.Instance.soday = Convert.ToInt32(SoDay);
                            Save_Data.Instance.sohangmoiday = Convert.ToInt32(SoHangMoiDay);
                            Save_Data.Instance.soghemoihang = SoGheMoiHang;

                            PhienHop = new PhienHop() { TenPhienHop = TenPhienHop, Mota = Mota, HinhThuc = hinhthuc, NgayBatDau = NgayBatDau, NgayKetThuc = NgayKetThuc, SoDay = Convert.ToInt32(SoDay), SoHangMoiDay = Convert.ToInt32(SoHangMoiDay), SoGheMoiHang = SoGheMoiHang };
                            Tao_SD = true;
                        }
                        else
                        {
                            Tao_SD = false;
                        }
                    }

                    if (Tao_SD == true)
                    {
                        //thêm dl vào csdl và cập nhật csdl
                        DataProvider.Ins.DB.PhienHops.Add(PhienHop);
                        DataProvider.Ins.DB.SaveChanges();

                        /*cập nhật dữ liệu lên gridview*/
                        List.Add(PhienHop);

                        //lưu id phiên họp vừa tạo
                        var find_id = DataProvider.Ins.DB.PhienHops.Where(x => x.TenPhienHop == TenPhienHop).SingleOrDefault();
                        Save_Data.Instance.idphienhop = find_id.IDPhienHop;
                    }
                }
                catch (Exception EE)
                {

                }

               
            });


            //SỬA PHIÊN HỌP
            EditCommand = new RelayCommand<object>((p) => /*Điều kiện trước khi sửa*/
            {

                /*tồn tại 1 row được chọn*/
                if (SelectedItem == null)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(TenPhienHop) || string.IsNullOrEmpty(HinhThuc))
                    return false; /*Nếu không điền thông tin tên phiên hợp và hình thức sẽ không được thêm*/
                if (HinhThuc == "0" && (String.IsNullOrEmpty(ChieuDoc) || String.IsNullOrEmpty(ChieuNgang)))
                {
                    return false;
                }
                if (HinhThuc == "1" && (String.IsNullOrEmpty(SoDay) || String.IsNullOrEmpty(SoHangMoiDay) || String.IsNullOrEmpty(SoGheMoiHang)))
                {
                    return false;
                }
                /*tên chưa tồn tại*/
                var displayList = DataProvider.Ins.DB.PhienHops.Where(x => x.TenPhienHop == TenPhienHop);
                if (displayList == null || displayList.Count() > 0 && TenPhienHop != SelectedItem.TenPhienHop)
                {
                    return false;

                }
                if (NgayBatDau > NgayKetThuc)
                {
                    return false;
                }

                return true;

            }, (p) =>      /*Xử lý sự kiện khi button sửa được nhấn*/
            {
                try
                {
                    if (String.IsNullOrEmpty(NgayBatDau.ToString()) || String.IsNullOrEmpty(NgayKetThuc.ToString()))
                    {
                        NgayBatDau = NgayKetThuc = DateTime.Now;
                    }
                    String hinhthuc = "";
                    if (HinhThuc == "1")
                    {
                        hinhthuc = "Hội trường";
                    }
                    else if (HinhThuc == "0")
                    {
                        hinhthuc = "Phòng họp";
                    }

                    var Phienhop = DataProvider.Ins.DB.PhienHops.Where(x => x.IDPhienHop == SelectedItem.IDPhienHop).SingleOrDefault();
                    if ((ChieuDoc != SelectedItem.ChieuDoc.ToString() || ChieuNgang != SelectedItem.ChieuNgang.ToString()) || SoDay != SelectedItem.SoDay.ToString() || SoHangMoiDay != SelectedItem.SoHangMoiDay.ToString() || SoGheMoiHang != SelectedItem.SoGheMoiHang || hinhthuc != SelectedItem.HinhThuc)
                    {
                        var thongbao = MessageBox.Show("Bạn sẽ tạo lại sơ đồ nếu điều chỉnh kích thước sơ đồ", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (thongbao == MessageBoxResult.Yes)
                        {
                            var deleteghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == SelectedItem.IDPhienHop);
                            if (deleteghe != null)
                            {
                                DataProvider.Ins.DB.ThongTinGhes.RemoveRange(deleteghe);
                                DataProvider.Ins.DB.SaveChanges();
                            }

                            Phienhop.TenPhienHop = TenPhienHop;
                            Phienhop.Mota = Mota;
                            Phienhop.HinhThuc = hinhthuc;
                            Phienhop.NgayBatDau = NgayBatDau;
                            Phienhop.NgayKetThuc = NgayKetThuc;

                            if (hinhthuc == "Hội trường")
                            {
                                if (KTra_kichthuong_SDHT(Convert.ToInt32(SoDay), SoGheMoiHang) == true)
                                {
                                    Phienhop.SoDay = Convert.ToInt32(SoDay);
                                    Phienhop.SoHangMoiDay = Convert.ToInt32(SoHangMoiDay);
                                    Phienhop.SoGheMoiHang = SoGheMoiHang;

                                    ((MainWindow)Application.Current.MainWindow).XoaSoDo();
                                    Tao_SD = true;
                                }
                                else
                                {
                                    Tao_SD = false;
                                }
                            }
                            else
                            {
                                Phienhop.ChieuDoc = Convert.ToInt32(ChieuDoc);
                                Phienhop.ChieuNgang = Convert.ToInt32(ChieuNgang);
                                ((MainWindow)Application.Current.MainWindow).XoaSoDo();
                                Tao_SD = true;
                            }
                        }
                        else { }
                    }
                    else
                    {
                        Phienhop.TenPhienHop = TenPhienHop;
                        Phienhop.Mota = Mota;
                        Phienhop.HinhThuc = hinhthuc;
                        Phienhop.NgayBatDau = NgayBatDau;
                        Phienhop.NgayKetThuc = NgayKetThuc;

                        Tao_SD = false;
                    }
                    /*Cập nhật dữ liệu vào database*/
                    DataProvider.Ins.DB.SaveChanges();

                    List.Remove(SelectedItem);
                    List.Add(Phienhop);
                    SelectedItem = Phienhop;
                }
                catch(Exception e)
                {

                }
               
            });


            //XOÁ PHIÊN HỌP
            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return true;

            }, (p) =>      /*Xử lý sự kiện khi button xoá được nhấn*/
            {
                try
                {
                    var thongbao = MessageBox.Show("Bạn có muốn xóa phiên họp này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (thongbao == MessageBoxResult.Yes)
                    {
                        /*Xoá thông tin ghế trước*/
                        var deleteghe = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == SelectedItem.IDPhienHop);
                        if (deleteghe != null)
                        {
                            DataProvider.Ins.DB.ThongTinGhes.RemoveRange(deleteghe);
                            DataProvider.Ins.DB.SaveChanges();
                        }
                        /*Xoá phiên họp*/
                        var deletephienhop = DataProvider.Ins.DB.PhienHops.Where(x => x.IDPhienHop == SelectedItem.IDPhienHop).SingleOrDefault();
                        DataProvider.Ins.DB.PhienHops.Remove(deletephienhop);
                        DataProvider.Ins.DB.SaveChanges();

                        List.Remove(SelectedItem);
                        clear();
                        //gọi phương thức xoá sơ đồ
                        ((MainWindow)Application.Current.MainWindow).XoaSoDo();
                        Tao_SD = false;
                    }
                    else { }
                }
                catch
                {

                }
                

            });

            //COPY PHIÊN HỌP
            CopyCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return true;

            }, (p) =>
            {
                try
                {
                    using (var transaction = DataProvider.Ins.DB.Database.BeginTransaction())
                    {
                        string tenphienhop = SelectedItem.TenPhienHop;
                        string new_tenph = taomoitenph(tenphienhop);

                        PhienHop PhienHop = null;
                        if (Save_Data.Instance.hinhthuc == "Phòng họp")
                        {
                            PhienHop = new PhienHop() { 
                                TenPhienHop = new_tenph, 
                                Mota = SelectedItem.Mota, 
                                HinhThuc = SelectedItem.HinhThuc, 
                                NgayBatDau = SelectedItem.NgayBatDau, 
                                NgayKetThuc = SelectedItem.NgayKetThuc, 
                                ChieuDoc = Convert.ToInt32(SelectedItem.ChieuDoc), 
                                ChieuNgang = Convert.ToInt32(SelectedItem.ChieuNgang) };
                        }
                        else
                        {
                            PhienHop = new PhienHop()
                            {
                                TenPhienHop = new_tenph,
                                Mota = SelectedItem.Mota,
                                HinhThuc = SelectedItem.HinhThuc,
                                NgayBatDau = SelectedItem.NgayBatDau,
                                NgayKetThuc = SelectedItem.NgayKetThuc,
                                SoDay = Convert.ToInt32(SelectedItem.SoDay),
                                SoHangMoiDay = Convert.ToInt32(SelectedItem.SoHangMoiDay),
                                SoGheMoiHang = SelectedItem.SoGheMoiHang
                            };
                        }
                        //var PhienHop = new PhienHop() { TenPhienHop = new_tenph, Mota = SelectedItem.Mota, HinhThuc = SelectedItem.HinhThuc, NgayBatDau = SelectedItem.NgayBatDau, NgayKetThuc = SelectedItem.NgayKetThuc, ChieuDoc = Convert.ToInt32(SelectedItem.ChieuDoc), ChieuNgang = Convert.ToInt32(SelectedItem.ChieuNgang) };

                        DataProvider.Ins.DB.PhienHops.Add(PhienHop);
                        DataProvider.Ins.DB.SaveChanges();

                        var TTG = DataProvider.Ins.DB.ThongTinGhes.Where(x => x.IDPhienHop == SelectedItem.IDPhienHop).ToList();

                        foreach (var t in TTG)
                        {
                            var a = new ThongTinGhe() { STT = t.STT, IDGhe = t.IDGhe, TenGhe = t.TenGhe, IDPhienHop = PhienHop.IDPhienHop };
                            DataProvider.Ins.DB.ThongTinGhes.Add(a);
                        }

                        DataProvider.Ins.DB.SaveChanges();
                        transaction.Commit();

                        List.Add(PhienHop);
                        MessageBox.Show("Đã copy thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi copy: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Tao_SD = true;
       
        });

            
        }
        public void clear()
        {
            TenPhienHop = "";
            ChieuDoc = null;
            ChieuNgang = null;
            Mota = "";
            NgayBatDau = NgayKetThuc = DateTime.Now;
            HinhThuc = null;
            SoDay = null;
            SoGheMoiHang = null;
            SoHangMoiDay = null;
        }

        //KIỂM TRA CÁC KÍCH THƯỚC SDHT
        private bool KTra_kichthuong_SDHT(int soday, string soghemoihang)
        {
           
            if (!string.IsNullOrWhiteSpace(soghemoihang))
            {
                string[] widthValues = soghemoihang.Split(',');
                
                //kiểm tra số ghế theo số dãy
                if (widthValues.Length != soday )
                {
                    MessageBox.Show("Vui lòng nhập đúng số lượng ghế cho mỗi dãy", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else
                {
                    for (int i = 0; i < widthValues.Length; i++)
                    {
                        if(int.TryParse(widthValues[i], out int soghe))
                        {
                           
                        }
                        else
                        {
                            MessageBox.Show("Số ghế mỗi hàng không hợp lệ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    }
                    return true;
                }
            }
            else
            {
                MessageBox.Show("Số ghế mỗi hàng không hợp lệ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private string taomoitenph(string tenphienhop)
        {
            string new_tenph = tenphienhop + " copy";
            int copy = 1;
            while (trungten(new_tenph))
            {
                copy++;
                new_tenph = $"{tenphienhop}_Copy{copy}";
            }

            return new_tenph;
        }

        private bool trungten(string tenph)
        {
            return DataProvider.Ins.DB.PhienHops.Any(x => x.TenPhienHop == tenph);
        }




    }
}
