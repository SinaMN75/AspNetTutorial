namespace AspNetTutorial.Routes;

using Dtos;
using Entities;
using Services;

public static class User {
	public static void MapUserRoutes(this IEndpointRouteBuilder app, string tag) {
		app.MapPost("user/Create", async (IUserService userService, UserCreateParams dto, CancellationToken ct) => {
			UserResponse result = await userService.Create(dto, ct);
			return Results.Ok(result);
		}).WithTags(tag);

		app.MapGet("user/Read", async (IUserService userService, CancellationToken ct) => {
			IEnumerable<UserResponse> result = await userService.Read(ct);
			return Results.Ok(result);
		}).WithTags(tag).Produces<IEnumerable<UserResponse>>();

		app.MapGet("user/Read/{id:guid}", async (IUserService userService, Guid id, CancellationToken ct) => {
			BaseResponse<UserResponse?> result = await userService.ReadById(id, ct);
			return result.ToResult();
		}).WithTags(tag);

		app.MapPut("user/Update", async (IUserService userService, UserUpdateParams param, CancellationToken ct) => {
			UserResponse? result = await userService.Update(param, ct);
			return result == null ? Results.NotFound() : Results.Ok(result);
		}).WithTags(tag);

		app.MapDelete("user/Delete", async (IUserService userService, Guid id, CancellationToken ct) => {
			await userService.Delete(id, ct);
			return Results.Ok();
		}).WithTags(tag);

		app.MapGet("user/getProfile", async (IUserService service, CancellationToken ct) => {
			UserResponse? result = await service.GetProfile(ct);
			return result == null ? Results.Unauthorized() : Results.Ok(result);
		}).RequireAuthorization().WithTags(tag);

		app.MapPost("user/filterUsers", async (IUserService service, UserFilterParams p, CancellationToken ct) => {
			IEnumerable<UserEntity>? result = await service.Filter(p, ct);
			return Results.Ok(result);
		}).WithTags(tag);
	}
}