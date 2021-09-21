using System.ComponentModel.DataAnnotations;

namespace GeminiIssueProducer
{
    class OptionValidationAttribute : ValidationAttribute
    {
        public OptionValidationAttribute()
            : base("The value {0} must be specified")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null || (!GeminiIssueProducerOptionsParser.TryParse(value.ToString(), out GeminiIssueProducerOptions option)))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
