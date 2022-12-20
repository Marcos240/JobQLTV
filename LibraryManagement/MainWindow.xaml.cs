using LibraryManagement.Views;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibraryManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void hideNavigation()
        {
            if (ButtonCloseMenu.Visibility == Visibility.Visible)
            {
                ButtonCloseMenu.Visibility = Visibility.Collapsed;
                ButtonOpenMenu.Visibility = Visibility.Visible;
                lbName.Visibility = Visibility.Hidden;
                lbPermission.Visibility = Visibility.Hidden;
                btnLogout.Visibility = Visibility.Hidden;
                btnChangePassword.Visibility = Visibility.Hidden;
                Storyboard sb = this.FindResource("CloseMenu") as Storyboard;
                sb.Begin();
            }
        }
        private void loadSizeSomeControls()
        {
            gridForNavigation.Width = gridMain.ActualWidth / 20;
            ButtonOpenMenu.Width = gridForNavigation.Width;
            ButtonOpenMenu.Height = gridMain.ActualHeight / 15;
            ButtonCloseMenu.Width = gridForNavigation.Width;
            ButtonCloseMenu.Height = gridMain.ActualHeight / 15;
            EasingDoubleKeyFrame keyFrameOpenMenuToOpen = ((this.Resources["OpenMenu"] as Storyboard).Children[0]
                as DoubleAnimationUsingKeyFrames).KeyFrames[1] as EasingDoubleKeyFrame;
            keyFrameOpenMenuToOpen.Value = gridMain.ActualWidth / 5;
            EasingDoubleKeyFrame keyFrameOpenMenuToClose = ((this.Resources["OpenMenu"] as Storyboard).Children[0]
                as DoubleAnimationUsingKeyFrames).KeyFrames[0] as EasingDoubleKeyFrame;
            keyFrameOpenMenuToClose.Value = gridMain.ActualWidth / 20;
            EasingDoubleKeyFrame keyFrameCloseMenuToOpen = ((this.Resources["CloseMenu"] as Storyboard).Children[0]
                as DoubleAnimationUsingKeyFrames).KeyFrames[0] as EasingDoubleKeyFrame;
            keyFrameCloseMenuToOpen.Value = gridMain.ActualWidth / 5;
            EasingDoubleKeyFrame keyFrameCloseMenuToClose = ((this.Resources["CloseMenu"] as Storyboard).Children[0]
                as DoubleAnimationUsingKeyFrames).KeyFrames[1] as EasingDoubleKeyFrame;
            keyFrameCloseMenuToClose.Value = gridMain.ActualWidth / 20;
            lbName.Visibility = Visibility.Hidden;
            lbPermission.Visibility = Visibility.Hidden;
            btnLogout.Visibility = Visibility.Hidden;
            btnChangePassword.Visibility = Visibility.Hidden;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mr = MessageBox.Show("Are you sure exit application?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mr == MessageBoxResult.Yes)
            {
                this.Close();
                Application.Current.MainWindow.Close();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            lbName.Visibility = Visibility.Visible;
            lbPermission.Visibility = Visibility.Visible;
            btnLogout.Visibility = Visibility.Visible;
            btnChangePassword.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
            lbName.Visibility = Visibility.Hidden;
            lbPermission.Visibility = Visibility.Hidden;
            btnLogout.Visibility = Visibility.Hidden;
            btnChangePassword.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadSizeSomeControls();
            lbName.Content = "Hello, " + lbName.Content;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl urc = null;
            gridForContent.Children.Clear();
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "ItemHome":
                    this.lbTitle.Content = "QUẢN LÝ THƯ VIỆN";
                    urc = new HomeScreen();
                    gridForContent.Children.Add(urc);
                    hideNavigation();
                    break;
                case "ItemMember":
                    this.lbTitle.Content = "ĐỘC GIẢ";
                    urc = new ReaderScreen();
                    gridForContent.Children.Add(urc);
                    hideNavigation();
                    break;
                case "ItemBook":
                    this.lbTitle.Content = "SÁCH";
                    urc = new BookScreen();
                    gridForContent.Children.Add(urc);
                    hideNavigation();
                    break;
                case "ItemReport":
                    this.lbTitle.Content = "BÁO CÁO THỐNG KÊ";
                    urc = new ReportScreen();
                    gridForContent.Children.Add(urc);
                    hideNavigation();
                    break;
                case "ItemRegulation":
                    this.lbTitle.Content = "QUY ĐỊNH";
                    urc = new RegulationScreen();
                    gridForContent.Children.Add(urc);
                    hideNavigation();
                    break;
                case "ItemStaff":
                    this.lbTitle.Content = "NHÂN VIÊN";
                    urc = new StaffScreen();
                    gridForContent.Children.Add(urc);
                    hideNavigation();
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Visibility = Visibility.Visible;
        }

        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            hideNavigation();
        }
    }
}
