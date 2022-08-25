using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnToJira.Parameters
{
    internal class OptionValidationAttribute : ValidationAttribute
    {
        public OptionValidationAttribute()
            : base("The value for {0} must be '0' (add comment to Jira issue) or '1' (Jira Tracking Issue validation)")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null || (value is string str && str != EngineOptionArgument.AddJiraComment && str != EngineOptionArgument.CheckJiraBugFix))
            {
                return new ValidationResult(FormatErrorMessage(context.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
