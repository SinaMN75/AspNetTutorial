namespace AspNetTutorial.Dtos;

public class ClassCreateDto {
	public required string Title { get; set; }

	public required string Subject { get; set; }

	public Guid? SchoolId { get; set; }

	public IEnumerable<Guid> Users { get; set; }
}