using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LibraryManagement.ViewModels
{
    class ReaderDOBValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                DateTime date = (DateTime)value;
                int minAgeAllowed = DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 1).SingleOrDefault().valueParameter;
                int maxAgeAllowed = DataAdapter.Instance.DB.Paramaters.Where(x => x.idParameter == 2).SingleOrDefault().valueParameter;
                if (DateTime.Now.AddYears(-minAgeAllowed) < date        // date > lastestDate ~ age < minAge 
                    || date < DateTime.Now.AddYears(-maxAgeAllowed))    // date < oldestDate ~ age > maxAge
                {
                    return new ValidationResult(false, $"Độ tuổi cho phép là từ {minAgeAllowed} - {maxAgeAllowed}");
                }
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Ngày sinh không hợp lệ");
            }

            return ValidationResult.ValidResult;
        }
    }
}
