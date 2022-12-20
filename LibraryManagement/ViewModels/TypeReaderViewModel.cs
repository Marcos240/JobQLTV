using LibraryManagement.Models;
using LibraryManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Data.Entity.Migrations;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;

namespace LibraryManagement.ViewModels
{
    class TypeReaderViewModel: BaseViewModel
    {
        private ObservableCollection<TypeReader> _ListTypeReader;
        public ObservableCollection<TypeReader> ListTypeReader { get => _ListTypeReader; set { _ListTypeReader = value; OnPropertyChanged(); } }
        private TypeReader _SelectedItemTypeReader;
        public TypeReader SelectedItemTypeReader
        {
            get => _SelectedItemTypeReader;
            set
            {
                _SelectedItemTypeReader = value;
                OnPropertyChanged();
                if (SelectedItemTypeReader != null)
                {
                    IdTypeReader = SelectedItemTypeReader.idTypeReader;
                    NameTypeReader = SelectedItemTypeReader.nameTypeReader;
                    NameAddTypeReader = null;
                }
            }
        }
        private int _idTypeReader;
        public int IdTypeReader { get => _idTypeReader; set { _idTypeReader = value; OnPropertyChanged(); } }

        private string _nameTypeReader;
        public string NameTypeReader
        {
            get => _nameTypeReader;
            set
            {
                _nameTypeReader = value; OnPropertyChanged();
            }
        }
        private string _nameAddTypeReader;
        public string NameAddTypeReader
        {
            get => _nameAddTypeReader;
            set
            {
                _nameAddTypeReader = value; OnPropertyChanged();
            }
        }
        public AppCommand<object> AddTypeReaderCommand { get; set; }
        public AppCommand<object> DeleteTypeReaderCommand { get; }
        public TypeReaderViewModel()
        {
            // Retrieve data from DB
            RetrieveData();
            
            AddTypeReaderCommand = new AppCommand<object>((p) =>
            {
                if (NameAddTypeReader == null || NameAddTypeReader == "")
                    return false;
                 return true;
            }, (p) =>
            {
                var displayList = DataAdapter.Instance.DB.TypeReaders.Where(x => x.nameTypeReader.ToLower() == NameAddTypeReader.ToLower());
                if (displayList.Count() != 0)
                {
                    MessageBox.Show("Loại tác giả bị trùng");
                    NameAddTypeReader = null;
                    return;
                }
                var tp = new TypeReader()
                {
                    nameTypeReader = NameAddTypeReader
                };
                DataAdapter.Instance.DB.TypeReaders.Add(tp);
                DataAdapter.Instance.DB.SaveChanges();
                ListTypeReader.Add(tp);
                MessageBox.Show("Bạn đã thêm loại độc giả thành công");
                NameAddTypeReader = null;

        });
            DeleteTypeReaderCommand = new AppCommand<object>((p) =>
            {
                if (SelectedItemTypeReader == null)
                    return false;

                var displayList = DataAdapter.Instance.DB.TypeReaders.Where(x => x.idTypeReader == SelectedItemTypeReader.idTypeReader);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;

            }, (p) =>
            {
                var type = DataAdapter.Instance.DB.TypeReaders.Where(x => x.idTypeReader == SelectedItemTypeReader.idTypeReader).SingleOrDefault();
                if (DataAdapter.Instance.DB.Readers.Where(el => el.idTypeReader == type.idTypeReader).Count() > 0)
                {
                    MessageBox.Show("Không thể xóa loại độc giả do còn độc giả tham chiếu!");
                    return;
                }
                DataAdapter.Instance.DB.TypeReaders.Remove(type);
                DataAdapter.Instance.DB.SaveChanges();
                ListTypeReader.Remove(type);
                MessageBox.Show("Bạn đã xóa loại độc giả thành công");
            });
        }

        private void RetrieveData()
        {
            ListTypeReader = new ObservableCollection<TypeReader>(DataAdapter.Instance.DB.TypeReaders);
        }
    }
}
