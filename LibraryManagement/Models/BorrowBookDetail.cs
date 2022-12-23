using LibraryManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Models
{
    internal class BorrowBookDetail : BaseViewModel
    {
        public int No { get; set; }
        public int IdBook { get; set; }
        public string NameBook { get; set; }
        public string NameCategory { get; set; }
        public string PublisherName { get; set; }
        public DateTime DateBorrow { get; set; }
        public int Count { get; set; }
    }
}
