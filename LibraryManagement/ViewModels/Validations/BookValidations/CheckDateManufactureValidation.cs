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
    public class CheckDateManufactureValidation : ValidationRule
    {
        public CheckDateManufactureValidation()
        {
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            DateTime temp;
            int checkDateManufacture = DataAdapter.Instance.DB.Paramaters.Find(4).valueParameter;
            try
            {
                temp = (DateTime)value;
                if((DateTime.Now - temp).Days > checkDateManufacture * 365)
                    return new ValidationResult(false, $"Chỉ chấp nhận sách được xuất bản trong vòng {checkDateManufacture} năm");
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Ngày tháng không hợp lệ");
            }
            return ValidationResult.ValidResult;
        }
    }
}
