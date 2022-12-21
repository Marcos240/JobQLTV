using LibraryManagement.Models;
using LibraryManagement.Views;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using OfficeOpenXml.Filter;
using LibraryManagement.Helpers;

namespace LibraryManagement.ViewModels
{
    class BookViewModel : BaseViewModel
    {
        private BookPaginatingCollection list;
        public BookPaginatingCollection List { get => list; set { list = value; OnPropertyChanged(); } }

        private BookPaginatingCollection _ListLatestBooks;
        public BookPaginatingCollection ListLatestBooks { get => _ListLatestBooks; set { _ListLatestBooks = value; OnPropertyChanged(); } }

        private ObservableCollection<Author> _ListAuthors;
        public ObservableCollection<Author> ListAuthors { get => _ListAuthors; set { _ListAuthors = value; OnPropertyChanged(); } }
        public Helper helper = new Helper();

        //Selected data of DB
        private Book _SelectedItem;
        public Book SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    nameBook = SelectedItem.nameBook;
                    SelectedPublisher = SelectedItem.Publisher;
                    SelectedCategory = SelectedItem.Category;
                    dateAddBook = SelectedItem.dateAddBook;
                    dateManufacture = SelectedItem.dateManufacture;
                    price = SelectedItem.price;
                    BookImageCover = SelectedItem.image;
                    ListAuthors = new ObservableCollection<Author>(SelectedItem.Authors);
                }
            }
        }

        private Category _SelectedCategory;
        public Category SelectedCategory
        {
            get => _SelectedCategory;
            set
            {
                _SelectedCategory = value;
                OnPropertyChanged();
            }
        }

        private Publisher _SelectedPublisher;
        public Publisher SelectedPublisher
        {
            get => _SelectedPublisher;
            set
            {
                _SelectedPublisher = value;
                OnPropertyChanged();
            }
        }

        private Author _SelectedAuthor;
        public Author SelectedAuthor
        {
            get => _SelectedAuthor;
            set
            {
                _SelectedAuthor = value;
                OnPropertyChanged();
            }
        }

        private string _nameBook;
        public string nameBook { get => _nameBook; set { _nameBook = value; OnPropertyChanged(); } }

        private ObservableCollection<Category> _category;
        public ObservableCollection<Category> category { get => _category; set { _category = value; OnPropertyChanged(); } }

        private ObservableCollection<Publisher> _publisher;
        public ObservableCollection<Publisher> publisher { get => _publisher; set { _publisher = value; OnPropertyChanged(); } }

        private ObservableCollection<Author> _author;
        public ObservableCollection<Author> author { get => _author; set { _author = value; OnPropertyChanged(); } }

        private DateTime _dateManufacture;
        public DateTime dateManufacture { get => _dateManufacture; set { _dateManufacture = value; OnPropertyChanged(); } }

        private DateTime _dateAddBook;
        public DateTime dateAddBook
        {
            get
            {
                return _dateAddBook;
            }
            set { _dateAddBook = value; OnPropertyChanged(); }
        }

        private int _price;
        public int price { get => _price; set { _price = value; OnPropertyChanged(); } }

        private string _statusBook;
        public string statusBook { get => _statusBook; set { _statusBook = value; OnPropertyChanged(); } }

        public string SourceImageFile { get; set; }

        //Search Book
        private string bookSearchKeyword;
        public string BookSearchKeyword
        {
            get => bookSearchKeyword;
            set
            {
                bookSearchKeyword = value;
                OnPropertyChanged();
                InitBooks(helper.RemoveSign4VietnameseString(bookSearchKeyword));
            }
        }


        private string bookImageCover;
        public string BookImageCover { get => bookImageCover; set { bookImageCover = value; OnPropertyChanged(); } }

        //open Window
        public ICommand AddBookCommand { get; set; }
        public ICommand DeleteBookCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand CancelAddCommand { get; set; }

        //effect DB
        public ICommand AddBookToDBCommand { get; set; }
        public ICommand EditBookToDBCommand { get; set; }

        public ICommand AddBookFromFileCommand { get; set; }
        public ICommand ExportBooksCommand { get; set; }

        //Pagination
        public ICommand MoveToPreviousBooksPage { get; set; }
        public ICommand MoveToNextBooksPage { get; set; }
        public ICommand PrevBooks { get; set; }
        public ICommand NextBooks { get; set; }

        //Add Authors
        public ICommand AddAuthors { get; set; }

        //Delete Authors
        public ICommand UnSelectedAuthor { get; set; }
        
        //Add book image cover
        public ICommand AddImage { get; set; }
        public ICommand PrepareUpdateImage { get; set; }



        public BookViewModel()
        {
            //BookImageCover = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())), $"/BookImageCover/default-image.png");
            SourceImageFile = null;
            category = new ObservableCollection<Category>(DataAdapter.Instance.DB.Categories);
            publisher = new ObservableCollection<Publisher>(DataAdapter.Instance.DB.Publishers);
            author = new ObservableCollection<Author>(DataAdapter.Instance.DB.Authors);
            ListAuthors = new ObservableCollection<Author>();

            InitBooks();
            GetLastestBooks();

            CancelCommand = new AppCommand<object>((p) =>
            {
                return true;

            }, (p) =>
            {
                if (SelectedItem != null)
                {
                    SelectedItem.nameBook = nameBook;
                    SelectedItem.Category = SelectedCategory;
                    SelectedItem.Publisher = SelectedPublisher;
                    SelectedItem.dateManufacture = dateManufacture;
                    SelectedItem.dateAddBook = dateAddBook;
                    SelectedItem.price = price;
                    OnPropertyChanged("SelectedItem");
                }
                BookSearchKeyword = null;
            });

            AddBookCommand = new AppCommand<object>((p) => 
            { 
                return true; 
            }, (p) => 
            { 
                AddBookScreen wd = new AddBookScreen(); 
                nameBook = "";
                SelectedCategory = null;
                SelectedPublisher = null;
                SelectedAuthor = null;
                ListAuthors = new ObservableCollection<Author>();
                dateAddBook = DateTime.Now;
                dateManufacture = new DateTime(2000, 1, 1);
                price = 0;
                BookImageCover = null;
                wd.ShowDialog();

            });

            CancelAddCommand = new AppCommand<object>(
                p => true,
                p =>
                {
                    if (SelectedItem != null)
                    {
                        nameBook = SelectedItem.nameBook;
                        SelectedPublisher = SelectedItem.Publisher;
                        SelectedCategory = SelectedItem.Category;
                        dateAddBook = SelectedItem.dateAddBook;
                        dateManufacture = SelectedItem.dateManufacture;
                        price = SelectedItem.price;
                        BookImageCover = SelectedItem.image;
                        ListAuthors = new ObservableCollection<Author>(SelectedItem.Authors);
                    }
                    InitBooks();
                });

            AddBookToDBCommand = new AppCommand<object>((p) =>
            {
                //Kiểm tra điều kiện
                if (string.IsNullOrEmpty(nameBook))
                    return false;
                if (price <= 0 || SelectedCategory == null || SelectedPublisher == null || ListAuthors.Count == 0)
                    return false;
                return true;
            }, (p) =>
            {
                string newFileName = GetImageName();
                /// Copy image to project path
                if (SourceImageFile != null)
                {
                    string destinationFile = GetFullPath(newFileName);
                    try
                    {
                        System.IO.File.Copy(SourceImageFile, destinationFile, true);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Đã xảy ra lỗi khi lưu file!");
                    }
                }

                var book = new Book()
                {
                    nameBook = nameBook,
                    dateManufacture = dateManufacture,
                    dateAddBook = dateAddBook,
                    price = price,
                    idCategory = SelectedCategory.idCategory,
                    idPublisher = SelectedPublisher.idPublisher,
                    statusBook = "có sẵn",
                    image = SourceImageFile != null ? newFileName : "default-image.png",
                    nameBookSearch = helper.RemoveSign4VietnameseString(nameBook)
                };

                for (int i = 0; i < ListAuthors.Count; i++)
                {
                    book.Authors.Add(ListAuthors[i]);
                }
                // save changes
                DataAdapter.Instance.DB.Books.Add(book);
                DataAdapter.Instance.DB.SaveChanges();

                SourceImageFile = null;
                List.MoveToLastPage();
                SetSelectedItemToFirstItemOfPage(false);
                GetLastestBooks();
                MessageBox.Show("Thêm sách thành công!");
            });

            //Edit Book Information
            EditBookToDBCommand = new AppCommand<object>((p) =>
            {
                //Kiểm tra điều kiện
                if (string.IsNullOrEmpty(nameBook) || SelectedItem == null)
                    return false;
                if (price <= 0 || SelectedCategory == null || SelectedPublisher == null || ListAuthors.Count == 0)
                    return false;

                return true;
            }, (p) =>
            {
                string newFileName = GetImageName();
                /// Copy image to project path
                if (SourceImageFile != null)
                {
                    string destinationFile = GetFullPath(newFileName);
                    try
                    {
                        System.IO.File.Copy(SourceImageFile, destinationFile, true);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Đã xảy ra lỗi khi lưu file!");
                    }
                }

                var book = DataAdapter.Instance.DB.Books.Where(x => x.idBook == SelectedItem.idBook).SingleOrDefault();
                book.nameBook = SelectedItem.nameBook;
                book.dateManufacture = SelectedItem.dateManufacture;
                book.dateAddBook = SelectedItem.dateAddBook;
                book.price = SelectedItem.price;
                book.idCategory = SelectedItem.Category.idCategory;
                book.idPublisher = SelectedItem.Publisher.idPublisher;
                book.image = SourceImageFile != null ? newFileName : book.image;
                book.nameBookSearch =  helper.RemoveSign4VietnameseString(SelectedItem.nameBook);
                // Save authors
                book.Authors.Clear();
                foreach (var author in ListAuthors)
                {
                    book.Authors.Add(author);
                }
               
                DataAdapter.Instance.DB.SaveChanges();
                SourceImageFile = null;
                List.Refresh();
                OnPropertyChanged("SelectedItem");
                InitBooks();
                MessageBox.Show("Sửa thông tin sách thành công");
                
            });

            //Delete Book
            DeleteBookCommand = new AppCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return true;
            }, (p) =>
            {
                var book = DataAdapter.Instance.DB.Books.Where(x => x.idBook == SelectedItem.idBook).SingleOrDefault();

                // Check if book is being borrowed? if it is, do not delete it.
                var isBeingBorrowed = DataAdapter.Instance.DB.DetailBillBorrows
                    .Where(el => el.idBook == book.idBook && el.returned == 0)
                    .Count() > 0;
                if (isBeingBorrowed)
                {
                    MessageBox.Show("Không thể xóa sách đang được mượn");
                    return;
                }

                // Otherwise, delete it anyway
                // 1. Delete all information about detail borrow
                var detailBorrow = DataAdapter.Instance.DB.DetailBillBorrows.Where(el => el.idBook == el.idBook);
                DataAdapter.Instance.DB.DetailBillBorrows.RemoveRange(detailBorrow);
                // 2. Delete all information about detail return
                var detailReturn = DataAdapter.Instance.DB.DetailBillReturns.Where(el => el.idBook == el.idBook);
                DataAdapter.Instance.DB.DetailBillReturns.RemoveRange(detailReturn);
                // 3. finally delete it
                DataAdapter.Instance.DB.Books.Remove(book);

                // save changes to DB
                try
                {
                    DataAdapter.Instance.DB.SaveChanges();
                    MessageBox.Show("Xóa sách thành công");
                }
                catch (Exception)
                {
                    MessageBox.Show("Đã có lỗi xảy ra, không thể thực hiện thao tác xóa sách");
                }
                finally
                {
                    List.Refresh();
                    SetSelectedItemToFirstItemOfPage(true);
                    GetLastestBooks();
                }
                
            });

            //Command Add Authors
            AddAuthors = new AppCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                if (!ListAuthors.Contains(SelectedAuthor))
                {
                    ListAuthors.Add(SelectedAuthor);
                }
            });

            //Delete Author in List Author
            UnSelectedAuthor = new AppCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                Author removeAuthor = p as Author;
                ListAuthors.Remove(removeAuthor);
            });

            AddImage = new AppCommand<object>(
                
                p => true,
                p =>
                {
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    fileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                    if (fileDialog.ShowDialog() == true)
                    {
                        SourceImageFile = fileDialog.FileName;
                        BookImageCover = SourceImageFile;
                    }
                    else
                    {
                        SourceImageFile = null;
                    }
                });
            AddBookFromFileCommand = new AppCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                string newFileName = GetImageName();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                openFileDialog.Multiselect = false;
                openFileDialog.Title = "Open file excel to import books";
                if (openFileDialog.ShowDialog() == true)
                {
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(openFileDialog.FileName);
                    Excel._Worksheet xlWorkSheet = xlWorkBook.Sheets[1];
                    Excel.Range xlRange = xlWorkSheet.UsedRange;
                    int rowCount = xlRange.Rows.Count;
                    int colCount = xlRange.Columns.Count;
                    for (int i = 2; i <= rowCount; i++)
                    {
                        SourceImageFile = null;
                        nameBook = xlRange.Cells[i, 2].Value.ToString();
                        string nameCategory = xlRange.Cells[i, 3].Value.ToString();
                        foreach (var x in category)
                        {
                            if (x.nameCategory == nameCategory)
                            {
                                SelectedCategory = x;
                                break;
                            }
                        }
                        string authors = xlRange.Cells[i, 4].Value.ToString();
                        string namePublisher = xlRange.Cells[i, 5].Value.ToString();
                        foreach (var x in publisher)
                        {
                            if (x.namePublisher == namePublisher)
                            {
                                SelectedPublisher = x;
                                break;
                            }
                        }
                        dateManufacture = Convert.ToDateTime(xlRange.Cells[i, 6].Value.ToString());
                        dateAddBook = DateTime.Now;
                        price = int.Parse(xlRange.Cells[i, 7].Value.ToString());
                        var book = new Book()
                        {
                            nameBook = nameBook,
                            dateManufacture = dateManufacture,
                            dateAddBook = dateAddBook,
                            price = price,
                            idCategory = SelectedCategory.idCategory,
                            idPublisher = SelectedPublisher.idPublisher,
                            statusBook = "có sẵn",
                            image = SourceImageFile != null ? newFileName : "default-image.png"
                        };
                        ListAuthors = new ObservableCollection<Author>();
                        string[] nameAuthors = authors.Split('|');
                        for (int j = 0; j < nameAuthors.Length; j++)
                        {
                            string nameAuthor = nameAuthors[j];
                            foreach (var x in author)
                            {
                                if (x.nameAuthor == nameAuthor)
                                {
                                    SelectedAuthor = x;
                                    break;
                                }
                            }
                            ListAuthors.Add(SelectedAuthor);
                        }
                        for (int j = 0; j < ListAuthors.Count; j++)
                        {
                            book.Authors.Add(ListAuthors[j]);
                        }
                        DataAdapter.Instance.DB.Books.Add(book);
                        DataAdapter.Instance.DB.SaveChanges();
                        //InitProperties(book.idBook);
                    }

                    //cleanup
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    //rule of thumb for releasing com objects:
                    //  never use two dots, all COM objects must be referenced and released individually
                    //  ex: [somthing].[something].[something] is bad

                    //release com objects to fully kill excel process from running in the background
                    Marshal.ReleaseComObject(xlRange);
                    Marshal.ReleaseComObject(xlWorkSheet);

                    //close and release
                    xlWorkBook.Close();
                    Marshal.ReleaseComObject(xlWorkBook);

                    //quit and release
                    xlApp.Quit();
                    Marshal.ReleaseComObject(xlApp);

                    GetLastestBooks();
                    List.MoveToLastPage();
                    this.SetSelectedItemToFirstItemOfPage(false);
                    MessageBox.Show("Thêm sách thành công!");
                }
            });

            ExportBooksCommand = new AppCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.Title = "Choose place to save export file";
                if (saveFileDialog.ShowDialog() == true)
                {
                    object misValue = System.Reflection.Missing.Value;
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Add(misValue);
                    Excel._Worksheet xlWorkSheet = xlWorkBook.Sheets[1];
                    xlWorkSheet.Cells[1, 1] = "STT";
                    xlWorkSheet.Cells[1, 2] = "Tên sách";
                    xlWorkSheet.Cells[1, 3] = "Thể loại";
                    xlWorkSheet.Cells[1, 4] = "Tác giả";
                    xlWorkSheet.Cells[1, 5] = "Nhà xuất bản";
                    xlWorkSheet.Cells[1, 6] = "Năm xuất bản";
                    xlWorkSheet.Cells[1, 7] = "Trị giá";

                    BookPaginatingCollection books = new BookPaginatingCollection();
                    int j = 0;
                    for (int temp = 0; temp < books.PageCount; temp++)
                    {
                        for (int i = 0; i < books.Books.Count; i++)
                        {
                            xlWorkSheet.Cells[i + 2 + j, 1] = (i + 1 + j).ToString();
                            xlWorkSheet.Cells[i + 2 + j, 2] = books.Books[i].nameBook;
                            xlWorkSheet.Cells[i + 2 + j, 3] = books.Books[i].Category.nameCategory;
                            ICollection<Author> listAuthors = books.Books[i].Authors.ToList();
                            string writer = "";
                            for (int k = 0; k < listAuthors.Count; k++)
                                writer += (listAuthors.ElementAt(k).nameAuthor + "|");
                            if (writer.Length > 0)
                                writer = writer.Remove(writer.Length - 1);
                            xlWorkSheet.Cells[i + 2 + j, 4] = writer;
                            xlWorkSheet.Cells[i + 2 + j, 5] = books.Books[i].Publisher.namePublisher;
                            xlWorkSheet.Cells[i + 2 + j, 6] = books.Books[i].dateManufacture;
                            xlWorkSheet.Cells[i + 2 + j, 7] = books.Books[i].price;
                        }
                        j += books.Books.Count;
                        books.MoveToNextPage();
                    }

                    xlWorkBook.SaveAs(saveFileDialog.FileName, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    releaseObject(xlWorkSheet);
                    releaseObject(xlWorkBook);
                    releaseObject(xlApp);

                    MessageBoxResult messageBoxResult = MessageBox.Show("Xuất file thành công! Bạn có muốn mở file?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.FileName = saveFileDialog.FileName;
                        if (openFileDialog.ShowDialog() == true)
                        {
                            System.Diagnostics.Process.Start(openFileDialog.FileName);
                        }
                    }
                }
            });


            MoveToPreviousBooksPage = new AppCommand<object>(
                p =>
                {
                    return this.List.CurrentPage > 1;
                },
                p =>
                {
                    List.MoveToPreviousPage();
                    SetSelectedItemToFirstItemOfPage(true);
                });
            MoveToNextBooksPage = new AppCommand<object>(
                p =>
                {
                    return this.List.CurrentPage < this.List.PageCount;
                },
                p =>
                {
                    List.MoveToNextPage();
                    SetSelectedItemToFirstItemOfPage(true);
                });

            PrevBooks = new AppCommand<object>(
                p =>
                {
                    return ListLatestBooks.CurrentPage > 1;
                },
                p =>
                {
                    ListLatestBooks.MoveToPreviousPage();
                    OnPropertyChanged("ListLatestBooks");
                });
            NextBooks = new AppCommand<object>(
                p =>
                {
                    return ListLatestBooks.CurrentPage < List.PageCount;
                },
                p =>
                {
                    ListLatestBooks.MoveToNextPage();
                    OnPropertyChanged("ListLatestBooks");
                });
            PrepareUpdateImage = new AppCommand<object>(
                p => true,
                p =>
                {
                    BookImageCover = GetFullPath(SelectedItem.image);
                });
        }

        private string GetImageName()
        {
            List<String> images = DataAdapter.Instance.DB.Books.Select(b => b.image).ToList();
            int max = 0;
            foreach(var el in images)
            {
                string idPart = el.Split('-')[1].Split('.')[0]; // default-photo.png, photo-1.png
                int id;
                if (Int32.TryParse(idPart, out id))
                {
                    if (id > max) max = id;
                }
            }
            max += 1;
            return $"photo-{max}.png";
        }

        private string GetFullPath(string fileName)
        {
            string destPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            string destinationFile = Path.Combine($"{destPath}\\BookImageCover", fileName);
            return destinationFile;
        }

        private void InitBooks(string keyword = null)
        {
            if (keyword != null)
            {
                List = new BookPaginatingCollection(30, keyword);
            }
            else
            {
                List = new BookPaginatingCollection(30);
            }
            SetSelectedItemToFirstItemOfPage(true);
        }
        private void SetSelectedItemToFirstItemOfPage(bool isFirstItem)
        {
            if (List.Books == null || List.Books.Count == 0)
            {
               return;
            }
            if (isFirstItem)
            {
                SelectedItem = List.Books.FirstOrDefault();
            }
            else
            {
                SelectedItem = List.Books.LastOrDefault();
            }
        }

        private void GetLastestBooks()
        {
            ListLatestBooks = new LatestBookPaginationCollection(9, 18);
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
