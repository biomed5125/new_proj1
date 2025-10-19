using FluentValidation.Results;

namespace HMS.Module.Lab.Features.Lab.Validations
{   

    public static class FluentValidationExtensions
    {
        public static IDictionary<string, string[]> ToDictionary(this ValidationResult result) =>
            result.Errors
                  .GroupBy(e => e.PropertyName)
                  .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
    }

}
