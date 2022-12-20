using LibraryManagement.Models;
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
    public class UserNameValidation : ValidationRule
    {
        private readonly string USERNAME_PATTERN = "^(?=.{5,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$";
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string username = (string)value;
            if (value == null || username == "")
            {
                return new ValidationResult(false, "Vui lòng nhập tên tài khoản");
            }
            else if(!Regex.IsMatch(username, USERNAME_PATTERN))
            {
                return new ValidationResult(false, "Tên tài khoản gồm 5-20 kí tự, chỉ gồm các kí tự a-z, 0-9 và _, . ở giữa");
            }
            else
            {
                try
                {
                    var staffCount = DataAdapter.Instance.DB.Staffs.Where(st => st.accountStaff == username).Count();
                    if (staffCount != 0)
                    {
                        return new ValidationResult(false, "Tên tài khoản đã tồn tại");
                    }
                }
                catch (Exception)
                {
                    return new ValidationResult(false, "Tên tài khoản không hợp lệ");
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
