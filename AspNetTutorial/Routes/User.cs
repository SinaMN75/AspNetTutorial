namespace AspNetTutorial.Routes;

using Dtos;
using Services;

public static class User {
	public static void MapUserRoutes(this IEndpointRouteBuilder app, string tag) {
		app.MapPost("user/Create", async (IUserService userService, UserCreateParams dto) => {
			UserResponse result = await userService.Create(dto);
			return Results.Ok(result);
		}).WithTags(tag);

		app.MapGet("user/Read", async (IUserService userService) => {
			IEnumerable<UserResponse> result = await userService.Read();
			return Results.Ok(result);
		}).WithTags(tag).Produces<IEnumerable<UserResponse>>();

		app.MapGet("user/Read/{id:guid}", async (IUserService userService, Guid id) => {
			BaseResponse<UserResponse?> result = await userService.ReadById(id);
			return result.ToResult();
		}).WithTags(tag);

		app.MapPut("user/Update", async (IUserService userService, UserUpdateParams param) => {
			UserResponse? result = await userService.Update(param);
			return result == null ? Results.NotFound() : Results.Ok(result);
		}).WithTags(tag);

		app.MapDelete("user/Delete", async (IUserService userService, Guid id) => {
			await userService.Delete(id);
			return Results.Ok();
		}).WithTags(tag);

		app.MapGet("user/getProfile", async (IUserService service) => {
			UserResponse? result = await service.GetProfile();
			return result == null ? Results.Unauthorized() : Results.Ok(result);
		}).RequireAuthorization().WithTags(tag);
	}
}