using LibraryManagement.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Data.Entity.Migrations;
using System.ComponentModel;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Windows.Controls;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Runtime.Remoting;

namespace LibraryManagement.ViewModels
{
    class ReaderViewModel : BaseViewModel
    {
        private ReaderPaginatingCollection _List;
        public ReaderPaginatingCollection List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<TypeReader> _TypeReader;
        public ObservableCollection<TypeReader> TypeReader { get => _TypeReader; set { _TypeReader = value; OnPropertyChanged(); } }

        public Assets.Helper helper = new Assets.Helper();

        private Reader _SelectedItem;
        public Reader SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    IdReader = SelectedItem.idReader;
                    NameReader = SelectedItem.nameReader;
                    DobReader = SelectedItem.dobReader;
                    Email = SelectedItem.email;
                    AddressReader = SelectedItem.addressReader;
                    CreatedAt = SelectedItem.createdAt;
                    Debt = SelectedItem.debt;
                    IdTypeReader = SelectedItem.idTypeReader;
                    SelectedTypeReader = SelectedItem.TypeReader;
                    LatestExtended = SelectedItem.latestExtended;
                }
            }
        }
        private TypeReader _SelectedTypeReader;
        public TypeReader SelectedTypeReader { get => _SelectedTypeReader; set { _SelectedTypeReader = value; OnPropertyChanged(); } }

        private int _idReader;
        public int IdReader { get => _idReader; set { _idReader = value; OnPropertyChanged(); } }

        private string _nameReader;
        public string NameReader {get => _nameReader; set { _nameReader = value; OnPropertyChanged(); } }

        private string _email;
        public string Email { get => _email; set {_email = value;OnPropertyChanged();} }


        private string _addressReader;
        public string AddressReader { get => _addressReader; set { _addressReader = value; OnPropertyChanged(); }}

        private int _idTypeReader;
        public int IdTypeReader { get => _idTypeReader; set { _idTypeReader = value; OnPropertyChanged(); } }

        private DateTime? _createdAt ;
        private DateTime? _dobReader;
        private DateTime? _latestExtended;

        public DateTime? CreatedAt {
            get => _createdAt;
            set
            {   
                _createdAt = value; OnPropertyChanged(); } }
        public DateTime? DobReader { get => _dobReader; set {  _dobReader = value; OnPropertyChanged(); } 
        }
        public DateTime? LatestExtended
        {
            get => _latestExtended;
            set
            {
                _latestExtended = value; OnPropertyChanged();
            }
        }
        private int _debt;
        public int Debt{ get => _debt; set { _debt = value; OnPropertyChanged();}}

        //Search Reader
        private string readerSearchKeyword;
        public string ReaderSearchKeyword
        {
            get => readerSearchKeyword;
            set
            {
                readerSearchKeyword = value;
                OnPropertyChanged();
                InitReaders(helper.RemoveSign4VietnameseString (readerSearchKeyword));
            }
        }


        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand PrepareAddReaderCommand { get; set; }
        public ICommand CheckCommand { get; set;}
        public ICommand AddTypeReaderCommand { get; set;}
        public ICommand DeleteTypeReaderCommand { get; set;}
        public ICommand CancelCommand { get; set;}
        public ICommand CancelAddCommand { get; set; }
        public ICommand ReloadTypeReaderCommand { get; set;}
        public ICommand MoveToPreviousReadersPage { get; set; }
        public ICommand MoveToNextReadersPage { get; set; }
        public ICommand ExtendReaderCard { get; set; }
        public ICommand ExportReader { get; set; }
        public ICommand ImportReaderOld { get; set; }
        public ICommand ImportReaderNew { get; set; }

        public ReaderViewModel()
        {
            // Retrieve data from DB
            InitReaders();
           
            CancelCommand = new AppCommand<object>((p) =>
            {
                if (SelectedItem == null || SelectedTypeReader == null)
                    return false;
                return true;

            }, (p) =>
            {
                SelectedItem.nameReader = NameReader;
                SelectedItem.dobReader = (DateTime)DobReader;
                SelectedItem.email = Email;
                SelectedItem.addressReader = AddressReader;
                SelectedItem.debt = Debt;
                SelectedItem.createdAt = (DateTime)CreatedAt;
                SelectedItem.idTypeReader = IdTypeReader;
                OnPropertyChanged("SelectedItem");
                ReaderSearchKeyword = null;
                InitReaders();
            });
            ReloadTypeReaderCommand = new AppCommand<object>((p) =>
            {
                if (SelectedItem == null || SelectedTypeReader == null)
                    return false;
                return true;

            }, (p) =>
            {
                TypeReader = new ObservableCollection<TypeReader>(DataAdapter.Instance.DB.TypeReaders);
            });
            AddCommand = new AppCommand<object>((p) =>
            {
                if (NameReader == null || Email == null || AddressReader == null )
                    return false;
                return true;

            }, (p) =>
            {
                try
                {
                    var Reader = new Reader()
                    {
                        nameReader = NameReader,
                        dobReader = (DateTime)DobReader,
                        email = Email,
                        addressReader = AddressReader,
                        createdAt = DateTime.Today,
                        latestExtended = (DateTime)CreatedAt,
                        debt = 0,
                        idTypeReader = SelectedTypeReader.idTypeReader,
                        nameReaderSearch = helper.RemoveSign4VietnameseString(NameReader)
                    };
                    DataAdapter.Instance.DB.Readers.Add(Reader);
                    DataAdapter.Instance.DB.SaveChanges();
                    List.MoveToLastPage();
                    SetSelectedItemToFirstItemOfPage(false);
                    MessageBox.Show("Bạn đã thêm người dùng thành công");
                    InitReaders();
                }
                catch(Exception)
                {
                    MessageBox.Show("Đã có lỗi xảy ra!");
                }
            });

            EditCommand = new AppCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return true;

            }, (p) =>
            {
                var Reader = DataAdapter.Instance.DB.Readers.Where(x => x.idReader == SelectedItem.idReader).SingleOrDefault();
                Reader.nameReader = SelectedItem.nameReader;
                Reader.dobReader = (DateTime)SelectedItem.dobReader;
                Reader.email = SelectedItem.email;
                Reader.addressReader = SelectedItem.addressReader;
                Reader.debt = SelectedItem.debt;
                Reader.createdAt = (DateTime)SelectedItem.createdAt;
                Reader.idTypeReader = SelectedItem.TypeReader.idTypeReader;
                Reader.nameReaderSearch = helper.RemoveSign4VietnameseString(SelectedItem.nameReader);
                DataAdapter.Instance.DB.SaveChanges();
                List.Refresh();
                OnPropertyChanged("SelectedItem");
                InitReaders();
                MessageBox.Show("Sửa thông tin độc giả thành công");
            });
            DeleteCommand = new AppCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return true;

            }, (p) =>
            {
                var reader = DataAdapter.Instance.DB.Readers.Where(x => x.idReader == SelectedItem.idReader).SingleOrDefault();

                // Check if reader is borrowing any book? if it is, do not delete reader
                var borrowing = DataAdapter.Instance.DB.DetailBillBorrows
                    .Where(el => el.BillBorrow.idReader == reader.idReader && el.returned == 0)
                    .Count() > 0;
                if (borrowing)
                {
                    MessageBox.Show("Không thể xóa độc giả đang mượn sách");
                    return;
                }
                // Check if reader still has debt need to payment? if it is, do not delete reader
                if (reader.debt > 0)
                {
                    MessageBox.Show("Không thể xóa độc giả còn nợ");
                    return;
                }

                // Otherwise, delete reader anyway
                // 1. delete information about borrowing
                var borrowed = DataAdapter.Instance.DB.BillBorrows
                    .Where(el => el.idReader == reader.idReader);
                foreach(var el in borrowed)
                {
                    var detailBorrowedCorresponding = DataAdapter.Instance.DB.DetailBillBorrows
                        .Where(detail => detail.idBillBorrow == el.idBillBorrow);
                    DataAdapter.Instance.DB.DetailBillBorrows.RemoveRange(detailBorrowedCorresponding);
                }
                DataAdapter.Instance.DB.BillBorrows.RemoveRange(borrowed);
                
                // 2. delete information about returning
                var returned = DataAdapter.Instance.DB.BillReturns
                    .Where(el => el.idReader == reader.idReader);
                foreach (var el in returned)
                {
                    var detailReturnedCorresponding = DataAdapter.Instance.DB.DetailBillReturns
                        .Where(detail => detail.idBillReturn == el.idBillReturn);
                    DataAdapter.Instance.DB.DetailBillReturns.RemoveRange(detailReturnedCorresponding);
                }
                DataAdapter.Instance.DB.BillReturns.RemoveRange(returned);

                // 3. delete information about collecting fine
                var collected = DataAdapter.Instance.DB.Payments.Where(el => el.idReader == reader.idReader);
                DataAdapter.Instance.DB.Payments.RemoveRange(collected);

                // finnally delete it

                DataAdapter.Instance.DB.Readers.Remove(reader);
                try
                {
                    DataAdapter.Instance.DB.SaveChanges();
                    MessageBox.Show("Xóa độc giả thành công");
                }
                catch(Exception)
                {
                    MessageBox.Show("Đã có lỗi xảy ra, không thể thực hiện thao tác xóa độc giả");
                }
                finally
                {
                    List.Refresh();
                    SetSelectedItemToFirstItemOfPage(true);
                }
            });
            PrepareAddReaderCommand = new AppCommand<object>(
                p => true,
                p =>
                {
                    NameReader = null;
                    AddressReader = null;
                    Email = null;
                    DobReader = new DateTime(2000, 1, 1);
                    CreatedAt = DateTime.Now;
                    Debt = 0;
                    SelectedTypeReader = TypeReader.FirstOrDefault();
                });
            CheckCommand = new AppCommand<object>((p) =>
            {
                if (SelectedItem == null || SelectedTypeReader == null)
                    return false;
                return true;

            }, (p) =>
            {
               
            });
            MoveToPreviousReadersPage = new AppCommand<object>(
               p =>
               {
                   return List.CurrentPage > 1;
               },
               p =>
               {
                   List.MoveToPreviousPage();
                   SetSelectedItemToFirstItemOfPage(true);
               });
            MoveToNextReadersPage = new AppCommand<object>(
                p =>
                {
                    return List.CurrentPage < List.PageCount;
                },
                p =>
                {
                    List.MoveToNextPage();
                    SetSelectedItemToFirstItemOfPage(true);
                });
            ExtendReaderCard = new AppCommand<object>(
                p => SelectedItem != null,
                p =>
                {
                    DateTime latestExtendedDate = SelectedItem.latestExtended;
                    int expiryDays = DataAdapter.Instance.DB.Paramaters.Find(4).valueParameter * 30;
                    if (DateTime.Now >= latestExtendedDate.AddDays(expiryDays))
                    {
                        SelectedItem.latestExtended = DateTime.Now;
                    }
                    else
                    {
                        SelectedItem.latestExtended = DateTime.Now.AddDays(SelectedItem.latestExtended.AddDays(expiryDays).Subtract(DateTime.Now).Days);
                    }
                    DataAdapter.Instance.DB.SaveChanges();
                    OnPropertyChanged("SelectedItem");
                    List.Refresh();
                    MessageBox.Show("Gia hạn độc giả thành công!");
                });
            CancelAddCommand = new AppCommand<object>(
                p => true,
                p =>
                {
                    if (SelectedItem != null)
                    {
                        IdReader = SelectedItem.idReader;
                        NameReader = SelectedItem.nameReader;
                        DobReader = SelectedItem.dobReader;
                        Email = SelectedItem.email;
                        AddressReader = SelectedItem.addressReader;
                        CreatedAt = SelectedItem.createdAt;
                        Debt = SelectedItem.debt;
                        IdTypeReader = SelectedItem.idTypeReader;
                        SelectedTypeReader = SelectedItem.TypeReader;
                        SelectedItem = SelectedItem;
                    }
                });
                ExportReader = new AppCommand<object>(
                    param => true,
                    param =>
                    {
                        try
                        {
                            string filePath = "";
                            // tạo SaveFileDialog để lưu file excel
                            SaveFileDialog dialog = new SaveFileDialog();

                            // chỉ lọc ra các file có định dạng Excel
                            dialog.Filter = "Excel | *.xlsx | Excel 2003 | *.xls";

                            // Nếu mở file và chọn nơi lưu file thành công sẽ lưu đường dẫn lại dùng
                            if (dialog.ShowDialog() == true)
                            {
                                filePath = dialog.FileName;

                                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                                using (ExcelPackage p = new ExcelPackage())
                                {
                                    // đặt tên người tạo file
                                    p.Workbook.Properties.Author = "4T UIT";

                                    // đặt tiêu đề cho file
                                    p.Workbook.Properties.Title = "Danh sách độc giả lưu trữ";

                                    //Tạo một sheet để làm việc trên đó
                                    p.Workbook.Worksheets.Add("ListReader LibraryManagement");


                                    // lấy sheet vừa add ra để thao tác
                                    ExcelWorksheet ws = p.Workbook.Worksheets["ListReader LibraryManagement"];

                                    // đặt tên cho sheet
                                    ws.Name = "ListReader LibraryManagement";
                                    // fontsize mặc định cho cả sheet
                                    ws.Cells.Style.Font.Size = 11;
                                    // font family mặc định cho cả sheet
                                    ws.Cells.Style.Font.Name = "Calibri";

                                    // Tạo danh sách các column header
                                    string[] arrColumnHeader = {
                                                        "ID",
                                                        "Họ tên",
                                                        "Ngày sinh",
                                                        "Email",
                                                        "Địa chỉ",
                                                        "Ngày tạo thẻ",
                                                        "Số nợ",
                                                        "Loại độc giả",
                                                        "Ngày gia hạn"
                                };

                                    // lấy ra số lượng cột cần dùng dựa vào số lượng header
                                    var countColHeader = arrColumnHeader.Count();

                                    ws.Cells[1, 1].Value = "Danh sách độc giả";
                                    ws.Cells[1, 1, 1, countColHeader].Merge = true;
                                    // in đậm
                                    ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
                                    // căn giữa
                                    ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                                    //Ngày in danh sách
                                    ws.Cells[2, 1].Value = "Ngày in danh sách: " + DateTime.Today.ToShortDateString();
                                    ws.Cells[2, 1, 2, countColHeader].Merge = true;
                                    // in đậm
                                    ws.Cells[2, 1, 2, countColHeader].Style.Font.Bold = true;
                                    // căn giữa
                                    ws.Cells[2, 1, 2, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    int colIndex = 1;
                                    int rowIndex = 3;

                                    ws.Column(1).Width = 10;
                                    ws.Column(2).Width = 20;
                                    ws.Column(3).Width = 20;
                                    ws.Column(4).Width = 20;
                                    ws.Column(5).Width = 20;
                                    ws.Column(6).Width = 20;
                                    ws.Column(7).Width = 20;
                                    ws.Column(8).Width = 20;
                                    ws.Column(9).Width = 20;
                                    //tạo các header từ column header đã tạo từ bên trên
                                    foreach (var item in arrColumnHeader)
                                    {
                                        var cell = ws.Cells[rowIndex, colIndex];
                                        ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                                        //set màu thành gray
                                        var fill = cell.Style.Fill;
                                        fill.PatternType = ExcelFillStyle.Solid;
                                        fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                                        //căn chỉnh các border
                                        var border = cell.Style.Border;
                                        border.Bottom.Style =
                                            border.Top.Style =
                                            border.Left.Style =
                                            border.Right.Style = ExcelBorderStyle.Thin;

                                        //gán giá trị
                                        cell.Value = item;

                                        colIndex++;
                                    }

                                    //lấy ra danh sách Reader
                                    List<Reader> ListReader = DataAdapter.Instance.DB.Readers.ToList();

                                    //với mỗi item trong danh sách sẽ ghi trên 1 dòng
                                    foreach (var item in ListReader)
                                    {
                                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                                        colIndex = 1;

                                        // rowIndex tương ứng từng dòng dữ liệu
                                        rowIndex++;

                                        //gán giá trị cho từng cell      
                                        ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowIndex, colIndex++].Value = item.idReader;

                                        ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowIndex, colIndex++].Value = item.nameReader;

                                        ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowIndex, colIndex++].Value = item.dobReader.ToShortDateString();

                                        ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowIndex, colIndex++].Value = item.email;

                                        ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowIndex, colIndex++].Value = item.addressReader;

                                        ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowIndex, colIndex++].Value = item.createdAt.ToShortDateString();

                                        ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowIndex, colIndex++].Value = item.debt;

                                        ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowIndex, colIndex++].Value = item.TypeReader.nameTypeReader;

                                        ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[rowIndex, colIndex++].Value = item.latestExtended.ToShortDateString();

                                    }

                                    //Lưu file lại
                                    Byte[] bin = p.GetAsByteArray();
                                    File.WriteAllBytes(filePath, bin);
                                }
                                MessageBox.Show("Xuất excel thành công!");
                            }
                            
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show("Có lỗi khi lưu file");
                            }
                 });
                    ImportReaderOld = new AppCommand<object>(
                    param => true,
                    param =>
                    {
                        try
                        {

                            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                            //// mở file excel
                            OpenFileDialog dlg = new OpenFileDialog();
                            string fileName = "";
                            if (dlg.ShowDialog() == true)
                            {
                                fileName = dlg.FileName;

                                var package = new ExcelPackage(new FileInfo(fileName));

                                // lấy ra sheet đầu tiên để thao tác
                                ExcelWorksheet workSheet = package.Workbook.Worksheets[0];

                                // duyệt tuần tự từ dòng thứ 2 đến dòng cuối cùng của file. lưu ý file excel bắt đầu từ số 1 không phải số 0
                                for (int i = workSheet.Dimension.Start.Row + 3; i <= workSheet.Dimension.End.Row; i++)
                                {
                                    // biến j biểu thị cho một column trong file
                                    int j = 2;
                                    string name = workSheet.Cells[i, j++].Value.ToString();

                                    // lấy ra giá trị ngày tháng và ép kiểu thành DateTime
                                    string dobTemp = workSheet.Cells[i, j++].Value.ToString();
                                    DateTime dob = new DateTime();
                                    if (dobTemp != null)
                                    {
                                        dob = Convert.ToDateTime(dobTemp);
                                    }
                                    string mail = workSheet.Cells[i, j++].Value.ToString();
                                    string address = workSheet.Cells[i, j++].Value.ToString();
                                    string crTemp = workSheet.Cells[i, j++].Value.ToString();
                                    DateTime cr = new DateTime();
                                    if (crTemp != null)
                                    {
                                        cr = Convert.ToDateTime(crTemp);
                                    }
                                    string dbt = workSheet.Cells[i, j++].Value.ToString();
                                    string typerd = workSheet.Cells[i, j++].Value.ToString();

                                    //RetrieveData();
                                    //InitReaders();
                                    int typereader = 1;
                                    foreach (var item in TypeReader)
                                    {
                                        if (item.nameTypeReader == typerd)
                                            typereader = item.idTypeReader;
                                    }
                                    string ltTemp = workSheet.Cells[i, j++].Value.ToString();
                                    DateTime lt = new DateTime();
                                    if (ltTemp != null)
                                    {
                                        lt = Convert.ToDateTime(ltTemp);
                                    }


                                    //tạo Reader từ dữ liệu đã lấy được
                                    Reader rd = new Reader()
                                    {
                                        nameReader = name,
                                        dobReader = dob,
                                        email = mail,
                                        addressReader = address,
                                        createdAt = cr,
                                        debt = Int32.Parse(dbt),
                                        idTypeReader = typereader,
                                        latestExtended = lt
                                    };

                                    // add Reader vào danh sách 
                                    DataAdapter.Instance.DB.Readers.Add(rd);
                                    DataAdapter.Instance.DB.SaveChanges();
                                }
                                List.MoveToLastPage();
                                this.SetSelectedItemToFirstItemOfPage(false);
                                MessageBox.Show("Import danh sách độc giả thành công!");
                            }
                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show("Đã xảy ra lỗi khi Import dữ liệu!");
                        }
                    });
                    ImportReaderNew = new AppCommand<object>(
                    param => true,
                    param =>
                    {
                        try
                        {
                            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                            //// mở file excel
                            OpenFileDialog dlg = new OpenFileDialog();
                            string fileName = "";
                            if (dlg.ShowDialog() == true)
                            {
                                fileName = dlg.FileName;
                                var package = new ExcelPackage(new FileInfo(fileName));

                                // lấy ra sheet đầu tiên để thao tác
                                ExcelWorksheet workSheet = package.Workbook.Worksheets[0];

                                // duyệt tuần tự từ dòng thứ 2 đến dòng cuối cùng của file. lưu ý file excel bắt đầu từ số 1 không phải số 0
                                for (int i = workSheet.Dimension.Start.Row + 2; i <= workSheet.Dimension.End.Row; i++)
                                {
                                    // biến j biểu thị cho một column trong file
                                    int j = 1;
                                    string name = workSheet.Cells[i, j++].Value.ToString();

                                    // lấy ra giá trị ngày tháng và ép kiểu thành DateTime
                                    string dobTemp = workSheet.Cells[i, j++].Value.ToString();
                                    DateTime dob = new DateTime();
                                    if (dobTemp != null)
                                    {
                                        dob = Convert.ToDateTime(dobTemp);
                                    }
                                    string mail = workSheet.Cells[i, j++].Value.ToString();
                                    string address = workSheet.Cells[i, j++].Value.ToString();
                                    string typerd = workSheet.Cells[i, j++].Value.ToString();

                                    //RetrieveData();
                                    int typereader = 1;
                                    foreach (var item in TypeReader)
                                    {
                                        if (item.nameTypeReader == typerd)
                                            typereader = item.idTypeReader;
                                    }


                                    //tạo Reader từ dữ liệu đã lấy được
                                    Reader rd = new Reader()
                                    {
                                        nameReader = name,
                                        dobReader = dob,
                                        email = mail,
                                        addressReader = address,
                                        createdAt = DateTime.Now,
                                        debt = 0,
                                        idTypeReader = typereader,
                                        latestExtended = DateTime.Now
                                    };

                                    // add Reader vào danh sách 
                                    DataAdapter.Instance.DB.Readers.Add(rd);
                                    DataAdapter.Instance.DB.SaveChanges();
                                    //List.AddItem(rd);
                                    //InitProperty(rd.idReader);
                                }
                                List.MoveToLastPage();
                                this.SetSelectedItemToFirstItemOfPage(false);
                                MessageBox.Show("Import danh sách độc giả mới thành công!");
                            }
                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show("Đã xảy ra lỗi khi Import dữ liệu!");
                        }
                    });

        }

        private void InitReaders(string keyword = null)
        {
            TypeReader = new ObservableCollection<TypeReader>(DataAdapter.Instance.DB.TypeReaders);
            if (keyword != null)
            {
                List = new ReaderPaginatingCollection(30, keyword);
            }
            else
            {
                List = new ReaderPaginatingCollection(30);
            }
            SetSelectedItemToFirstItemOfPage(true);
        }
        private void SetSelectedItemToFirstItemOfPage(bool isFirstItem)
        {
            if (List.Readers == null || List.Readers.Count == 0)
            {
                return;
            }
            if (isFirstItem)
            {
                SelectedItem = List.Readers.FirstOrDefault();
            }
            else
            {
                SelectedItem = List.Readers.LastOrDefault();
            }
        }
    }

}
