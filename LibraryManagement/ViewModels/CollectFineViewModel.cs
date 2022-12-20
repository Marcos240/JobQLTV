using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagement.ViewModels
{
    class CollectFineViewModel : BaseViewModel
    {
        /// <summary>
        /// Definition of fields
        /// </summary>
        private ReaderPaginatingCollection listReader;
        private Reader readerSelected;
        private Payment payment;
        private int collectedAmount;
        private string readerKeyword;
        /// <summary>
        /// Fefinition of properties
        /// </summary>
        public ReaderPaginatingCollection ListReader
        {
            get => listReader;
            set
            {
                listReader = value;
                OnPropertyChanged();
            }
        }
        public Reader ReaderSelected
        {
            get => readerSelected;
            set
            {
                readerSelected = value;
                calcRemainingDebt();
                OnPropertyChanged();
            }
        }
        public Payment Payment
        {
            get => payment;
            set
            {
                payment = value;
                OnPropertyChanged();
            }
        }
        public int CollectedAmount
        {
            get => collectedAmount;
            set { collectedAmount = value; calcRemainingDebt(); OnPropertyChanged(); }
        }
        public string ReaderKeyword { get => readerKeyword; set { readerKeyword = value; OnPropertyChanged(); InitReaders(readerKeyword); } }
        /// <summary>
        /// Command for buttons
        /// </summary>
        public ICommand CollectFine { get; set; }
        public ICommand MoveToPreviousReadersPage { get; set; }
        public ICommand MoveToNextReadersPage { get; set; }


        /// <summary>
        /// Contructor
        /// </summary>
        public CollectFineViewModel()
        {
            // Init some data
            Init();
            // Define command
            CollectFine = new AppCommand<object>(
                p =>
                {
                    if (ReaderSelected == null || collectedAmount <= 0 || (ReaderSelected != null && ReaderSelected.debt == 0))
                        return false;
                    return true;
                },
                p =>
                {

                    var reader = DataAdapter.Instance.DB.Readers.Find(ReaderSelected.idReader);
                    Payment.idReader = ReaderSelected.idReader;
                    Payment.paymentDate = DateTime.Now;
                    Payment.collectedAmount = CollectedAmount;
                    Payment.currentDebt = ReaderSelected.debt;
                    Payment.remainDebt = Payment.currentDebt - Payment.collectedAmount;
                    reader.debt = Payment.remainDebt;

                    DataAdapter.Instance.DB.Payments.Add(Payment);

                    try
                    {
                        // Save DB state
                        DataAdapter.Instance.DB.SaveChanges();
                        Init();
                        MessageBox.Show("Thu tiền phạt thành công!");
                    }
                    catch (DbUpdateException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                    catch (DbEntityValidationException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                    catch (NotSupportedException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                    catch (ObjectDisposedException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                    catch (InvalidOperationException)
                    {
                        MessageBox.Show("Không thể thao tác vì lỗi cơ sở dữ liệu!");
                    }
                }
            );
            MoveToPreviousReadersPage = new AppCommand<object>(
              p =>
              {
                  return ListReader.CurrentPage > 1;
              },
              p =>
              {
                  ListReader.MoveToPreviousPage();
              });
            MoveToNextReadersPage = new AppCommand<object>(
                p =>
                {
                    return ListReader.CurrentPage < ListReader.PageCount;
                },
                p =>
                {
                    ListReader.MoveToNextPage();
                });
        }

        private void Init()
        {
            Payment = new Payment { paymentDate = DateTime.Now };
            InitReaders();
            CollectedAmount = 0;
            
        }

        private void calcRemainingDebt()
        {
            if (Payment != null && ReaderSelected != null)
            {
                Payment.remainDebt = ReaderSelected.debt - CollectedAmount >= 0 ? ReaderSelected.debt - CollectedAmount : 0;
                OnPropertyChanged("Payment");
            }
        }



        // Init data for pagination
        private void InitReaders(string keyword = null)
        {
            if (keyword != null)
            {
                ListReader = new ReaderPaginatingCollection(10, keyword);
            }
            else
            {
                ListReader = new ReaderPaginatingCollection(10);
            }
            SetSelectedItemToFirstItemOfPage(true);
        }

        private void SetSelectedItemToFirstItemOfPage(bool isFirstItem)
        {
            if (ListReader.Readers == null || ListReader.Readers.Count == 0)
            {
                return;
            }
            if (isFirstItem)
            {
                ReaderSelected = ListReader.Readers.FirstOrDefault();
            }
            else
            {
                ReaderSelected = ListReader.Readers.LastOrDefault();
            }
        }
    }
}
