using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagement.ViewModels
{
    class ParameterViewModel : BaseViewModel
    {
        private int _ageMin;
        public int ageMin
        {
            get => _ageMin; set { _ageMin = value; OnPropertyChanged(); }
        }

        private int _ageMax;
        public int ageMax
        {
            get => _ageMax; set { _ageMax = value; OnPropertyChanged();}
        }

        private int _expiryDate;
        public int expiryDate
        {
            get => _expiryDate; set { _expiryDate = value; OnPropertyChanged(); }
        }

        private int _distancePublish;
        public int distancePublish
        {
            get => _distancePublish; set { _distancePublish = value; OnPropertyChanged(); }
        }

        private int _bookBorrowMax;
        public int bookBorrowMax
        {
            get => _bookBorrowMax; set { _bookBorrowMax = value; OnPropertyChanged(); }
        }

        private int _moneyPenalty;
        public int moneyPenalty
        {
            get => _moneyPenalty; set { _moneyPenalty = value; OnPropertyChanged(); }
        }

        private int _dateBorrowMax;
        public int dateBorrowMax
        {
            get => _dateBorrowMax; set { _dateBorrowMax = value; OnPropertyChanged(); }
        }

     
        public ICommand EditParametersCommand { get; set; }
        public ICommand InitParamaters { get; set; }

        public ParameterViewModel()
        {
            InitParams();
            EditParametersCommand = new AppCommand<object>(
                (p) =>
                {
                    if (ageMin > 0 && ageMax > 0 && expiryDate > 0 && distancePublish > 0 && bookBorrowMax > 0 && moneyPenalty >= 0 && dateBorrowMax > 0) 
                        return true;
                    return false;
                }, (p) =>
                {
                    // Sửa lại các giá trị 
                    // Tuổi tối thiểu
                    DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 1).SingleOrDefault().valueParameter = ageMin;
                    // Tuổi tối đa
                    DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 2).SingleOrDefault().valueParameter = ageMax;
                    // Thời hạn thẻ
                    DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 3).SingleOrDefault().valueParameter = expiryDate;
                    // Khoảng cách xuất bản
                    DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 4).SingleOrDefault().valueParameter = distancePublish;
                    // Số sách mượn tối đa
                    DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 5).SingleOrDefault().valueParameter = bookBorrowMax;
                    // Số tiền phạt trễ theo từng ngày
                    DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 6).SingleOrDefault().valueParameter = moneyPenalty;
                    // Số ngày mượn tối đa
                    DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 7).SingleOrDefault().valueParameter = dateBorrowMax;

                    DataAdapter.Instance.DB.SaveChanges();
                });
            InitParamaters = new AppCommand<object>(
                p => true,
                p =>
                {
                    InitParams();
                });

        }

        private void InitParams()
        {
            ageMin = DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 1).SingleOrDefault().valueParameter;
            ageMax = DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 2).SingleOrDefault().valueParameter;
            expiryDate = DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 3).SingleOrDefault().valueParameter;
            distancePublish = DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 4).SingleOrDefault().valueParameter;
            bookBorrowMax = DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 5).SingleOrDefault().valueParameter;
            moneyPenalty = DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 6).SingleOrDefault().valueParameter;
            dateBorrowMax = DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 7).SingleOrDefault().valueParameter;
    }
    }
}