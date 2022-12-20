using LibraryManagement.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagement.ViewModels
{
    class ReturnBookViewModel : BaseViewModel
    {
        /// <summary>
        /// Variables, Properties definition
        /// </summary>
        private string readerSearchKeyword;
        private ReaderPaginatingCollection listReader;
        private Reader readerSelected;
        private ObservableCollection<DetailBillBorrow> listDetailBorrowCorresponding;
        private ObservableCollection<DetailBillBorrow> listDetailBorrowSelected;
        private BillReturn billReturn;
        private DateTime dateReturn;

        public string ReaderSearchKeyword
        {
            get => readerSearchKeyword;
            set
            {
                readerSearchKeyword = value;
                OnPropertyChanged();
                InitReaders(readerSearchKeyword);
            }
        }
        public ReaderPaginatingCollection ListReader {
            get => listReader;
            set { listReader = value; OnPropertyChanged(); }
        }
        public Reader ReaderSelected {
            get => readerSelected;
            set {
                readerSelected = value;
                OnPropertyChanged();
                RetrieveDetailBorrow();
                if (readerSelected != null)
                {
                    BillReturn.idReader = readerSelected.idReader;
                    OnPropertyChanged("BillReturn");
                }
            }
        }
        public ObservableCollection<DetailBillBorrow> ListDetailBorrowCorresponding {
            get => listDetailBorrowCorresponding;
            set { listDetailBorrowCorresponding = value; OnPropertyChanged(); }
        }
        public ObservableCollection<DetailBillBorrow> ListDetailBorrowSelected {
            get => listDetailBorrowSelected;
            set { listDetailBorrowSelected = value; OnPropertyChanged(); }
        }
        public BillReturn BillReturn {
            get => billReturn;
            set
            {
                billReturn = value; OnPropertyChanged();
            }
        }
        public DateTime DateReturn { get => dateReturn; set { dateReturn = value; OnPropertyChanged(); } }

        public ICommand SelectBook { get; set; }
        public ICommand UnSelectBook { get; set; }
        public ICommand ReturnBook { get; set; }
        public ICommand MoveToPreviousReadersPage { get; set; }
        public ICommand MoveToNextReadersPage { get; set; }




        /// <summary>
        /// Constructor
        /// </summary>

        public ReturnBookViewModel()
        {
            DefineCommands();
            RetrieveDataAndClearInput();
        }

        /// <summary>
        /// Definition for commands
        /// </summary>
        private void DefineCommands()
        {
            SelectBook = new AppCommand<object>(
                p =>
                {
                    return true;
                },
                p =>
                {
                    DetailBillBorrow detailSelected = p as DetailBillBorrow;
                    ListDetailBorrowCorresponding.Remove(detailSelected);
                    ListDetailBorrowSelected.Add(detailSelected);
                    CalculateTotalFine();
                });
            UnSelectBook = new AppCommand<object>(
                p =>
                {
                    return true;
                },
                p =>
                {
                    DetailBillBorrow detailSelected = p as DetailBillBorrow;
                    ListDetailBorrowSelected.Remove(detailSelected);
                    ListDetailBorrowCorresponding.Add(detailSelected);
                    CalculateTotalFine();
                });
            ReturnBook = new AppCommand<object>(
                p =>
                {
                    if (ListDetailBorrowSelected == null || ListDetailBorrowSelected.Count == 0)
                    {
                        return false;
                    }
                    return true;
                },
                p =>
                {
                    // Add Bill return to DB
                    BillReturn.returnDate = DateReturn;
                    DataAdapter.Instance.DB.BillReturns.Add(BillReturn);
                    int finePerExcessDay = DataAdapter.Instance.DB.Paramaters.Find(6).valueParameter; // Get this from DB
                    int daysBorrowAllowed = DataAdapter.Instance.DB.Paramaters.Find(7).valueParameter; // Get this from DB
                    try
                    {
                        foreach (var detailBorrow in ListDetailBorrowSelected)
                        {

                            // Add details of bill return to DB
                            DataAdapter.Instance.DB.DetailBillReturns.Add(
                                new DetailBillReturn
                                {
                                    idBook = detailBorrow.idBook,
                                    idBillReturn = BillReturn.idBillReturn,
                                    idBillBorrow = detailBorrow.idBillBorrow,
                                    dayBorrowed = DateTime.Now.Subtract(detailBorrow.BillBorrow.borrowDate).Days,
                                    fine = getFine(detailBorrow.BillBorrow.borrowDate, finePerExcessDay, daysBorrowAllowed)
                                }
                                );
                            // Change detail borrow status
                            detailBorrow.returned = 1;
                            // Add total fine to reader's debt
                            DataAdapter.Instance.DB.Readers.Find(BillReturn.idReader).debt += BillReturn.sumFine;
                            // Change the book status to available
                            DataAdapter.Instance.DB.Books.Find(detailBorrow.idBook).statusBook = "có sẵn";
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                    finally
                    {
                        // Change Application state to intialize state
                        RetrieveDataAndClearInput();
                    }


                    try
                    {
                        // Save DB state
                        DataAdapter.Instance.DB.SaveChanges();
                    }
                    catch (DbUpdateException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                    catch (DbEntityValidationException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                    catch (NotSupportedException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                    catch (ObjectDisposedException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                    catch (InvalidOperationException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                    finally
                    {
                        // Change Application state to intialize state
                        RetrieveDataAndClearInput();
                        MessageBox.Show("Trả sách thành công!");
                    }
                });
            MoveToPreviousReadersPage = new AppCommand<object>(
               p =>
               {
                   return ListReader.CurrentPage > 1;
               },
               p =>
               {
                   ListReader.MoveToPreviousPage();
               });
            MoveToNextReadersPage = new AppCommand<object>(
                p =>
                {
                    return ListReader.CurrentPage < ListReader.PageCount;
                },
                p =>
                {
                    ListReader.MoveToNextPage();
                });
        }
        /// <summary>
        /// Retrieve default data from DB and initialize properties
        /// </summary>
        private void RetrieveDataAndClearInput()
        {

            BillReturn = new BillReturn { sumFine = 0 };
            DateReturn = DateTime.Now;
            InitReaders();
            ReaderSearchKeyword = null;
            ListDetailBorrowSelected = new ObservableCollection<DetailBillBorrow>();
            RetrieveDetailBorrow();
        }

        /// <summary>
        /// Retrieve detail borrow data with corresponding reader
        /// </summary>
        private void RetrieveDetailBorrow()
        {
            if (ReaderSelected != null)
            {
                try
                {
                    var BooksBorrowedCorresponding = from b in DataAdapter.Instance.DB.Books
                                                     join d in DataAdapter.Instance.DB.DetailBillBorrows on b.idBook equals d.idBook
                                                     join br in DataAdapter.Instance.DB.BillBorrows on d.idBillBorrow equals br.idBillBorrow
                                                     where br.idReader == ReaderSelected.idReader && d.returned == 0
                                                     select d;
                    ListDetailBorrowCorresponding = new ObservableCollection<DetailBillBorrow>(BooksBorrowedCorresponding);
                }
                catch(ArgumentNullException)
                {
                    MessageBox.Show("Đã có lỗi xảy ra khi đọc dữ liệu!");
                    RetrieveDataAndClearInput();
                }
               
            }
            else
            {
                ListDetailBorrowCorresponding = new ObservableCollection<DetailBillBorrow>();
            }
            ListDetailBorrowSelected = new ObservableCollection<DetailBillBorrow>();
        }
        /// <summary>
        /// Calculate total fine for return
        /// </summary>
        private void CalculateTotalFine()
        {
            int finePerExcessDay = DataAdapter.Instance.DB.Paramaters.Find(6).valueParameter; // Get this from DB
            int daysBorrowAllowed = DataAdapter.Instance.DB.Paramaters.Find(7).valueParameter; // Get this from DB
            BillReturn.sumFine = 0;

            // Calculate total fine
            foreach (var detailBorrow in ListDetailBorrowSelected)
            {
                //  Calculate fine of each book
                BillReturn.sumFine += getFine(detailBorrow.BillBorrow.borrowDate, finePerExcessDay, daysBorrowAllowed);
            }
            OnPropertyChanged("BillReturn");
        }
        private int getFine(DateTime date, int finePerExcessDay, int daysBorrowAllowed)
        {   
            int fine = (DateTime.Now.Subtract(date).Days - daysBorrowAllowed) * finePerExcessDay;
            if (fine < 0) fine = 0;
            return fine;
        }


        // Init data for pagination
        private void InitReaders(string keyword = null)
        {
            if (keyword != null)
            {
                ListReader = new ReaderPaginatingCollection(10, keyword);
            }
            else
            {
                ListReader = new ReaderPaginatingCollection(10);
            }
            SetSelectedItemToFirstItemOfPage(true);
        }

        private void SetSelectedItemToFirstItemOfPage(bool isFirstItem)
        {
            if (ListReader.Readers == null || ListReader.Readers.Count == 0)
            {
                return;
            }
            if (isFirstItem)
            {
                ReaderSelected = ListReader.Readers.FirstOrDefault();
            }
            else
            {
                ReaderSelected = ListReader.Readers.LastOrDefault();
            }
        }
    }
}
