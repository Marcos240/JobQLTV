using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LibraryManagement.ViewModels
{

    /// <summary>
    /// Convert idpermisson for button Regulation UI control 
    /// </summary>
    class PermissonToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return false;
                int idPermission = Int32.Parse(value.ToString());
                if (idPermission == 1)
                {
                    return "Visible";
                }
                else
                {
                    return "Hidden";
                }
            }
            catch (Exception)
            {
                return "Hidden";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }

    }

        /// <summary>
        /// Convert idpermisson for textbox Regulation UI control 
        /// </summary>
        class PermissonToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return false;
                int idPermission = Int32.Parse(value.ToString());
                if (idPermission == 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }
    }

    /// <summary>
    /// Convert List to boolean (for paginating buttons' visibility)
    /// </summary>
    class ListToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return false;
                LatestBookPaginationCollection list = (LatestBookPaginationCollection)value;
                int button = Int32.Parse(paramater.ToString());
                if (button == 1)
                {
                    return list.CurrentPage > 1 ? "Visible" : "Hidden";
                }
                else
                {
                    return list.CurrentPage < list.PageCount ? "Visible" : "Hidden";
                }
            }
            catch (Exception)
            {
                return "Hidden";
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }
    }

    /// <summary>
    /// Convert selected item to boolean (for enable/disable context menu item)
    /// </summary>
    class SelectedItemToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null)
                return false;
            return true;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }
    }
    /// <summary>
    /// Convert Permission id to boolean (for checkbox checked property)
    /// </summary>
    class PerrmissionIDToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null || Int32.Parse(value.ToString()) == 0)
                return false;
            try
            {
                if (Int32.Parse(value.ToString()) == 1)
                {
                    return true;
                }
                else if (Int32.Parse(value.ToString()) == 2 && Int32.Parse(paramater.ToString()) == 1)
                {
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }
    }

    /// <summary>
    /// Convert the bill return to total amount of fine of the reader
    /// </summary>
    class ImageToSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null)
                return 0;
            string fileName = (string)value;

            string destPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            string destinationFile = Path.Combine($"{destPath}\\BookImageCover", fileName);
            return destinationFile;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }

    }

    /// <summary>
    /// Convert the bill return to total amount of fine of the reader
    /// </summary>
    class BillReturnToReaderDebt: IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null || DataAdapter.Instance.DB.Readers.Find(((BillReturn)value).idReader) == null)
                return 0;
            return ((BillReturn)value).sumFine + DataAdapter.Instance.DB.Readers.Find(((BillReturn)value).idReader).debt;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }

    }

    /// <summary>
    /// Convert the date a book was borrowed to the amount of fine. 
    /// </summary>
    class DateBorrowedToFine : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null)
                return value;
            int fine = 0;
            int noDaysBorrowAllowed = DataAdapter.Instance.DB.Paramaters.Find(7).valueParameter; // Get this from DB
            int amountFinePerDayExcess = DataAdapter.Instance.DB.Paramaters.Find(6).valueParameter; // Get this from DB
            int excessDays = DateTime.Now.Subtract((DateTime)value).Days - noDaysBorrowAllowed;
            if (excessDays > 0)
            {
                fine = amountFinePerDayExcess * excessDays;
            }
            return fine;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }

    }

    /// <summary>
    /// Convert the date a book was borrowed to the number of excess days. 
    /// </summary>
    class DateBorrowedToExcessDays : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null)
                return value;
            int noDaysBorrowAllowed = DataAdapter.Instance.DB.Paramaters.Find(7).valueParameter; // Get this from DB
            int excessDays = DateTime.Now.Subtract((DateTime)value).Days - noDaysBorrowAllowed;
            if (excessDays < 0)
            {
                excessDays = 0;
            }
            return excessDays;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }

    }

    /// <summary>
    /// Convert the date a book was borrowed to the number of days borrowed. 
    /// </summary>
    class DateBorrowedToDaysBorrowed : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null)
                return value;
            int noDaysBorrowes = DateTime.Now.Subtract((DateTime)value).Days;
            return noDaysBorrowes;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }

    }

    /// <summary>
    /// Convert the date a book was borrowed to the number of remaining days to be returned according to the rule. 
    /// </summary>
    class DateBorrowedToRemainingDay : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null)
                return value;
            int noDaysBorrowAllowed = DataAdapter.Instance.DB.Paramaters.Find(7).valueParameter; // Get this from DB
            int noRemainingDay = noDaysBorrowAllowed - DateTime.Now.Subtract((DateTime)value).Days;
            return noRemainingDay;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }

    }


    /// <summary>
    /// Concatenate authors collection into a string to display in a cell: 
    /// </summary>
    class AuthorsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null)
                return value;
            HashSet<Author> Authors = (HashSet<Author>)value;
            List<Author> list = Authors.ToList();
            string result = "";
            foreach (var item in list)
            {
                if (item != list.First())
                {
                    result += ", ";
                }
                result += item.nameAuthor;
            }
            return result;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }

    }

    /// <summary>
    /// Convert Boolean value: true/false to Visibility value: Visible/Hidden
    /// </summary>
    class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value == null || (bool)value == true)
                return "Visible";
            return "Hidden";
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MethodToValueConverter can only be used for one way conversion.");
        }
    }
}
