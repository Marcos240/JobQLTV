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
    public class ReaderNameValidation : ValidationRule
    {
        private readonly string NAME_PARTTERN= "^[a-zA-Z_ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶ" +
            "ẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợ" +
            "ụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\\s]+$";
        
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                string name = (string)value;
                // Check empty string?
                if (value == null || name.Trim().Length == 0)
                {
                    return new ValidationResult(false, "Vui lòng nhập họ tên");
                }
                // No. character must be greater than 4.
                if (name.Length < 4) // Le Y
                {
                    return new ValidationResult(false, "Họ tên phải có ít nhất 4 kí tự");
                }
                // Regular expression check name pattern
                if (!Regex.IsMatch(name, NAME_PARTTERN)) {
                    return new ValidationResult(false, "Họ tên chỉ có thể gồm các kí tự từ a-z, dấu cách");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Họ tên độc giả không hợp lệ");
            }
            return ValidationResult.ValidResult;
        }
    }
}
