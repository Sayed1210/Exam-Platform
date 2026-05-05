namespace Exam.Models;

using System.ComponentModel.DataAnnotations;

public record ResetPasswordRequest(
    [property: Required]
    string Token,

    [property: Required]
    [property: MinLength(8)]
    string NewPassword);
