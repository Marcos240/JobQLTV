using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class PasswordLoginValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string password = (string)value;
            if (value == null || password == "")
            {
                return new ValidationResult(false, "Vui lòng nhập mật khẩu");
            }
            return ValidationResult.ValidResult;
        }
    }
}
