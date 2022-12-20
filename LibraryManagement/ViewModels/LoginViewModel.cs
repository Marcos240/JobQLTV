using LibraryManagement.Models;
using LibraryManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LibraryManagement.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        private ObservableCollection<Staff> _List;
        public ObservableCollection<Staff> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<Permission> _Permission;
        public ObservableCollection<Permission> Permission { get => _Permission; set { _Permission = value; OnPropertyChanged(); } }

        public Window main;

        private Staff _currentStaff;
        public Staff CurrentStaff { get => _currentStaff; set { _currentStaff = value; OnPropertyChanged(); } }

        private int _idStaff;
        public int IdStaff { get => _idStaff; set { _idStaff = value; OnPropertyChanged(); } }

        private string _accountStaff;
        public string AccountStaff
        {
            get => _accountStaff;
            set
            {
                _accountStaff = value; OnPropertyChanged();
            }
        }

        private string _passwordStaff;
        public string PasswordStaff
        {
            get => _passwordStaff;
            set
            {
                _passwordStaff = value; OnPropertyChanged();
            }
        }

        private string _newPasswordStaff;
        public string NewPasswordStaff
        {
            get => _newPasswordStaff;
            set
            {
                _newPasswordStaff = value; OnPropertyChanged();
            }
        }

        private string _confirmPasswordStaff;
        public string ConfirmPasswordStaff
        {
            get => _confirmPasswordStaff;
            set
            {
                _confirmPasswordStaff = value; OnPropertyChanged();
            }
        }

        private string _namePermission;
        public string NamePermission
        {
            get => _namePermission;
            set
            {
                _namePermission = value;
                OnPropertyChanged();
            }
        }

        private int _idPermission;
        public int IdPermission
        {
            get => _idPermission;
            set
            {
                _idPermission = value;
                OnPropertyChanged();
            }
        }

        private string _nameStaff;
        public string NameStaff
        {
            get => _nameStaff;
            set
            {
                _nameStaff = value; 
                OnPropertyChanged();
            }
        }

        private bool _canChangeRegualtion;
        public bool CanChangeRegulation
        {
            get => _canChangeRegualtion;
            set
            {
                _canChangeRegualtion = value;
                OnPropertyChanged();
            }
        }

        private bool _canChangePermission;
        public bool CanChangePermission
        {
            get => _canChangePermission;
            set
            {
                _canChangePermission = value;
                OnPropertyChanged();
            }
        }
        public AppCommand<object> EditPasswordCommand { get; }
        public AppCommand<object> ChangePasswordCommand { get; }
        public AppCommand<object> LogoutCommand { get; }
        public AppCommand<object> LoginCommand { get; }
        public LoginViewModel()
        {
            RetrieveData();
            AccountStaff = "admin";
            PasswordStaff= "abcd1234";
            LoginCommand = new AppCommand<object>((p) =>
            {
                if (PasswordStaff == null || AccountStaff == null)
                    return false;
                return true;

            }, (p) =>
            {
                
                for (int i = 0; i < List.Count; i++)
                {
                    string username = List[i].accountStaff;
                    string password = List[i].passwordStaff;
                    if (username == AccountStaff && password == EncryptSHA512Managed(PasswordStaff))
                    {
                        CurrentStaff = List[i];
                        NameStaff = CurrentStaff.nameStaff;
                        IdPermission = CurrentStaff.idPermission;
                        CanChangePermission = (IdPermission == 1);
                        CanChangeRegulation = (IdPermission == 1);
                        for (int j = 0; j < Permission.Count; j++)
                            if (Permission[j].idPermission == IdPermission)
                                NamePermission = Permission[j].namePermission;
                        break;
                    }
                    else CurrentStaff = null;
                }    
                if (CurrentStaff == null)
                {
                    MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    main = new MainWindow();
                    Application.Current.MainWindow.Visibility = Visibility.Hidden;
                    main.ShowDialog();
                }
            });

            ChangePasswordCommand = new AppCommand<object>((p) =>
            {
                if (CurrentStaff == null)
                    return false;
                return true;
            }, (p) =>
            {
                Window changePasswordWindow = new ChangePassword();
                changePasswordWindow.ShowDialog();
            });

            LogoutCommand = new AppCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                main.Close();
                RetrieveData();
            });

            EditPasswordCommand = new AppCommand<object>((p) =>
            {
                if (CurrentStaff == null || PasswordStaff == null || NewPasswordStaff == null || ConfirmPasswordStaff == null)
                    return false;
                return true;

            }, (p) =>
            {

                var Staff = DataAdapter.Instance.DB.Staffs.Where(x => x.idStaff == CurrentStaff.idStaff).SingleOrDefault();
                if (EncryptSHA512Managed(PasswordStaff) != CurrentStaff.passwordStaff)
                {
                    MessageBox.Show("Mật khẩu cũ không chính xác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else if (NewPasswordStaff != ConfirmPasswordStaff)
                {
                    MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    Staff.passwordStaff = EncryptSHA512Managed(NewPasswordStaff);
                    DataAdapter.Instance.DB.Staffs.AddOrUpdate(Staff);
                    DataAdapter.Instance.DB.SaveChanges();
                    MessageBox.Show("Bạn đã đổi mật khẩu thành công");
                }
            });
        }

        public string EncryptSHA512Managed(string password)
        {
            UnicodeEncoding uEncode = new UnicodeEncoding();
            byte[] bytPassword = uEncode.GetBytes(password);
            SHA512Managed sha = new SHA512Managed();
            byte[] hash = sha.ComputeHash(bytPassword);
            return Convert.ToBase64String(hash);
        }
        private void RetrieveData()
        {
            List = new ObservableCollection<Staff>(DataAdapter.Instance.DB.Staffs);
            Permission = new ObservableCollection<Permission>(DataAdapter.Instance.DB.Permissions);
        }
    }
}
