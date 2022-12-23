using LibraryManagement.Models;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LibraryManagement.ViewModels
{
    class ReportViewModel : BaseViewModel
    {

        private int _month;
        public int Month { get => _month; set { _month = value; OnPropertyChanged(); } }
        private int _year;
        public int Year { get => _year; set { _year = value; OnPropertyChanged(); } }
        public int _sumBorrow;
        public int SumBorrow { get => _sumBorrow; set { _sumBorrow = value; OnPropertyChanged(); } }

        public DateTime _dateExpired;
        public DateTime DateExpired { get => _dateExpired; set { _dateExpired = value; OnPropertyChanged(); } }

        // private property for borrowbook list by time
        private DateTime _searchDate;
        private int _searchMonth;
        private int _searchQuarter;
        private int _searchYear;

        // public property for borrowbook list by time
        public DateTime SearchDate
        {
            get => _searchDate;
            set
            {
                _searchDate = value;
                OnPropertyChanged();
                getBookReportByTime(SearchDate, SearchDate.AddDays(1));
            }
        }
        public int SearchMonth
        {
            get => _searchMonth;
            set
            {
                _searchMonth = value;
                OnPropertyChanged();
                var firstDayOfMonth = new DateTime(DateTime.Today.Year, SearchMonth, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                getBookReportByTime(firstDayOfMonth, lastDayOfMonth);
            }
        }
        public int SearchQuarter
        {
            get => _searchQuarter;
            set
            {
                _searchQuarter = value;
                OnPropertyChanged();
                DateTime firstDayOfQuarter = new DateTime(DateTime.Today.Year, 3 * SearchQuarter - 2, 1);
                DateTime lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
                getBookReportByTime(firstDayOfQuarter, lastDayOfQuarter);
            }
        }
        public int SearchYear
        {
            get => _searchYear;
            set
            {
                _searchYear = value;
                OnPropertyChanged();
                DateTime firstDay = new DateTime(SearchYear, 1, 1);
                DateTime lastDay = new DateTime(SearchYear, 12, 31);
                getBookReportByTime(firstDay, lastDay);
            }
        }


        //Danh sách thể loại
        private ObservableCollection<ReportCategory> _ListCategory;
        public ObservableCollection<ReportCategory> Category_List { get => _ListCategory; set { _ListCategory = value; OnPropertyChanged(); } }
        public AppCommand<object> LoadReportCategory { get; }
        //Danh sách sách trả trễ
        private ObservableCollection<ReportReturnLate> _ListLate;
        public ObservableCollection<ReportReturnLate> Late_List { get => _ListLate; set { _ListLate = value; OnPropertyChanged(); } }
        
        //Danh sách sách đang mượn
        private ObservableCollection<BorrowBook> _borrowList;
        public ObservableCollection<BorrowBook> BorrowList { get => _borrowList; set { _borrowList = value; OnPropertyChanged(); } }
        //Danh sách thống kê số lượt mượn
        private ObservableCollection<BorrowBookDetail> _borrowListByTime;
        public ObservableCollection<BorrowBookDetail> BorrowListByTime { get => _borrowListByTime; set { _borrowListByTime = value; OnPropertyChanged();  } }

        private ObservableCollection<BorrowBookDetail> _top10List;
        public ObservableCollection<BorrowBookDetail> Top10List { get => _top10List; set { _top10List = value; OnPropertyChanged(); } }

        public AppCommand<object> LoadReportLate { get; }
        public AppCommand<object> ExportCategory { get; set; }
        public AppCommand<object> ExportLate { get; set; }
        public AppCommand<object> ExportBorrowingBook { get; set; }
        public AppCommand<object> ExportBestBook { get; set; }
        public AppCommand<object> ExportBorrowBookByTime { get; set; }
        public AppCommand<object> ReloadReport { get; set; }
        // public AppCommand<object>  LoadReportBorrow { get; set; }

        public ReportViewModel()
        {
            // init data
            SearchDate = DateTime.Now;
            //Report Book borow with category
            Category_List = new ObservableCollection<ReportCategory>();
            var categoryList = DataAdapter.Instance.DB.Categories;
            int sumTurn = 0;
            int i = 1;
            //Cal ALL tunrn borrow save in SumTurn  
            foreach (var item in categoryList)
            {
                var Cate1 = from b in DataAdapter.Instance.DB.Books
                            join d in DataAdapter.Instance.DB.DetailBillBorrows on b.idBook equals d.idBook
                            join br in DataAdapter.Instance.DB.BillBorrows on d.idBillBorrow equals br.idBillBorrow
                            where b.idCategory == item.idCategory && br.borrowDate.Month == DateTime.Today.Month && br.borrowDate.Year == DateTime.Today.Year
                            select b;
                try
                {
                    if (Cate1 != null)
                    {
                        int sumBook = 0;

                        if (Cate1 != null)
                        {
                            sumBook = Cate1.Sum(b => 1);
                        }
                        sumTurn += sumBook;
                        i++;

                    }
                }
                catch (Exception)
                {

                }
            }
            // -- End Cal ALL tunrn borrow save in SumTurn  
            SumBorrow = sumTurn;
            //Load Turn Borrow with Month of Today and Year of Today
            i = 1;
            foreach (var item1 in categoryList)
            {
                var Cate2 = from b in DataAdapter.Instance.DB.Books
                            join d in DataAdapter.Instance.DB.DetailBillBorrows on b.idBook equals d.idBook
                            join br in DataAdapter.Instance.DB.BillBorrows on d.idBillBorrow equals br.idBillBorrow
                            where b.idCategory == item1.idCategory && br.borrowDate.Month == DateTime.Today.Month && br.borrowDate.Year == DateTime.Today.Year
                            select b;
                try
                {
                    if (Cate2 != null)
                    {
                        int sumBook = 0;

                        if (Cate2 != null)
                        {
                            sumBook = Cate2.Sum(b => 1);
                        }
                        ReportCategory report = new ReportCategory();
                        report.Name = item1.nameCategory;
                        report.No = i;
                        report.TurnBorrow = sumBook;
                        report.Ratio = (sumBook * 100) / sumTurn;
                        Category_List.Add(report);
                        i++;

                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show("Đã có lỗi xảy ra!");
                }
            }
            // -- End Load Turn Borrow with Month of Today and Year of Today

            //Load Turn Borrow with Month and Year User select.
            LoadReportCategory = new AppCommand<object>((p) =>
            {
                if (Month.ToString() != null && Year.ToString() != null)
                    return true;
                else return false;

            }, (p) =>
            {
                Category_List.Clear();
                sumTurn = 0;
                i = 1;

                //Calculator Sum turn borrow
                foreach (var item2 in categoryList)
                {
                    var Cate3 = from b in DataAdapter.Instance.DB.Books
                                join d in DataAdapter.Instance.DB.DetailBillBorrows on b.idBook equals d.idBook
                                join br in DataAdapter.Instance.DB.BillBorrows on d.idBillBorrow equals br.idBillBorrow
                                where (b.idCategory == item2.idCategory && br.borrowDate.Month == Month && br.borrowDate.Year == Year)
                                select b;
                    try
                    {
                        if (Cate3 != null)
                        {
                            int sumBook = 0;

                            if (Cate3 != null)
                            {
                                sumBook = Cate3.Sum(b => 1);
                            }
                            sumTurn += sumBook;
                            i++;

                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                // -- End Calculator Sum turn borrow
                SumBorrow = sumTurn;
                //Add report Category into Category_List
                i = 1;
                foreach (var item3 in categoryList)
                {
                    var Cate4 = from b in DataAdapter.Instance.DB.Books
                                join d in DataAdapter.Instance.DB.DetailBillBorrows on b.idBook equals d.idBook
                                join br in DataAdapter.Instance.DB.BillBorrows on d.idBillBorrow equals br.idBillBorrow
                                where (b.idCategory == item3.idCategory && br.borrowDate.Month == Month && br.borrowDate.Year == Year)
                                select b;
                    try
                    {
                        if (Cate4 != null)
                        {
                            int sumBook = 0;

                            if (Cate4 != null)
                            {
                                sumBook = Cate4.Sum(b => 1);
                            }
                            ReportCategory report = new ReportCategory();
                            report.Name = item3.nameCategory;
                            report.No = i;
                            report.TurnBorrow = sumBook;
                            report.Ratio = (sumBook * 100) / sumTurn;
                            Category_List.Add(report);
                            i++;

                        }
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show("Đã có lỗi xảy ra!");
                    }
                }
            });

            // -- End Report Book borow with category


            // Report Book return late    
            Late_List = new ObservableCollection<ReportReturnLate>();
            var billBBorrow = DataAdapter.Instance.DB.BillBorrows;
            DateTime dateCal;
            int j;
            DateExpired = DateTime.Today;


            //Load book borrowing return late from today
            dateCal = DateExpired.AddDays(-(DataAdapter.Instance.DB.Paramaters.Find(7).valueParameter));
            j = 1;
            foreach (var item5 in billBBorrow)
            {
                var Late3 = from b in DataAdapter.Instance.DB.Books
                            join d in DataAdapter.Instance.DB.DetailBillBorrows on b.idBook equals d.idBook
                            join br in DataAdapter.Instance.DB.BillBorrows on d.idBillBorrow equals br.idBillBorrow
                            where br.idBillBorrow == item5.idBillBorrow && d.returned == 0
                            select new { NameBook = b.nameBook, BorrowDate = item5.borrowDate };
                try
                {
                    foreach (var item6 in Late3)
                    {
                        ReportReturnLate reportlate = new ReportReturnLate();
                        reportlate.Name = item6.NameBook;
                        reportlate.No = j;
                        reportlate.DateBorrow = item6.BorrowDate;
                        reportlate.DaysReturnLate = (int)((dateCal - item6.BorrowDate).TotalDays);
                        if ((dateCal - item6.BorrowDate).TotalDays - (int)((dateCal - item6.BorrowDate).TotalDays) > 0) reportlate.DaysReturnLate++;
                        if (reportlate.DaysReturnLate > 0)
                        {
                            Late_List.Add(reportlate);
                            j++;
                        }
                    }

                }
                catch (Exception)
                {
                    //MessageBox.Show("Đã có lỗi xảy ra!");
                }

            }
            //End load book borrowing return late from today

            //Command LoadReportLate
            LoadReportLate = new AppCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                //Load Book return late with day you select
                Late_List.Clear();
                dateCal = DateExpired.AddDays(-(DataAdapter.Instance.DB.Paramaters.Find(7).valueParameter));
                j = 1;
                foreach (var item5 in billBBorrow)
                {
                    var Late3 = from b in DataAdapter.Instance.DB.Books
                                join d in DataAdapter.Instance.DB.DetailBillBorrows on b.idBook equals d.idBook
                                join br in DataAdapter.Instance.DB.BillBorrows on d.idBillBorrow equals br.idBillBorrow
                                where br.idBillBorrow == item5.idBillBorrow && d.returned == 0
                                select new { NameBook = b.nameBook, BorrowDate = item5.borrowDate };
                    try
                    {
                        foreach (var item6 in Late3)
                        {
                            ReportReturnLate reportlate = new ReportReturnLate();
                            reportlate.Name = item6.NameBook;
                            reportlate.No = j;
                            reportlate.DateBorrow = item6.BorrowDate;
                            reportlate.DaysReturnLate = (int)((dateCal - item6.BorrowDate).TotalDays);
                            if ((dateCal - item6.BorrowDate).TotalDays - (int)((dateCal - item6.BorrowDate).TotalDays) > 0) reportlate.DaysReturnLate++;
                            if (reportlate.DaysReturnLate > 0)
                            {
                                Late_List.Add(reportlate);
                                j++;
                            }
                        }

                    }
                    catch (Exception)
                    {
                        //MessageBox.Show("Đã có lỗi xảy ra!");
                    }

                }
            });
           
            //Export Excel borrow book by category
            ExportCategory = new AppCommand<object>(
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
                        }

                        // nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
                        if (string.IsNullOrEmpty(filePath))
                        {
                            return;
                        }

                        ExcelPackage.LicenseContext = LicenseContext.Commercial;

                        // If you use EPPlus in a noncommercial context
                        // according to the Polyform Noncommercial license:
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (ExcelPackage p = new ExcelPackage())
                        {
                            // đặt tên người tạo file
                            p.Workbook.Properties.Author = "Team";

                            // đặt tiêu đề cho file
                            p.Workbook.Properties.Title = "Báo cáo thống kê sách mượn theo thể loại";

                            //Tạo một sheet để làm việc trên đó
                            p.Workbook.Worksheets.Add("Report LibraryManagement");


                            // lấy sheet vừa add ra để thao tác
                            ExcelWorksheet ws = p.Workbook.Worksheets["Report LibraryManagement"];

                            // đặt tên cho sheet
                            ws.Name = "Report LibraryManagement";
                            // fontsize mặc định cho cả sheet
                            ws.Cells.Style.Font.Size = 11;
                            // font family mặc định cho cả sheet
                            ws.Cells.Style.Font.Name = "Calibri";

                            // Tạo danh sách các column header
                            string[] arrColumnHeader = {
                                                    "STT",
                                                    "Tên thể loại",
                                                    "Số lượt mượn",
                                                    "Tỉ lệ(%)"
                    };

                            // lấy ra số lượng cột cần dùng dựa vào số lượng header
                            var countColHeader = arrColumnHeader.Count();

                            // merge các column lại từ column 1 đến số column header
                            // gán giá trị cho cell vừa merge là Thống kê thông tni User Kteam
                            ws.Cells[1, 1].Value = "Báo cáo thống kê sách mượn theo thể loại";
                            ws.Cells[1, 1, 1, countColHeader].Merge = true;
                            // in đậm
                            ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            //Tháng với cả năm
                            //tháng
                            ws.Cells[2, 1].Value = "Tháng: " + Month;
                            ws.Cells[2, 1, 2, 2].Merge = true;
                            // in đậm
                            ws.Cells[2, 1, 2, 2].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[2, 1, 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            //năm
                            ws.Cells[2, 3].Value = "Năm: " + Year;
                            ws.Cells[2, 3, 2, 4].Merge = true;
                            // in đậm
                            ws.Cells[2, 3, 2, 4].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[2, 3, 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            //Tổng số lượt mượn
                            ws.Cells[3, 1].Value = "Tổng số lượt mượn: " + SumBorrow;
                            ws.Cells[3, 1, 3, countColHeader].Merge = true;
                            // in đậm
                            ws.Cells[3, 1, 3, countColHeader].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[3, 1, 3, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            int colIndex = 1;
                            int rowIndex = 4;

                            ws.Column(1).Width = 15;
                            ws.Column(2).Width = 15;
                            ws.Column(3).Width = 15;
                            ws.Column(4).Width = 15;
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

                            //lấy ra danh sách ListCategory từ ItemSource của DataGrid
                            List<ReportCategory> ListCate = Category_List.Cast<ReportCategory>().ToList();

                            //với mỗi item trong danh sách sẽ ghi trên 1 dòng
                            foreach (var item in ListCate)
                            {
                                // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                                colIndex = 1;

                                // rowIndex tương ứng từng dòng dữ liệu
                                rowIndex++;

                                //gán giá trị cho từng cell      
                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.No;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.Name;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.TurnBorrow;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.Ratio;

                            }

                            //Lưu file lại
                            Byte[] bin = p.GetAsByteArray();
                            File.WriteAllBytes(filePath, bin);
                        }
                        MessageBox.Show("Xuất excel thành công!");
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show("Có lỗi khi lưu file");
                    }
                });

            //Export Excel borrow LATE book
            ExportLate = new AppCommand<object>(
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
                        }

                        // nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
                        if (string.IsNullOrEmpty(filePath))
                        {
                            return;
                        }

                        ExcelPackage.LicenseContext = LicenseContext.Commercial;

                        // If you use EPPlus in a noncommercial context
                        // according to the Polyform Noncommercial license:
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (ExcelPackage p = new ExcelPackage())
                        {
                            // đặt tên người tạo file
                            p.Workbook.Properties.Author = "Team";

                            // đặt tiêu đề cho file
                            p.Workbook.Properties.Title = "Báo cáo thống kê sách trả trễ";

                            //Tạo một sheet để làm việc trên đó
                            p.Workbook.Worksheets.Add("Report LibraryManagement");


                            // lấy sheet vừa add ra để thao tác
                            ExcelWorksheet ws = p.Workbook.Worksheets["Report LibraryManagement"];

                            // đặt tên cho sheet
                            ws.Name = "Report LibraryManagement";
                            // fontsize mặc định cho cả sheet
                            ws.Cells.Style.Font.Size = 11;
                            // font family mặc định cho cả sheet
                            ws.Cells.Style.Font.Name = "Calibri";

                            // Tạo danh sách các column header
                            string[] arrColumnHeader = {
                                                    "STT",
                                                    "Tên sách",
                                                    "Ngày mượn",
                                                    "Số ngày trả trễ"
                    };

                            // lấy ra số lượng cột cần dùng dựa vào số lượng header
                            var countColHeader = arrColumnHeader.Count();

                            // merge các column lại từ column 1 đến số column header
                            // gán giá trị cho cell vừa merge là Thống kê thông tni User Kteam
                            ws.Cells[1, 1].Value = "Báo cáo thống kê sách trả trễ";
                            ws.Cells[1, 1, 1, countColHeader].Merge = true;
                            // in đậm
                            ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            //Ngày
                            ws.Cells[2, 1].Value = "Ngày: " + DateExpired.ToShortDateString();
                            ws.Cells[2, 1, 2, countColHeader].Merge = true;
                            // in đậm
                            ws.Cells[2, 1, 2, countColHeader].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[2, 1, 2, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                            int colIndex = 1;
                            int rowIndex = 3;

                            ws.Column(1).Width = 15;
                            ws.Column(2).Width = 15;
                            ws.Column(3).Width = 15;
                            ws.Column(4).Width = 15;
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

                            //lấy ra danh sách ListCategory từ ItemSource của DataGrid
                            List<ReportReturnLate> ListCate = Late_List.Cast<ReportReturnLate>().ToList();

                            //với mỗi item trong danh sách sẽ ghi trên 1 dòng
                            foreach (var item in ListCate)
                            {
                                // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                                colIndex = 1;

                                // rowIndex tương ứng từng dòng dữ liệu
                                rowIndex++;

                                //gán giá trị cho từng cell      
                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.No;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.Name;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.DateBorrow.ToShortDateString();

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.DaysReturnLate;

                            }

                            //Lưu file lại
                            Byte[] bin = p.GetAsByteArray();
                            File.WriteAllBytes(filePath, bin);
                        }
                        MessageBox.Show("Xuất excel thành công!");
                    }
                    catch (Exception EE)
                    {
                        MessageBox.Show("Có lỗi khi lưu file");
                    }
                });

            //Export Excel BRROWING BOOK
            ExportBorrowingBook = new AppCommand<object>(
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
                        }

                        // nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
                        if (string.IsNullOrEmpty(filePath))
                        {
                            return;
                        }

                        ExcelPackage.LicenseContext = LicenseContext.Commercial;

                        // If you use EPPlus in a noncommercial context
                        // according to the Polyform Noncommercial license:
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (ExcelPackage p = new ExcelPackage())
                        {
                            // đặt tên người tạo file
                            p.Workbook.Properties.Author = "Team";

                            // đặt tiêu đề cho file
                            p.Workbook.Properties.Title = "Báo cáo thống kê sách đang mượn";

                            //Tạo một sheet để làm việc trên đó
                            p.Workbook.Worksheets.Add("Report LibraryManagement");


                            // lấy sheet vừa add ra để thao tác
                            ExcelWorksheet ws = p.Workbook.Worksheets["Report LibraryManagement"];

                            // đặt tên cho sheet
                            ws.Name = "Report LibraryManagement";
                            // fontsize mặc định cho cả sheet
                            ws.Cells.Style.Font.Size = 11;
                            // font family mặc định cho cả sheet
                            ws.Cells.Style.Font.Name = "Calibri";

                            // Tạo danh sách các column header
                            string[] arrColumnHeader = {
                                                    "STT",
                                                    "Tên sách",
                                                    "Loại sách",
                                                    "Nhà sản xuất",
                                                    "Ngày mượn",
                                                    "Số ngày mượn còn lại",
                                                    "Người mượn"
                    };

                            // lấy ra số lượng cột cần dùng dựa vào số lượng header
                            var countColHeader = arrColumnHeader.Count();

                            // merge các column lại từ column 1 đến số column header
                            ws.Cells[1, 1].Value = "Báo cáo thống kê sách đang mượn";
                            ws.Cells[1, 1, 1, countColHeader].Merge = true;
                            // in đậm
                            ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            //Ngày xuất báo cáo
                            ws.Cells[2, 1].Value = "Ngày xuất báo cáo: " + DateTime.Today.ToShortDateString();
                            ws.Cells[2, 1, 2, countColHeader].Merge = true;
                            // in đậm
                            ws.Cells[2, 1, 2, countColHeader].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[2, 1, 2, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            int colIndex = 1;
                            int rowIndex = 3;

                            ws.Column(1).Width = 25;
                            ws.Column(2).Width = 25;
                            ws.Column(3).Width = 25;
                            ws.Column(4).Width = 25;
                            ws.Column(5).Width = 25;
                            ws.Column(6).Width = 25;
                            ws.Column(7).Width = 25;
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

                            //lấy ra danh sách ListCategory từ ItemSource của DataGrid
                            List<BorrowBook> ListCate = BorrowList.Cast<BorrowBook>().ToList();

                            //với mỗi item trong danh sách sẽ ghi trên 1 dòng
                            foreach (var item in ListCate)
                            {
                                // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                                colIndex = 1;

                                // rowIndex tương ứng từng dòng dữ liệu
                                rowIndex++;

                                //gán giá trị cho từng cell      
                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.No;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.NameBook;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.NameCategory;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.PublisherName;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.DateBorrow.ToShortDateString();

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.NumberDateBorrow;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.UserBorrow;

                            }

                            //Lưu file lại
                            Byte[] bin = p.GetAsByteArray();
                            File.WriteAllBytes(filePath, bin);
                        }
                        MessageBox.Show("Xuất excel thành công!");
                    }
                    catch (Exception EE)
                    {
                        MessageBox.Show(EE.Message);
                        MessageBox.Show("Có lỗi khi lưu file");
                    }
                });

            //Export Excel Top10 best book
            ExportBestBook = new AppCommand<object>(
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
                        }

                        // nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
                        if (string.IsNullOrEmpty(filePath))
                        {
                            return;
                        }

                        ExcelPackage.LicenseContext = LicenseContext.Commercial;

                        // If you use EPPlus in a noncommercial context
                        // according to the Polyform Noncommercial license:
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (ExcelPackage p = new ExcelPackage())
                        {
                            // đặt tên người tạo file
                            p.Workbook.Properties.Author = "Team";

                            // đặt tiêu đề cho file
                            p.Workbook.Properties.Title = "Báo cáo thống kê top 10 sách được mượn nhiều";

                            //Tạo một sheet để làm việc trên đó
                            p.Workbook.Worksheets.Add("Report LibraryManagement");


                            // lấy sheet vừa add ra để thao tác
                            ExcelWorksheet ws = p.Workbook.Worksheets["Report LibraryManagement"];

                            // đặt tên cho sheet
                            ws.Name = "Report LibraryManagement";
                            // fontsize mặc định cho cả sheet
                            ws.Cells.Style.Font.Size = 11;
                            // font family mặc định cho cả sheet
                            ws.Cells.Style.Font.Name = "Calibri";

                            // Tạo danh sách các column header
                            string[] arrColumnHeader = {
                                                    "STT",
                                                    "Tên sách",
                                                    "Loại sách",
                                                    "Nhà sản xuất",
                                                    "Số lượng mượn"
                    };

                            // lấy ra số lượng cột cần dùng dựa vào số lượng header
                            var countColHeader = arrColumnHeader.Count();

                            // merge các column lại từ column 1 đến số column header
                            ws.Cells[1, 1].Value = "Báo cáo thống kê TOP 10 sách mượn nhiều nhất";
                            ws.Cells[1, 1, 1, countColHeader].Merge = true;
                            // in đậm
                            ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            //Ngày xuất báo cáo
                            ws.Cells[2, 1].Value = "Ngày xuất báo cáo: " + DateTime.Today.ToShortDateString();
                            ws.Cells[2, 1, 2, countColHeader].Merge = true;
                            // in đậm
                            ws.Cells[2, 1, 2, countColHeader].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[2, 1, 2, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            int colIndex = 1;
                            int rowIndex = 3;

                            ws.Column(1).Width = 25;
                            ws.Column(2).Width = 25;
                            ws.Column(3).Width = 25;
                            ws.Column(4).Width = 25;
                            ws.Column(5).Width = 25;
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

                            //lấy ra danh sách ListCategory từ ItemSource của DataGrid
                            List<BorrowBookDetail> ListCate = Top10List.Cast<BorrowBookDetail>().ToList();

                            //với mỗi item trong danh sách sẽ ghi trên 1 dòng
                            foreach (var item in ListCate)
                            {
                                // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                                colIndex = 1;

                                // rowIndex tương ứng từng dòng dữ liệu
                                rowIndex++;

                                //gán giá trị cho từng cell      
                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.No;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.NameBook;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.NameCategory;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.PublisherName;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.Count;

                            }

                            //Lưu file lại
                            Byte[] bin = p.GetAsByteArray();
                            File.WriteAllBytes(filePath, bin);
                        }
                        MessageBox.Show("Xuất excel thành công!");
                    }
                    catch (Exception EE)
                    {
                        MessageBox.Show(EE.Message);
                        MessageBox.Show("Có lỗi khi lưu file");
                    }
                });

            //Export Excel brrow book by time
            ExportBorrowBookByTime = new AppCommand<object>(
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
                        }

                        // nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
                        if (string.IsNullOrEmpty(filePath))
                        {
                            return;
                        }

                        ExcelPackage.LicenseContext = LicenseContext.Commercial;

                        // If you use EPPlus in a noncommercial context
                        // according to the Polyform Noncommercial license:
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (ExcelPackage p = new ExcelPackage())
                        {
                            // đặt tên người tạo file
                            p.Workbook.Properties.Author = "Team";

                            // đặt tiêu đề cho file
                            p.Workbook.Properties.Title = "Báo cáo thống kê số lượt mượn theo thời gian";

                            //Tạo một sheet để làm việc trên đó
                            p.Workbook.Worksheets.Add("Report LibraryManagement");


                            // lấy sheet vừa add ra để thao tác
                            ExcelWorksheet ws = p.Workbook.Worksheets["Report LibraryManagement"];

                            // đặt tên cho sheet
                            ws.Name = "Report LibraryManagement";
                            // fontsize mặc định cho cả sheet
                            ws.Cells.Style.Font.Size = 11;
                            // font family mặc định cho cả sheet
                            ws.Cells.Style.Font.Name = "Calibri";

                            // Tạo danh sách các column header
                            string[] arrColumnHeader = {
                                                    "STT",
                                                    "Tên sách",
                                                    "Loại sách",
                                                    "Nhà sản xuất",
                                                    "Số lượng mượn"
                    };

                            // lấy ra số lượng cột cần dùng dựa vào số lượng header
                            var countColHeader = arrColumnHeader.Count();

                            // merge các column lại từ column 1 đến số column header
                            ws.Cells[1, 1].Value = "Báo cáo thống kê số lượt mượn theo thời gian";
                            ws.Cells[1, 1, 1, countColHeader].Merge = true;
                            // in đậm
                            ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            //Ngày xuất báo cáo
                            ws.Cells[2, 1].Value = "Ngày xuất báo cáo: " + DateTime.Today.ToShortDateString();
                            ws.Cells[2, 1, 2, countColHeader].Merge = true;
                            // in đậm
                            ws.Cells[2, 1, 2, countColHeader].Style.Font.Bold = true;
                            // căn giữa
                            ws.Cells[2, 1, 2, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            int colIndex = 1;
                            int rowIndex = 3;

                            ws.Column(1).Width = 25;
                            ws.Column(2).Width = 25;
                            ws.Column(3).Width = 25;
                            ws.Column(4).Width = 25;
                            ws.Column(5).Width = 25;
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

                            //lấy ra danh sách ListCategory từ ItemSource của DataGrid
                            List<BorrowBookDetail> ListCate = BorrowListByTime.Cast<BorrowBookDetail>().ToList();

                            //với mỗi item trong danh sách sẽ ghi trên 1 dòng
                            foreach (var item in ListCate)
                            {
                                // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                                colIndex = 1;

                                // rowIndex tương ứng từng dòng dữ liệu
                                rowIndex++;

                                //gán giá trị cho từng cell      
                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.No;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.NameBook;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.NameCategory;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.PublisherName;

                                ws.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowIndex, colIndex++].Value = item.Count;

                            }

                            //Lưu file lại
                            Byte[] bin = p.GetAsByteArray();
                            File.WriteAllBytes(filePath, bin);
                        }
                        MessageBox.Show("Xuất excel thành công!");
                    }
                    catch (Exception EE)
                    {
                        MessageBox.Show(EE.Message);
                        MessageBox.Show("Có lỗi khi lưu file");
                    }
                });

            ReloadReport = new AppCommand<object>((p) =>
                {
                    return true;
                }, (p) =>
                {
                    //Load Book return late with day you select
                    init();
                    getTop10();
                    getBookReportByTime(DateTime.Today,DateTime.Today.AddDays(1));
                });
        }
        // End Report Book return late 
        private void init()
        { 
            int dateExpire = DataAdapter.Instance.DB.Paramaters.Find(7).valueParameter;
            try
            {
                var BorrowBookLst = (from b in DataAdapter.Instance.DB.Books
                                     join d in DataAdapter.Instance.DB.DetailBillBorrows on b.idBook equals d.idBook
                                     join br in DataAdapter.Instance.DB.BillBorrows on d.idBillBorrow equals br.idBillBorrow
                                     join user in DataAdapter.Instance.DB.Readers on br.idReader equals user.idReader
                                     where d.returned == 0
                                     select new
                                     {
                                         NameBook = b.nameBook,
                                         DateBorrow = br.borrowDate,
                                         NameCategory = b.Category.nameCategory,
                                         PublisherName = b.Publisher.namePublisher,
                                         UserBorrow = user.nameReader,
                                     }
                                    ).AsEnumerable().Select((item, index) => new BorrowBook {
                                        No = index+1,
                                        NameBook = item.NameBook,
                                        DateBorrow = item.DateBorrow,
                                        NameCategory = item.NameCategory,
                                        PublisherName = item.PublisherName,
                                        UserBorrow = item.UserBorrow,
                                        NumberDateBorrow = dateExpire - DateTime.Now.Subtract(item.DateBorrow).Days
                                    }).Where((item) => item.NumberDateBorrow >= 0).ToList();

                if(BorrowBookLst.Any()) BorrowList = new ObservableCollection<BorrowBook>(BorrowBookLst);
            }
            catch (Exception EE)
            {
                MessageBox.Show("Lỗi lấy data");
            }
        }

        private void getTop10()
        {
            var BorrowBookLst = (from b in DataAdapter.Instance.DB.Books
                                 join d in DataAdapter.Instance.DB.DetailBillBorrows on b.idBook equals d.idBook
                                 join br in DataAdapter.Instance.DB.BillBorrows on d.idBillBorrow equals br.idBillBorrow
                                 select new
                                     {
                                         IdBook =b.idBook,
                                         NameBook = b.nameBook,
                                         DateBorrow = br.borrowDate,
                                         NameCategory = b.Category.nameCategory,
                                         PublisherName = b.Publisher.namePublisher,
                                     }
                                 ).GroupBy(o=> new { o.IdBook,o.NameBook,o.NameCategory,o.PublisherName}).AsEnumerable()
                                .Select((item, index) => new BorrowBookDetail
                                {
                                    No = index + 1,
                                    IdBook = item.Key.IdBook,
                                    NameBook = item.Key.NameBook,
                                    NameCategory = item.Key.NameCategory,
                                    PublisherName = item.Key.PublisherName,
                                    Count = item.Count()
                                }).OrderByDescending(item=>item.Count)
                                .Take(10).ToList();

            Top10List = new ObservableCollection<BorrowBookDetail>(BorrowBookLst);
        }

        private void getBookReportByTime(DateTime fromDate,DateTime toDate)
        {
            var BorrowBookLst = (from b in DataAdapter.Instance.DB.Books
                                 join d in DataAdapter.Instance.DB.DetailBillBorrows on b.idBook equals d.idBook
                                 join br in DataAdapter.Instance.DB.BillBorrows on d.idBillBorrow equals br.idBillBorrow
                                 select new
                                 {
                                     IdBook = b.idBook,
                                     NameBook = b.nameBook,
                                     DateBorrow = br.borrowDate,
                                     NameCategory = b.Category.nameCategory,
                                     PublisherName = b.Publisher.namePublisher,
                                 }
                                ).Where((item) => fromDate <= item.DateBorrow && item.DateBorrow <= toDate)
                                .GroupBy(o => new { o.IdBook, o.NameBook, o.NameCategory, o.PublisherName }).AsEnumerable()
                                .Select((item, index) => new BorrowBookDetail
                                {
                                    No = index + 1,
                                    IdBook = item.Key.IdBook,
                                    NameBook = item.Key.NameBook,
                                    NameCategory = item.Key.NameCategory,
                                    PublisherName = item.Key.PublisherName,
                                    Count = item.Count()
                                })
                                .OrderByDescending(item => item.Count)
                                .ToList();

            BorrowListByTime = new ObservableCollection<BorrowBookDetail>(BorrowBookLst);
        }
    }


}
