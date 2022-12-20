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
    public class PhoneNumberValidation : ValidationRule
    {
        private readonly string NUMBER_PATTERN = @"^-*[0-9,\.?\-?\(?\)?\ ]+$";

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string phone = (string)value;
            if (value == null || phone == "")
            {
                return new ValidationResult(false, "Vui lòng nhập số điện thoại");
            }
            else if (!Regex.IsMatch(phone, NUMBER_PATTERN))
            {
                return new ValidationResult(false, "Số điện thoại chỉ gồm các kí tự số");
            }
            else if(phone.Length < 9)
            {
                return new ValidationResult(false, "Số điện thoại phải có ít nhất 9 kí tự");
            }
            return ValidationResult.ValidResult;
        }
    }
}
