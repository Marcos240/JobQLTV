using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class CategoryNameValidation : ValidationRule
    {
        public CategoryNameValidation()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (value == null || ((string)value).Trim().Length == 0)
                    return new ValidationResult(false, "Vui lòng nhập tên thể loại sách mới");
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Tên thể loại mới không hợp lệ!");
            }
            return ValidationResult.ValidResult;
        }
    }
}
