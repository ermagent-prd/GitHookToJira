using QDToolsUtilities;
using System.ComponentModel.DataAnnotations;

namespace KpiEngine.Parameters
{
    internal class OptionValidationAttribute : ValidationAttribute
    {
        public OptionValidationAttribute()
            : base("The value for {0} must be one of these values: (....)")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var commandOption = CommandOptionEnumConverter.ToEnum<OptionType>(value.ToString());

            if (commandOption == default(OptionType))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
