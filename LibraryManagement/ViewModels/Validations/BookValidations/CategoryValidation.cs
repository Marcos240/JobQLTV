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
    public class CategoryValidation : ValidationRule
    {
        public CategoryValidation()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "Vui lòng chọn thể loại sách");
            return ValidationResult.ValidResult;
        }
    }
}
