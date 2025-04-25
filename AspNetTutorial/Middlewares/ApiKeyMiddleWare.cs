namespace AspNetTutorial.Middlewares;

using Microsoft.Extensions.Primitives;
using System.Text.Json;

// public class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration) {
// 	public async Task Invoke(HttpContext context) {
// 		if ((context.Request.Path.Value ?? "").Contains("api"))
// 			if (!context.Request.Headers.TryGetValue("X-API-Key", out StringValues apiKey) || apiKey != configuration.GetValue<string>("ApiKey")!) {
// 				context.Response.StatusCode = StatusCodes.Status404NotFound;
// 				return;
// 			}
//
// 		await next(context);
// 	}
// }

public class ApiKeyMiddleware(RequestDelegate next, IConfiguration config) {
	public async Task InvokeAsync(HttpContext context) {
		if (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase)) {
			if (!context.Request.ContentType?.Contains("application/json") ?? true) {
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				await context.Response.WriteAsync("Invalid content type");
				return;
			}

			context.Request.EnableBuffering();
			string body = await new StreamReader(context.Request.Body).ReadToEndAsync();
			context.Request.Body.Position = 0;

			JsonElement json = JsonSerializer.Deserialize<JsonElement>(body);
			if (!json.TryGetProperty("apiKey", out JsonElement apiKeyProperty) ||
			    apiKeyProperty.GetString() != config["ApiKey"]) {
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				await context.Response.WriteAsync("Invalid API key");
				return;
			}
		}

		await next(context);
	}
}