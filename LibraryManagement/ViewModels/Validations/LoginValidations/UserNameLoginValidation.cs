using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    class UserNameLoginValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string username = (string)value;
            if (value == null || username == "")
            {
                return new ValidationResult(false, "Vui lòng nhập tài khoản");
            }
            return ValidationResult.ValidResult;
        }
    }
}
