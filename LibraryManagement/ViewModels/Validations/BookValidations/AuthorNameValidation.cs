using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class AuthorNameValidation : ValidationRule
    {
        public AuthorNameValidation()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || ((string)value).Trim().Length == 0)
                return new ValidationResult(false, "Vui lòng nhập nhập tên tác giả mới");
            return ValidationResult.ValidResult;
        }
    }
}
