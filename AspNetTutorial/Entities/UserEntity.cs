namespace AspNetTutorial.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Users")]
public class UserEntity {
	[Key]
	public Guid Id { get; set; }

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
	
	public required string Password { get; set; }

	public DateTime? Birthdate { get; set; }

	public bool IsMarried { get; set; } = false;


	public List<ClassEntity> Classes { get; set; }
}