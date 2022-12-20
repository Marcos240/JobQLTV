using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagement.ViewModels
{
    class PaginatingCollection : BaseViewModel
    {
        protected int currentPage;
        protected int pageCount;
        protected string keyword;

        public int ItemsPerPage { get; set; }
        public int PageCount { get => pageCount; set { pageCount = value; OnPropertyChanged(); } }
        public int CurrentPage { get => currentPage; set { currentPage = value; OnPropertyChanged(); } }

        public PaginatingCollection(int itemsPerPage)
        {
            ItemsPerPage = itemsPerPage;
            CurrentPage = 1;
        }
        public PaginatingCollection(int itemsPerPage, string keyword)
        {
            this.keyword = keyword;
            ItemsPerPage = itemsPerPage;
            CurrentPage = 1;
        }
        public virtual bool MoveToNextPage()
        {
            if (this.currentPage < this.PageCount)
            {
                this.CurrentPage++;
                return true;
            }
            return false;
        }
        public virtual bool MoveToPreviousPage()
        {
            if (this.currentPage > 1)
            {
                this.CurrentPage--;
                return true;
            }
            return false;
        }
        public virtual void MoveToLastPage()
        {
            this.CurrentPage = this.PageCount;
        }
        public virtual void MoveToFirstPage()
        {
            this.CurrentPage = 1;
        }
    }
}
