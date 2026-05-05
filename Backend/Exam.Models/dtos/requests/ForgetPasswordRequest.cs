namespace Exam.Models;

using System.ComponentModel.DataAnnotations;

public record ForgetPasswordRequest(
    [property: Required]
    [property: EmailAddress]
    string Email);
