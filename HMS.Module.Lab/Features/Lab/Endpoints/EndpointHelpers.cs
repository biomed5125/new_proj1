using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Module.Lab.Features.Lab.Endpoints
{
    // helper
    static class EndpointHelpers
    {
        public static IResult AsResult(this Task<bool> task)
            => task.Result ? Results.Ok() : Results.BadRequest();

        public static Dictionary<string, string[]> ToDictionary(this FluentValidation.Results.ValidationResult v, string key = "")
            => v.Errors.GroupBy(e => string.IsNullOrEmpty(key) ? e.PropertyName : key)
                       .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
    }
}
