using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    public class MoneyPenaltyValidation : ValidationRule
    {
        public MoneyPenaltyValidation()
        {

        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int moneyPenalty = 0;

            try
            {
                if (((string)value).Length > 0)
                    moneyPenalty = Int32.Parse((String)value);
                else
                {
                    return new ValidationResult(false, "Vui lòng nhập mức phạt");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Mức phạt phải là số nguyên dương nhỏ hơn 2^31");
            }

            if (moneyPenalty < 0)
            {
                return new ValidationResult(false, "Mức phạt phải là một số nguyên dương");
            }
            return ValidationResult.ValidResult;
        }
    }
}
