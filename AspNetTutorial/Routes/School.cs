namespace AspNetTutorial.Routes;

using Dtos;
using Entities;
using Services;

public static class School {
	public static void MapSchoolRoutes(this IEndpointRouteBuilder app, string tag) {
		RouteGroupBuilder route = app.MapGroup("api/v1/School/");

		route.MapPost("School/Create", async (ISchoolService service, SchoolEntity dto) => {
			SchoolEntity result = await service.Create(dto);
			return Results.Ok(result);
		}).WithTags(tag);

		route.MapGet("School/Read", async (ISchoolService service) => {
			IEnumerable<SchoolEntity> result = await service.Read();
			return Results.Ok(result);
		}).WithTags(tag);

		route.MapGet("School/Read/{id:guid}", async (ISchoolService service, Guid id) => {
			SchoolEntity? result = await service.ReadById(id);
			return result == null ? Results.NotFound() : Results.Ok(result);
		}).WithTags(tag);

		route.MapPut("School/Update", async (ISchoolService service, SchoolEntity param) => {
			SchoolEntity? result = await service.Update(param);
			return result == null ? Results.NotFound() : Results.Ok(result);
		}).WithTags(tag);

		route.MapDelete("School/Delete", async (ISchoolService service, Guid id) => {
			await service.Delete(id);
			return Results.Ok();
		}).WithTags(tag);
	}
}