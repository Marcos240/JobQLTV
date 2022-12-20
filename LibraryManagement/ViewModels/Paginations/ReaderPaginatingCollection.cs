using LibraryManagement.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LibraryManagement.ViewModels
{
    class ReaderPaginatingCollection : PaginatingCollection
    {
        private ObservableCollection<Reader> readers;
        public ObservableCollection<Reader> Readers { get => readers; set { readers = value; OnPropertyChanged(); } }

        public ReaderPaginatingCollection(int itemsPerPage = 15) : base(itemsPerPage)
        {
            LoadItems();
        }

        public ReaderPaginatingCollection(int itemsPerPage, string keyword) : base(itemsPerPage, keyword)
        {
            LoadItems();
        }

        private void LoadItems()
        {
            int totalItems = DataAdapter.Instance.DB.Readers.Count();
            this.PageCount = 1 + (totalItems - 1) / this.ItemsPerPage;

            int items = this.ItemsPerPage;
            //MessageBox.Show($"current={this.CurrentPage}, count={this.PageCount}");
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
            //MessageBox.Show($"current={this.CurrentPage}, count={this.PageCount}");

            // Load data based on keyword for searching

            if (this.keyword == null || this.keyword.Trim() == "")
            {
                var ReadersInPage = DataAdapter.Instance.DB.Readers
                    .OrderBy(el => el.idReader)
                    .Skip((CurrentPage - 1) * ItemsPerPage)
                    .Take(items);
                this.Readers = new ObservableCollection<Reader>(ReadersInPage);
                //MessageBox.Show(Readers.Count.ToString());
                return;
            }
            try
            {
                var ReadersInPage = DataAdapter.Instance.DB.Readers
                    .Where(reader => reader.nameReader.ToLower().StartsWith(keyword.ToLower()))
                    .OrderBy(el => el.idReader)
                    .Skip((CurrentPage - 1) * ItemsPerPage)
                    .Take(items);
                this.Readers = new ObservableCollection<Reader>(ReadersInPage);
                RefrestPageCount(this.keyword);
            }
            catch (ArgumentNullException)
            {
                var ReadersInPage = DataAdapter.Instance.DB.Readers
                  .OrderBy(el => el.idReader)
                  .Skip((CurrentPage - 1) * ItemsPerPage)
                  .Take(items);
                this.Readers = new ObservableCollection<Reader>(ReadersInPage);
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
                totalItems = DataAdapter.Instance.DB.Readers.Count();
            }
            else
            {
                totalItems = DataAdapter.Instance.DB.Readers
                    .Where(reader => reader.nameReader.ToLower().StartsWith(keyword.ToLower())).Count();
            }
            this.PageCount = 1 + (totalItems - 1) / this.ItemsPerPage;
        }
    }
}
