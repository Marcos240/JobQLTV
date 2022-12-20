using LibraryManagement.Models;
using LibraryManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagement.ViewModels
{
    class CategoryViewModel : BaseViewModel
    {
        private ObservableCollection<Models.Category> _ListCategory;
        public ObservableCollection<Models.Category> ListCategory { get => _ListCategory; set { _ListCategory = value; OnPropertyChanged(); } }

        //Selected data of DB
        private Category _SelectedItem;
        public Category SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    IdCategory = SelectedItem.idCategory;
                    NameCategory = SelectedItem.nameCategory;
                }
            }
        }

        private int _idCategory;
        public int IdCategory { get => _idCategory; set { _idCategory = value; OnPropertyChanged(); } }

        private string _nameCategory;
        public string NameCategory { get => _nameCategory; set { _nameCategory = value; OnPropertyChanged(); } }

        //open Window
        public ICommand AddCategoryCommand { get; set; }


        //effect DB
        public ICommand AddCategoryToDBCommand { get; set; }
        public ICommand DeleteCategorytoDBCommand { get; set; }

        public CategoryViewModel()
        {
            //DB to Window
            ListCategory = new ObservableCollection<Category>(DataAdapter.Instance.DB.Categories);

            AddCategoryCommand = new AppCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                AddCategoryScreen wd = new AddCategoryScreen();
                NameCategory = null;
                wd.ShowDialog();
            });

            //Add Category
            AddCategoryToDBCommand = new AppCommand<object>((p) =>
            {
                if (NameCategory == null || NameCategory == "")
                    return false;
                return true;

            }, (p) =>
            {
                if (NameCategory == null)
                {
                    MessageBox.Show("Thể loại không được bỏ trống");
                    return; 
                }
                var displayList = DataAdapter.Instance.DB.Categories.Where(x => x.nameCategory.ToLower() == NameCategory.ToLower());
                if (displayList.Count() != 0)
                {
                    MessageBox.Show("Thể loại bị trùng");
                    NameCategory = null;
                    return;
                }
                var category = new Category()
                {
                    nameCategory = NameCategory
                };

                DataAdapter.Instance.DB.Categories.Add(category);
                DataAdapter.Instance.DB.SaveChanges();

                ListCategory.Add(category);
                MessageBox.Show("Thêm thể loại thành công");
            });

            //Delete Category
            DeleteCategorytoDBCommand = new AppCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return true;
            }, (p) =>
            {
                var category = DataAdapter.Instance.DB.Categories.Where(x => x.idCategory == SelectedItem.idCategory).SingleOrDefault();

                if (DataAdapter.Instance.DB.Books.Where(b => b.idCategory == category.idCategory).Count() > 0) {
                    MessageBox.Show("Không thể xóa thể loại do thể loại còn được tham chiếu trong sách");
                }
                else
                {
                    DataAdapter.Instance.DB.Categories.Remove(category);
                    DataAdapter.Instance.DB.SaveChanges();
                    ListCategory.Remove(category);
                    MessageBox.Show("Xóa thể loại thành công");
                }
            });
        }
    }
}
