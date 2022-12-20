using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class DistancePublishValidation : ValidationRule
    {
        public DistancePublishValidation()
        {

        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int distancePublish = 0;

            try
            {
                if (((string)value).Length > 0)
                    distancePublish = Int32.Parse((String)value);
                else
                {
                    return new ValidationResult(false, "Vui lòng nhập khoảng cách năm xuất bản cho phép");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Khoảng cách năm phải là số nguyên dương nhỏ hơn 2^31");
            }

            if (distancePublish <= 0)
            {
                return new ValidationResult(false, "Khoảng cách năm xuất bản cho phép phải là số nguyên dương");
            }
            return ValidationResult.ValidResult;
        }
    }
}
