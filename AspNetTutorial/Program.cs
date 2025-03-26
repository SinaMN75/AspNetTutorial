using AspNetTutorial;
using AspNetTutorial.Dtos;
using AspNetTutorial.Entities;
using AspNetTutorial.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options => {
	options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	options.SerializerOptions.WriteIndented = false;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
	c.UseInlineDefinitionsForEnums();
	c.OrderActionsBy(s => s.RelativePath);
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
		Description = "JWT Authorization header.\r\n\r\nExample: \"Bearer 12345abcdef\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement {
		{ new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() },
	});
});

builder.Services.AddScoped<AppDbContext>();
builder.Services.AddDbContextPool<AppDbContext>(o => { o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDatabase")); });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(options => {
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters {
		RequireSignedTokens = true,
		ValidateIssuerSigningKey = true,
		ValidateIssuer = true,
		ValidateAudience = true,
		RequireExpirationTime = false,
		ClockSkew = TimeSpan.Zero,
		ValidAudience = "https://SinaMN75.com,123456789987654321",
		ValidIssuer = "https://SinaMN75.com,123456789987654321",
		IssuerSigningKey = new SymmetricSecurityKey("https://SinaMN75.com,123456789987654321"u8.ToArray())
	};
});

builder.Services.AddAuthorization();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("user/Create", async (IUserService userService, UserCreateParams dto) => {
	UserResponse result = await userService.Create(dto);
	return Results.Ok(result);
});

app.MapGet("user/Read", async (IUserService userService) => {
	IEnumerable<UserResponse> result = await userService.Read();
	return Results.Ok(result);
});

app.MapGet("user/Read/{id:guid}", async (IUserService userService, Guid id) => {
	UserResponse? result = await userService.ReadById(id);
	return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapPut("user/Update", async (IUserService userService, UserUpdateParams param) => {
	UserResponse? result = await userService.Update(param);
	return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapDelete("user/Delete", async (IUserService userService, Guid id) => {
	await userService.Delete(id);
	return Results.Ok();
});


app.MapPost("class/Create", async (IClassService service, ClassCreateDto dto) => {
	ClassEntity result = await service.Create(dto);
	return Results.Ok(result);
});

app.MapGet("class/Read", async (IClassService service) => {
	IEnumerable<ClassEntity> result = await service.Read();
	return Results.Ok(result);
});

app.MapGet("class/Read/{id:guid}", async (IClassService service, Guid id) => {
	ClassEntity? result = await service.ReadById(id);
	return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapPut("class/Update", async (IClassService service, ClassEntity param) => {
	ClassEntity? result = await service.Update(param);
	return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapDelete("class/Delete", async (IClassService service, Guid id) => {
	await service.Delete(id);
	return Results.Ok();
});


app.MapPost("School/Create", async (ISchoolService service, SchoolEntity dto) => {
	SchoolEntity result = await service.Create(dto);
	return Results.Ok(result);
});

app.MapGet("School/Read", async (ISchoolService service) => {
	IEnumerable<SchoolEntity> result = await service.Read();
	return Results.Ok(result);
});

app.MapGet("School/Read/{id:guid}", async (ISchoolService service, Guid id) => {
	SchoolEntity? result = await service.ReadById(id);
	return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapPut("School/Update", async (ISchoolService service, SchoolEntity param) => {
	SchoolEntity? result = await service.Update(param);
	return result == null ? Results.NotFound() : Results.Ok(result);
});

app.MapDelete("School/Delete", async (ISchoolService service, Guid id) => {
	await service.Delete(id);
	return Results.Ok();
});


app.MapPost("auth/login", async (LoginParams p, IAuthService service) => {
	LoginResponse? result = await service.Login(p);
	return result == null ? Results.Unauthorized() : Results.Ok(result);
});


app.MapPost("auth/Register", async (IAuthService authService, UserCreateParams dto) => {
	UserResponse result = await authService.Register(dto);
	return Results.Ok(result);
});

app.MapGet("user/getProfile", async (IUserService service) => {
	UserResponse? result = await service.GetProfile();
	return result == null ? Results.Unauthorized() : Results.Ok(result);
}).RequireAuthorization();

app.Run();