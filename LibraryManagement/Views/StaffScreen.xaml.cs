using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryManagement.Views
{
    /// <summary>
    /// Interaction logic for StaffSc.xaml
    /// </summary>
    public partial class StaffScreen : UserControl
    {
        public StaffScreen()
        {
            InitializeComponent();
        }

        private void btnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Command.CanExecute(null))
            {
                button.Command.Execute(null);
            }
            AddStaff wd = new AddStaff();
            wd.ShowDialog();
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            btnSua.Visibility = Visibility.Hidden;
            btnLuu.Visibility = Visibility.Visible;
            NameStaff.IsReadOnly = false;
            Address.IsReadOnly = false;
            PhoneNumber.IsReadOnly = false;
            DobStaff.IsEnabled = true;
            UserName.IsReadOnly = false;
            cbPermission.IsEnabled = true;
            btnAddStaff.IsEnabled = false;
            btnDeleStaff.IsEnabled = false;
            SearchBox.IsEnabled = false;
            lvNhanVien.IsEnabled = false;
            btnResetPassword.IsEnabled = false;
            btnCancel.Visibility = Visibility.Visible;
        }

        private void btnLuu_Click(object sender, RoutedEventArgs e)
        {
            btnLuu.Visibility = Visibility.Hidden;
            btnSua.Visibility = Visibility.Visible;
            NameStaff.IsReadOnly = true;
            Address.IsReadOnly = true;
            PhoneNumber.IsReadOnly = true;
            DobStaff.IsEnabled = false;
            UserName.IsReadOnly = true;
            cbPermission.IsEnabled = false;
            btnAddStaff.IsEnabled = true;
            btnDeleStaff.IsEnabled = true;
            SearchBox.IsEnabled = true;
            lvNhanVien.IsEnabled = true;
            btnResetPassword.IsEnabled = true;
            btnCancel.Visibility = Visibility.Hidden;
        }

        private void lvNhanVien_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ListBoxItem))
                lvNhanVien.UnselectAll();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnLuu.Visibility = Visibility.Hidden;
            btnSua.Visibility = Visibility.Visible;
            NameStaff.IsReadOnly = true;
            Address.IsReadOnly = true;
            PhoneNumber.IsReadOnly = true;
            DobStaff.IsEnabled = false;
            UserName.IsReadOnly = true;
            cbPermission.IsEnabled = false;
            btnAddStaff.IsEnabled = true;
            btnDeleStaff.IsEnabled = true;
            SearchBox.IsEnabled = true;
            lvNhanVien.IsEnabled = true;
            btnResetPassword.IsEnabled = true;
            btnCancel.Visibility = Visibility.Hidden;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}