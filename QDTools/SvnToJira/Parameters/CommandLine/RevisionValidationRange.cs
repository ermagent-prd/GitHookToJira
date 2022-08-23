using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SvnToJira.Parameters.CommandLine
{
    internal class RevisionValidationRange : ValidationAttribute
    {
        public RevisionValidationRange()
            : base("The value for {0} must be a positive number")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null || (value is string str && (str.StartsWith("-"))))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
