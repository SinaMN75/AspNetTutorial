namespace AspNetTutorial.Routes;

using Dtos;
using Services;

public static class Auth {
	public static void MapAuthRoutes(this IEndpointRouteBuilder app, string tag) {
		app.MapPost("auth/login", async (LoginParams p, IAuthService service) => {
			LoginResponse? result = await service.Login(p);
			return result == null ? Results.Unauthorized() : Results.Ok(result);
		}).WithTags(tag);
		
		app.MapPost("auth/Register", async (IAuthService authService, UserCreateParams dto) => {
			UserResponse result = await authService.Register(dto);
			return Results.Ok(result);
		}).WithTags(tag);

	}
}