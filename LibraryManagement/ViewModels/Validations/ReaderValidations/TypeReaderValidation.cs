using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class TypeReaderValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Check empty string?
            if (value == null)
            {
                return new ValidationResult(false, "Vui lòng chọn loại độc giả");
            }
            return ValidationResult.ValidResult;
        }
    }
}
