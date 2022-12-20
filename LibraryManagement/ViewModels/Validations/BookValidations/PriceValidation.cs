using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class PriceValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int price = 0;

            try
            {
                if (((string)value).Length > 0)
                    price = Int32.Parse((String)value);
                else
                {
                    return new ValidationResult(false, "Vui lòng nhập giá");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Giá sách phải là một số nguyên dương");
            }

            if (price <= 0)
            {
                return new ValidationResult(false, "Giá sách phải là một số nguyên dương");
            }
            return ValidationResult.ValidResult;
        }
    }
}
