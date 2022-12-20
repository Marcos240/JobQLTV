using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class DateBorrowMaxValidation : ValidationRule
    {
        public DateBorrowMaxValidation()
        {

        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int dateBorrowMax = 0;

            try
            {
                if (((string)value).Length > 0)
                    dateBorrowMax = Int32.Parse((String)value);
                else
                {
                    return new ValidationResult(false, "Vui lòng nhập số ngày mượn tối đa");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Số ngày mượng phải là số nguyên dương nhỏ hơn 2^31");
            }

            if (dateBorrowMax <= 0)
            {
                return new ValidationResult(false, "Số ngày mượn tối đa phải là số nguyên dương");
            }
            return ValidationResult.ValidResult;
        }
    }
}
