using LibraryManagement.Models;
using LibraryManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagement.ViewModels
{
    class PublisherViewModel : BaseViewModel
    {
        private ObservableCollection<Models.Publisher> _ListPublisher;
        public ObservableCollection<Models.Publisher> ListPublisher { get => _ListPublisher; set { _ListPublisher = value; OnPropertyChanged(); } }

        //Selected data of DB
        private Publisher _SelectedItem;
        public Publisher SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    IdPublisher = SelectedItem.idPublisher;
                    NamePublisher = SelectedItem.namePublisher;
                }
            }
        }

        private int _idPublisher;
        public int IdPublisher { get => _idPublisher; set { _idPublisher = value; OnPropertyChanged(); } }

        private string _namePublisher;
        public string NamePublisher { get => _namePublisher; set { _namePublisher = value; OnPropertyChanged(); } }

        //open Window
        public ICommand AddPublisherCommand { get; set; }


        //effect DB
        public ICommand AddPublisherToDBCommand { get; set; }
        public ICommand DeletePublishertoDBCommand { get; set; }

        public PublisherViewModel()
        {
            //DB to Window
            ListPublisher = new ObservableCollection<Publisher>(DataAdapter.Instance.DB.Publishers);

            AddPublisherCommand = new AppCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                AddPublisherScreen wd = new AddPublisherScreen();
                NamePublisher = null;
                wd.ShowDialog();
            });

            //AddPublisher
            AddPublisherToDBCommand = new AppCommand<object>((p) =>
            {
                if (NamePublisher == null || NamePublisher == "")
                    return false;
                return true;
            }, (p) =>
            {
                if (NamePublisher == null)
                {
                    MessageBox.Show("Tên nhà sản xuất không được bỏ trống");
                    return;
                }
                var displayList = DataAdapter.Instance.DB.Publishers.Where(x => x.namePublisher.ToLower() == NamePublisher.ToLower());
                if (displayList.Count() != 0)
                {
                    MessageBox.Show("Tên nhà sản xuất bị trùng");
                    NamePublisher = null;
                    return;
                }
                var Publisher = new Publisher()
                {
                    namePublisher = NamePublisher
                };

                DataAdapter.Instance.DB.Publishers.Add(Publisher);
                DataAdapter.Instance.DB.SaveChanges();

                ListPublisher.Add(Publisher);
                MessageBox.Show("Thêm nhà sản xuất thành công");
            });

            //Delete Publisher
            DeletePublishertoDBCommand = new AppCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return true;
            }, (p) =>
            {
                var Publisher = DataAdapter.Instance.DB.Publishers.Where(x => x.idPublisher == SelectedItem.idPublisher).SingleOrDefault();
                foreach (var el in DataAdapter.Instance.DB.Books)
                {
                    if (el.Publisher.idPublisher == Publisher.idPublisher)
                    {
                        MessageBox.Show("Không thể xóa nhà sản xuất do nhà sản xuất còn được tham chiếu trong sách");
                        return;
                    }
                }
                DataAdapter.Instance.DB.Publishers.Remove(Publisher);
                DataAdapter.Instance.DB.SaveChanges();
                ListPublisher.Remove(Publisher);
                MessageBox.Show("Xóa nhà sản xuất thành công");
            });
        }
    }
}
