using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class ReaderAddressValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                string email = (string)value;
                // Check empty string?
                if (value == null || email.Trim().Length == 0)
                {
                    return new ValidationResult(false, "Vui lòng nhập địa chỉ");
                }
                // No. character must be equal or greater than 10.
                if (email.Length < 6)
                {
                    return new ValidationResult(false, "Địa chỉ phải có ít nhất 6 kí tự");
                }

            }
            catch (Exception)
            {
                return new ValidationResult(false, "Địa chỉ không hợp lệ");
            }
            return ValidationResult.ValidResult;
        }
    }
}