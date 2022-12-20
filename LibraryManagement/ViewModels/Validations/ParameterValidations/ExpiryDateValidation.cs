using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class ExpiryDateValidation : ValidationRule
    {
        public ExpiryDateValidation()
        {

        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int expiryDate = 0;

            try
            {
                if (((string)value).Length > 0)
                    expiryDate = Int32.Parse((String)value);
                else
                {
                    return new ValidationResult(false, "Vui lòng nhập thời hạn hết hạn thẻ");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Thời hạn thẻ phải là số nguyên dương nhỏ hơn 2^31");
            }

            if (expiryDate <= 0)
            {
                return new ValidationResult(false, "Thời hạn hết hạn thẻ phải là một số nguyên dương");
            }
            return ValidationResult.ValidResult;
        }
    }
}
