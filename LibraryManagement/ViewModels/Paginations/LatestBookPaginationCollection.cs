using LibraryManagement.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LibraryManagement.ViewModels
{
    class LatestBookPaginationCollection : BookPaginatingCollection
    {
        private int maxItemsLoad;
        public LatestBookPaginationCollection(int itemsPerPage = 9, int maxItemsLoad = 18) : base(itemsPerPage)
        {
            this.maxItemsLoad = maxItemsLoad;
            LoadItems();
        }

        protected override void LoadItems()
        {
            int totalItems = this.maxItemsLoad;
            this.PageCount = 1 + (totalItems - 1) / this.ItemsPerPage;


            int items = this.ItemsPerPage;
           
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
            //MessageBox.Show($"totalitem={totalItems}, pagecount={PageCount}, items={items}");
            // Load data based on keyword for searching

            if (this.keyword == null || this.keyword.Trim() == "")
            {
                var BooksInpage = DataAdapter.Instance.DB.Books
                    .OrderByDescending(el => el.dateAddBook)
                    .Skip((CurrentPage - 1) * ItemsPerPage)
                    .Take(items);
                this.Books = new ObservableCollection<Book>(BooksInpage);
            }
        }
    }
}
