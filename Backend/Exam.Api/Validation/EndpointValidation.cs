using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Exam.Api.Validation;

public static class EndpointValidation
{
    public static IResult? PositiveNumber(string fieldName, int value)
    {
        if (value > 0) return null;
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [fieldName] = [$"{fieldName} must be a positive number."]
        });
    }

    public static IResult? Request<TRequest>(TRequest request) where TRequest : class
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request);
        if (Validator.TryValidateObject(request, context, results, validateAllProperties: true))
            return null;

        var errors = results
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => r.ErrorMessage ?? "Invalid value.").ToArray());

        return Results.ValidationProblem(errors);
    }
}