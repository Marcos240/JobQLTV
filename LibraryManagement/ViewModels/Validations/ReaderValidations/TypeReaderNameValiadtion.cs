using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class TypeReaderNameValiadtion : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (value != null || string.IsNullOrEmpty((string)value) == true)
                    return new ValidationResult(false, "Vui lòng nhập tên loại độc giả mới");
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Tên loại độc giả không hợp lệ");
            }
            return ValidationResult.ValidResult;
        }
    }
}
