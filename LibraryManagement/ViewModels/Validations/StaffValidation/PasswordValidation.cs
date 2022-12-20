using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class PasswordValidation : ValidationRule
    {
        private readonly string PASSWORD_PATTERN = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{5,}$";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string password = (string)value;
            if (value == null || password == "")
            {
                return new ValidationResult(false, "Vui lòng nhập mật khẩu");
            }
            else if (!Regex.IsMatch(password, PASSWORD_PATTERN))
            {
                return new ValidationResult(false, "Mật khẩu chỉ gồm chữ và số, có ít nhất 5 kí tự gồm ít nhất 1 số, 1 chữ");
            }
            return ValidationResult.ValidResult;
        }
    }
}
