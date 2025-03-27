namespace AspNetTutorial.Routes;

using Dtos;
using Entities;
using Services;

public static class Class {
	public static void MapClassRoutes(this IEndpointRouteBuilder app, string tag) {
		app.MapPost("class/Create", async (IClassService service, ClassCreateDto dto) => {
			ClassEntity result = await service.Create(dto);
			return Results.Ok(result);
		}).WithTags(tag);

		app.MapGet("class/Read", async (IClassService service) => {
			IEnumerable<ClassEntity> result = await service.Read();
			return Results.Ok(result);
		}).WithTags(tag);

		app.MapGet("class/Read/{id:guid}", async (IClassService service, Guid id) => {
			ClassEntity? result = await service.ReadById(id);
			return result == null ? Results.NotFound() : Results.Ok(result);
		}).WithTags(tag);

		app.MapPut("class/Update", async (IClassService service, ClassEntity param) => {
			ClassEntity? result = await service.Update(param);
			return result == null ? Results.NotFound() : Results.Ok(result);
		}).WithTags(tag);

		app.MapDelete("class/Delete", async (IClassService service, Guid id) => {
			await service.Delete(id);
			return Results.Ok();
		}).WithTags(tag);
	}
}