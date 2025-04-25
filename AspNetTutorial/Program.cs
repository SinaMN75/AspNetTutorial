using AspNetTutorial;
using AspNetTutorial.Dtos;
using AspNetTutorial.Entities;
using AspNetTutorial.Middlewares;
using AspNetTutorial.Routes;
using AspNetTutorial.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
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

builder.Services.AddOpenApi();
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
	c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme {
		Description = "Api Key is Required",
		Name = "X-API-Key",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "ApiKey"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement {
		{ new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() },
		{ new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" } }, Array.Empty<string>() },
	});
});

builder.Services.AddScoped<AppDbContext>();
builder.Services.AddDbContextPool<AppDbContext>(o => {
	o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
	o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDatabase"));
});

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
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseMiddleware<Base64EncodeMiddleware>();
app.UseMiddleware<Base64DecodeMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();


app.MapUserRoutes("User");
app.MapClassRoutes("Class");
app.MapSchoolRoutes("School");
app.MapAuthRoutes("Auth");


app.Run();