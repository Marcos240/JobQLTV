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
    public class CollectedAmountValidationWrapper : DependencyObject
    {
        public static readonly DependencyProperty maxAmountProperty =
             DependencyProperty.Register("MaxAmount", typeof(int),
             typeof(CollectedAmountValidationWrapper), new FrameworkPropertyMetadata(int.MaxValue));

        public int MaxAmount
        {
            get { return (int)GetValue(maxAmountProperty); }
            set { SetValue(maxAmountProperty, value); }
        }
    }

    public class CollectedAmountValidation : ValidationRule
    {
        public CollectedAmountValidationWrapper Wrapper { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int collectedAmount = 0;

            try
            {
                if (((string)value).Length > 0)
                    collectedAmount = Int32.Parse((String)value);
                if (collectedAmount > this.Wrapper.MaxAmount)
                {
                    return new ValidationResult(false, "Số tiền thu không được vượt quá tổng nợ");
                }
                if (collectedAmount % 1000 != 0)
                {
                    return new ValidationResult(false, "Số tiền thu phải là bội của 1000đ");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Số tiền thu phải là số nguyên dương nhỏ hơn 2^31");
            }

            if (collectedAmount <= 0)
            {
                return new ValidationResult(false, "Số tiền thu phải lớn hơn 0đ và nhỏ hơn tổng nợ");
            }
            return ValidationResult.ValidResult;
        }
    }


}
