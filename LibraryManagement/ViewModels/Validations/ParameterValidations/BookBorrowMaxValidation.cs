using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class BookBorrowMaxValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int bookBorrowMax = 0;

            try
            {
                if (((string)value).Length > 0)
                    bookBorrowMax = Int32.Parse((String)value);
                else
                {
                    return new ValidationResult(false, "Vui lòng nhập số sách mượn tối đa");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Số sách mượn phải là số nguyên dương nhỏ hơn 2^31");
            }

            if (bookBorrowMax <= 0)
            {
                return new ValidationResult(false, "Số sách mượn tối đa phải là số nguyên dương");
            }
            return ValidationResult.ValidResult;
        }
    }
}
