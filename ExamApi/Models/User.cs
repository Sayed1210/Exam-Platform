namespace ExamApi.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
// using System.ComponentModel.DataAnnotations.Schema;

public enum UserRole { Admin }

[Index(nameof(Email), IsUnique = true)]
public class User

{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MaxLength(255)]
     public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]

    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Admin;
}