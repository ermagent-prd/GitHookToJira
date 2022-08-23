using System;
using System.ComponentModel.DataAnnotations;

namespace SvnToJira.Parameters.CommandLine
{
    internal class RevisionValidationRange : ValidationAttribute
    {
        public RevisionValidationRange()
            : base("The value for {0} must be a positive integer number")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null)
                return new ValidationResult(String.Format("{0} non specified", context.DisplayName));

            int svnRev = -1;

            var parseResult = int.TryParse(value.ToString(), out svnRev);

            if (!parseResult || svnRev < 1)
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
