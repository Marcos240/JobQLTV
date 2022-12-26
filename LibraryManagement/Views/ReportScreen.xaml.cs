using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace LibraryManagement.Views
{
    /// <summary>
    /// Interaction logic for ReportScreen.xaml
    /// </summary>
    /// 

    public partial class ReportScreen : UserControl
    {
        public ReportScreen()
        {
            InitializeComponent();
        }

        private void year_Loaded(object sender, RoutedEventArgs e)
        {
            year.Items.Clear();
            for (int i = 2018; i <= DateTime.Now.Year; i++)
            {
                year.Items.Add(i);
            }
            year.SelectedItem = DateTime.Today.Year.ToString();
        }

        private void month_Loaded(object sender, RoutedEventArgs e)
        {
            month.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                month.Items.Add(i);
            }
            month.SelectedItem = DateTime.Today.Month.ToString();
        }

        private void day_Loaded(object sender, RoutedEventArgs e)
        {
            Day.SelectedDate = DateTime.Today;
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ScrollViewer_PreviewMouseWheel_1(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void MetroTabItem_Loaded(object sender, RoutedEventArgs e)
        {
            btnReloadReport.Command.Execute(null);
        }

        private void searchMode_Loaded(object sender, RoutedEventArgs e)
        {
            searchMode.Items.Clear();
            searchMode.Items.Add("Theo ngày");
            searchMode.Items.Add("Theo tháng");
            searchMode.Items.Add("Theo quý");
            searchMode.Items.Add("Theo năm");
            searchMode.SelectedIndex = 0;
        }

        private void searchMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchMode.SelectedIndex == 0)
            {
                searchDay.IsEnabled = true;
                searchDayArea.Visibility = Visibility.Visible;
                searchMonth.IsEnabled = false;
                searchMonthArea.Visibility = Visibility.Hidden;
                searchQuater.IsEnabled = false;
                searchQuaterArea.Visibility = Visibility.Hidden;
                searchYear.IsEnabled = false;
                searchYearArea.Visibility = Visibility.Hidden;
            }
            else if (searchMode.SelectedIndex == 1)
            {
                searchDay.IsEnabled = false;
                searchDayArea.Visibility = Visibility.Hidden;
                searchMonth.IsEnabled = true;
                searchMonthArea.Visibility = Visibility.Visible;
                searchQuater.IsEnabled = false;
                searchQuaterArea.Visibility = Visibility.Hidden;
                searchYear.IsEnabled = false;
                searchYearArea.Visibility = Visibility.Hidden;
            }
            else if (searchMode.SelectedIndex == 2)
            {
                searchDay.IsEnabled = false;
                searchDayArea.Visibility = Visibility.Hidden;
                searchMonth.IsEnabled = false;
                searchMonthArea.Visibility = Visibility.Hidden;
                searchQuater.IsEnabled = true;
                searchQuaterArea.Visibility = Visibility.Visible;
                searchYear.IsEnabled = false;
                searchYearArea.Visibility = Visibility.Hidden;
            }
            else
            {
                searchDay.IsEnabled = false;
                searchDayArea.Visibility = Visibility.Hidden;
                searchMonth.IsEnabled = false;
                searchMonthArea.Visibility = Visibility.Hidden;
                searchQuater.IsEnabled = false;
                searchQuaterArea.Visibility = Visibility.Hidden;
                searchYear.IsEnabled = true;
                searchYearArea.Visibility = Visibility.Visible;
            }
        }

        private void searchMonth_Loaded(object sender, RoutedEventArgs e)
        {
            searchMonth.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                searchMonth.Items.Add(i);
            }
            //searchMonth.SelectedIndex = DateTime.Today.Month - 1;
        }

        private void searchDay_Loaded(object sender, RoutedEventArgs e)
        {
            searchDay.SelectedDate = DateTime.Now;
        }

        private void searchQuater_Loaded(object sender, RoutedEventArgs e)
        {
            searchQuater.Items.Clear();
            for (int i = 1; i <= 4; i++)
            {
                searchQuater.Items.Add(i);
            }
            //searchQuater.SelectedIndex = (DateTime.Today.Month + 2)/3 - 1;
        }

        private void searchYear_Loaded(object sender, RoutedEventArgs e)
        {
            searchYear.Items.Clear();
            for (int i = DateTime.Today.Year; i >= DateTime.Today.Year - 5; i--)
            {
                searchYear.Items.Add(i);
            }
            //searchYear.SelectedIndex = 0;
        }

        private void searchModeLate_Loaded(object sender, RoutedEventArgs e)
        {
            searchModeLate.Items.Clear();
            searchModeLate.Items.Add("Theo ngày");
            searchModeLate.Items.Add("Theo tháng");
            searchModeLate.Items.Add("Theo quý");
            searchModeLate.Items.Add("Theo năm");
            searchModeLate.SelectedIndex = 0;
        }

        private void searchModeLate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchModeLate.SelectedIndex == 0)
            {
                searchDayLate.IsEnabled = true;
                searchDayAreaLate.Visibility = Visibility.Visible;
                searchMonthLate.IsEnabled = false;
                searchMonthAreaLate.Visibility = Visibility.Hidden;
                searchQuaterLate.IsEnabled = false;
                searchQuaterAreaLate.Visibility = Visibility.Hidden;
                searchYearLate.IsEnabled = false;
                searchYearAreaLate.Visibility = Visibility.Hidden;
            }
            else if (searchModeLate.SelectedIndex == 1)
            {
                searchDayLate.IsEnabled = false;
                searchDayAreaLate.Visibility = Visibility.Hidden;
                searchMonthLate.IsEnabled = true;
                searchMonthAreaLate.Visibility = Visibility.Visible;
                searchQuaterLate.IsEnabled = false;
                searchQuaterAreaLate.Visibility = Visibility.Hidden;
                searchYearLate.IsEnabled = false;
                searchYearAreaLate.Visibility = Visibility.Hidden;
            }
            else if (searchModeLate.SelectedIndex == 2)
            {
                searchDayLate.IsEnabled = false;
                searchDayAreaLate.Visibility = Visibility.Hidden;
                searchMonthLate.IsEnabled = false;
                searchMonthAreaLate.Visibility = Visibility.Hidden;
                searchQuaterLate.IsEnabled = true;
                searchQuaterAreaLate.Visibility = Visibility.Visible;
                searchYearLate.IsEnabled = false;
                searchYearAreaLate.Visibility = Visibility.Hidden;
            }
            else
            {
                searchDayLate.IsEnabled = false;
                searchDayAreaLate.Visibility = Visibility.Hidden;
                searchMonthLate.IsEnabled = false;
                searchMonthAreaLate.Visibility = Visibility.Hidden;
                searchQuaterLate.IsEnabled = false;
                searchQuaterAreaLate.Visibility = Visibility.Hidden;
                searchYearLate.IsEnabled = true;
                searchYearAreaLate.Visibility = Visibility.Visible;
            }
        }

        private void searchMonthLate_Loaded(object sender, RoutedEventArgs e)
        {
            searchMonthLate.Items.Clear();
            for (int i = 1; i <= 12; i++)
            {
                searchMonthLate.Items.Add(i);
            }
            //searchMonth.SelectedIndex = DateTime.Today.Month - 1;
        }

        private void searchDayLate_Loaded(object sender, RoutedEventArgs e)
        {
            searchDayLate.SelectedDate = DateTime.Now;
        }

        private void searchQuaterLate_Loaded(object sender, RoutedEventArgs e)
        {
            searchQuaterLate.Items.Clear();
            for (int i = 1; i <= 4; i++)
            {
                searchQuaterLate.Items.Add(i);
            }
            //searchQuater.SelectedIndex = (DateTime.Today.Month + 2)/3 - 1;
        }

        private void searchYearLate_Loaded(object sender, RoutedEventArgs e)
        {
            searchYearLate.Items.Clear();
            for (int i = DateTime.Today.Year; i >= DateTime.Today.Year - 5; i--)
            {
                searchYearLate.Items.Add(i);
            }
            //searchYear.SelectedIndex = 0;
        }
    }
}
