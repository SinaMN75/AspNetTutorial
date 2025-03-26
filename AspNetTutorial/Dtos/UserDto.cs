namespace AspNetTutorial.Dtos;

using Entities;
using System.ComponentModel.DataAnnotations;

public class UserCreateParams {
	[Required]
	[MinLength(2)]
	[MaxLength(100)]
	public required string FullName { get; set; }

	[EmailAddress]
	public required string Email { get; set; }

	[Required]
	[MaxLength(12)]
	[MinLength(10)]
	public required string PhoneNumber { get; set; }

	public DateTime? Birthdate { get; set; }

	public bool IsMarried { get; set; } = false;

	public required string Password { get; set; }
}

public class UserUpdateParams {
	[Required]
	public required Guid Id { get; set; }

	public string? FullName { get; set; }
	public string? Email { get; set; }

	public string? PhoneNumber { get; set; }
	public DateTime? Birthdate { get; set; }
	public bool? IsMarried { get; set; }
}

public class UserResponse {
	public required Guid Id { get; set; }
	public required string FullName { get; set; }
	public required string Email { get; set; }
	public required string PhoneNumber { get; set; }
	public DateTime? Birthdate { get; set; }
	public bool IsMarried { get; set; }
	public int? Age { get; set; }
	public List<ClassEntity>? Classes { get; set; }
}