using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class ComfirmPasswordValidationWrapper : DependencyObject
    {
        public static readonly DependencyProperty comparePasswordProperty =
             DependencyProperty.Register("ComparePassword", typeof(string),
             typeof(ComfirmPasswordValidationWrapper), new FrameworkPropertyMetadata());

        public string ComparePassword
        {
            get { return (string)GetValue(comparePasswordProperty); }
            set { SetValue(comparePasswordProperty, value); }
        }
    }
    public class ComfirmPasswordValidation : ValidationRule
    {
        public ComfirmPasswordValidationWrapper Wrapper { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            String password = Wrapper.ComparePassword;
            String comfirmPassword = (string)value;
            if (comfirmPassword == "")
            {
                return new ValidationResult(false, "Vui lòng nhập lại mật khẩu mới");
            }
            if (password != null && !password.Equals(comfirmPassword))
            {
                return new ValidationResult(false, "Mật khẩu mới không trùng khớp");
            }
            return ValidationResult.ValidResult;
        }
    }
}
