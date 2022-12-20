using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Models
{
    class ReportCategory : BaseViewModel
    {
        public Category Category { get; set; }
        public string Name { get; set; }
        public int No { get; set; }
        public int TurnBorrow { get; set; }
        public double Ratio { get; set; }
    }
}
