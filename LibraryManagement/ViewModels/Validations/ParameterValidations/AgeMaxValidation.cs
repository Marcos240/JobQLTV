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
    public class AgeMaxValidationWrapper : DependencyObject
    {
        public static readonly DependencyProperty minValueProperty =
             DependencyProperty.Register("MinValue", typeof(int),
             typeof(AgeMaxValidationWrapper), new FrameworkPropertyMetadata(int.MaxValue));

        public int MinValue
        {
            get { return (int)GetValue(minValueProperty); }
            set { SetValue(minValueProperty, value); }
        }
    }

    public class AgeMaxValidation : ValidationRule
    {
        public AgeMaxValidationWrapper Wrapper { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int ageMax = 0;

            try
            {
                if (((string)value).Length > 0)
                    ageMax = Int32.Parse((String)value);
                else
                {
                    return new ValidationResult(false, "Vui lòng nhập số tuổi độc giả tối đa");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Số tuổi phải là số nguyên dương nhỏ hơn 2^31");
            }

            if (ageMax <= this.Wrapper.MinValue)
            {
                return new ValidationResult(false, "Số tuổi tối đa phải lớn hơn số tuổi tối thiểu");
            }

            if (ageMax <= 0)
            {
                return new ValidationResult(false, "Số tuổi độc giả tối thiểu phải là số nguyên dương");
            }
            return ValidationResult.ValidResult;
        }
    }
}
