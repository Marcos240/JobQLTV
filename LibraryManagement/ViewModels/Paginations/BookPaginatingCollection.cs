using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LibraryManagement.ViewModels
{
    class BookPaginatingCollection : PaginatingCollection
    {
        private ObservableCollection<Book> books;
        public ObservableCollection<Book> Books { get => books; set { books = value; OnPropertyChanged(); } }

        public BookPaginatingCollection(int itemsPerPage = 15) : base(itemsPerPage)
        {
            LoadItems();
        }

        public BookPaginatingCollection(int itemsPerPage, string keyword) : base(itemsPerPage, keyword)
        {
            LoadItems();
        }

        protected virtual void LoadItems()
        {
            int totalItems = DataAdapter.Instance.DB.Books.Count();
            this.PageCount = 1 + (totalItems - 1) / this.ItemsPerPage;


            int items = this.ItemsPerPage;
            if (this.CurrentPage > this.PageCount)
            {
                this.CurrentPage = this.PageCount;
            }
            if (this.CurrentPage == this.PageCount)
            {
                if (totalItems % this.ItemsPerPage == 0)
                {
                    items = ItemsPerPage;
                }
                else
                {
                    items = totalItems % this.ItemsPerPage;
                }
            }

            // Load data based on keyword for searching

            if (this.keyword == null || this.keyword.Trim() == "")
            {
                var BooksInpage = DataAdapter.Instance.DB.Books
                    .OrderBy(el => el.idBook)
                    .Skip((CurrentPage - 1) * ItemsPerPage)
                    .Take(items);
                this.Books = new ObservableCollection<Book>(BooksInpage);
                return;
            }
            try
            {
                var BooksInpage = DataAdapter.Instance.DB.Books
                    .Where(book => book.nameBook.ToLower().StartsWith(this.keyword.ToLower()))
                    .OrderBy(el => el.idBook)
                    .Skip((CurrentPage - 1) * ItemsPerPage)
                    .Take(items);
                this.Books = new ObservableCollection<Book>(BooksInpage);
                RefrestPageCount(this.keyword);
            }
            catch (ArgumentNullException)
            {
                var BooksInpage = DataAdapter.Instance.DB.Books
                    .OrderBy(el => el.idBook)
                    .Skip((CurrentPage - 1) * ItemsPerPage)
                    .Take(items);
                this.Books = new ObservableCollection<Book>(BooksInpage);
                MessageBox.Show("Từ khóa tìm kiếm rỗng!");
            }
        }

        public override bool MoveToPreviousPage()
        {
            if (base.MoveToPreviousPage())
            {
                LoadItems();
                return true;
            };
            return false;
        }
        public override bool MoveToNextPage()
        {
            if (base.MoveToNextPage())
            {
                LoadItems();
                return true;
            }
            return false;
        }

        public override void MoveToLastPage()
        {
            RefrestPageCount();
            base.MoveToLastPage();
            LoadItems();
        }
        public override void MoveToFirstPage()
        {
            base.MoveToFirstPage();
            LoadItems();
        }

        public void Refresh()
        {
            LoadItems();
        }
        private void RefrestPageCount(string keyword = null)
        {
            int totalItems;
            if (keyword == null)
            {
                totalItems = DataAdapter.Instance.DB.Books.Count();
            }
            else
            {
                totalItems = DataAdapter.Instance.DB.Books
                    .Where(book => book.nameBook.ToLower().StartsWith(keyword.ToLower())).Count();
            }
            this.PageCount = 1 + (totalItems - 1) / this.ItemsPerPage;
        }
    }
}
