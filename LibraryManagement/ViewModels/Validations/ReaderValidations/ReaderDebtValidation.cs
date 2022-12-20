using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class ReaderDebtValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int debt = 0;

            try
            {
                if (value == null)
                {
                    return ValidationResult.ValidResult;
                }
                if (((string)value).Length > 0)
                    debt = Int32.Parse((String)value);
                else
                {
                    return new ValidationResult(false, "Vui lòng nhập số nợ");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Số nợ phải là một số nguyên dương nhỏ hơn 2^31");
            }

            if (debt < 0)
            {
                return new ValidationResult(false, "Số nợ không thể âm");
            }
            return ValidationResult.ValidResult;
        }
    }
}
