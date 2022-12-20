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
    class AuthorViewModel : BaseViewModel
    {
        private ObservableCollection<Models.Author> _ListAuthor;
        public ObservableCollection<Models.Author> ListAuthor { get => _ListAuthor; set { _ListAuthor = value; OnPropertyChanged(); } }

        //Selected data of DB
        private Author _SelectedItem;
        public Author SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    IdAuthor = SelectedItem.idAuthor;
                    NameAuthor = SelectedItem.nameAuthor;
                }
            }
        }

        private int _idAuthor;
        public int IdAuthor { get => _idAuthor; set { _idAuthor = value; OnPropertyChanged(); } }

        private string _nameAuthor;
        public string NameAuthor { get => _nameAuthor; set { _nameAuthor = value; OnPropertyChanged(); } }

        //open Window
        public ICommand AddAuthorCommand { get; set; }


        //effect DB
        public ICommand AddAuthorToDBCommand { get; set; }
        public ICommand DeleteAuthortoDBCommand { get; set; }

        public AuthorViewModel()
        {
            //DB to Window
            ListAuthor = new ObservableCollection<Author>(DataAdapter.Instance.DB.Authors);

            AddAuthorCommand = new AppCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                AddAuthorScreen wd = new AddAuthorScreen();
                NameAuthor = null;
                wd.ShowDialog();
            });

            //AddAuthor
            AddAuthorToDBCommand = new AppCommand<object>((p) =>
            {
                if (NameAuthor == null || NameAuthor == "")
                    return false;
                return true;
            }, (p) =>
            {
                if (NameAuthor == null)
                {
                    MessageBox.Show("Tên tác giả không được bỏ trống");
                    return;
                }
                var displayList = DataAdapter.Instance.DB.Authors.Where(x => x.nameAuthor.ToLower() == NameAuthor.ToLower());
                if (displayList.Count() != 0)
                {
                    MessageBox.Show("Tên tác giả bị trùng");
                    NameAuthor = null;
                    return;
                }
                var author = new Author()
                {
                    nameAuthor = NameAuthor
                };

                DataAdapter.Instance.DB.Authors.Add(author);
                DataAdapter.Instance.DB.SaveChanges();

                ListAuthor.Add(author);
                MessageBox.Show("Thêm tác giả thành công");
            });

            //Delete Author
            DeleteAuthortoDBCommand = new AppCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return true;
            }, (p) =>
            {
                    var author = DataAdapter.Instance.DB.Authors.Where(x => x.idAuthor == SelectedItem.idAuthor).SingleOrDefault();
                    foreach(var el in DataAdapter.Instance.DB.Books)
                    {
                        if (el.Authors.Where(a => a.idAuthor == author.idAuthor).Count() > 0)
                        {
                            MessageBox.Show("Không thể xóa tác giả do tác giả còn được tham chiếu trong sách");
                            return;
                        }
                    }
                    DataAdapter.Instance.DB.Authors.Remove(author);
                    DataAdapter.Instance.DB.SaveChanges();
                    ListAuthor.Remove(author);
                    MessageBox.Show("Xóa tác giả thành công");
            });
        }
    }
}
