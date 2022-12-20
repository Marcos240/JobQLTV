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
    public class AgeMinValidationWrapper : DependencyObject
    {
        public static readonly DependencyProperty maxValueProperty =
             DependencyProperty.Register("MaxValue", typeof(int),
             typeof(AgeMinValidationWrapper), new FrameworkPropertyMetadata(int.MaxValue));

        public int MaxValue
        {
            get { return (int)GetValue(maxValueProperty); }
            set { SetValue(maxValueProperty, value); }
        }
    }

    public class AgeMinValidation : ValidationRule
    {
        public AgeMinValidationWrapper Wrapper { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int ageMin = 0;

            try
            {
                if (((string)value).Length > 0)
                    ageMin = Int32.Parse((String)value);
                else
                {
                    return new ValidationResult(false, "Vui lòng nhập số tuổi độc giả tối thiểu");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Số tuổi phải là số nguyên dương nhỏ hơn 2^31");
            }

            if (ageMin >= this.Wrapper.MaxValue)
            {
                return new ValidationResult(false, "Số tuổi tối thiểu phải nhỏ hơn số tuổi tối đa");
            }

            if (ageMin <= 0)
            {
                return new ValidationResult(false, "Số tuổi độc giả tối thiểu phải là số nguyên dương");
            }
            return ValidationResult.ValidResult;
        }
    }
}
