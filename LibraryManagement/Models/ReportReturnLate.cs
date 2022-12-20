using LibraryManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Models
{
    class ReportReturnLate : BaseViewModel
    {
        public int No { get; set; }
        public string Name { get; set; }
        public DateTime DateBorrow { get; set; }
        public int DaysReturnLate { get; set; }
    }
}
