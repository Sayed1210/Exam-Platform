using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

public enum UserRole { Admin }

public class User

{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(20, ErrorMessage = "Password cannot exceed 20 characters")]
     public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]

    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Admin;
}