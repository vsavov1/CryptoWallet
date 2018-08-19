using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Wallet.Presentation.Validators
{
    public class CreateNewAccountRule : ValidationRule
    {
        private int _minLenght = 8;

        public CreateNewAccountRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if ((value as string).Length < _minLenght)
            {
                return new ValidationResult(false, "Length should be greater than 8 chars.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
