using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Exam.Api;

public static class UploadEndpoints
{
    public static IEndpointRouteBuilder MapUploadEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/upload")
            .WithTags("Upload");

        group.MapPost("/", UploadImage)
            .WithName("UploadImage")
            .WithSummary("Upload image")
            .WithDescription("Uploads an image and returns its path.")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();

        return app;
    }

    private static async Task<IResult> UploadImage(
        HttpRequest httpRequest,
        IWebHostEnvironment env,
        CancellationToken cancellationToken)
    {
        var form = await httpRequest.ReadFormAsync(cancellationToken);

        var file = form.Files["file"];

        var validationProblem = Validate(file);

        if (validationProblem is not null)
        {
            return validationProblem;
        }

        var extension = Path
            .GetExtension(file!.FileName)
            .ToLowerInvariant();

        var uploadsFolder = Path.Combine(
            env.WebRootPath,
            "uploads"
        );

        Directory.CreateDirectory(uploadsFolder);

        var fileName =
            $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(
            uploadsFolder,
            fileName
        );

        await using var stream = new FileStream(
            filePath,
            FileMode.Create
        );

        await file.CopyToAsync(stream, cancellationToken);

        var imageUrl = $"/uploads/{fileName}";

        return Results.Ok(new
        {
            imageUrl
        });
    }

    private static IResult? Validate(IFormFile? file)
    {
        if (file is null)
        {
            return Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    {
                        "file",
                        ["Image file is required."]
                    }
                });
        }

        var errors = new Dictionary<string, string[]>();

        if (file.Length == 0)
        {
            errors["file"] = ["File cannot be empty."];
        }

        if (!file.ContentType.StartsWith("image/"))
        {
            errors["file"] = ["Only image files are allowed."];
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            errors["file"] = ["File size cannot exceed 5 MB."];
        }

        var allowedExtensions = new[]
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        var extension = Path
            .GetExtension(file.FileName)
            .ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            errors["file"] =
            [
                "Only .jpg, .jpeg, .png, and .webp files are allowed."
            ];
        }

        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        return null;
    }
}