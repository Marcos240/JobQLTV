using LibraryManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Models
{
    public class PaymentDebt : BaseViewModel
    {
        public int No { get; set; }
        public int IdReader { get; set; }
        public string NameReader { get; set; }
        public int CollectedAmount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
