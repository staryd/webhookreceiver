using System.Net;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.GetSection("Settings").Get<Settings>() ?? new Settings();
builder.Services.AddSingleton(settings);
builder.Services.AddHostedService<DeleteOldFiles>();
var app = builder.Build();

if (!Directory.Exists(settings.GetUserTempPath()))
	Directory.CreateDirectory(settings.GetUserTempPath());

Console.WriteLine($"Temp file root path: {settings.GetUserTempPath()}");

app.MapGet("/", () => Results.Ok());


app.MapPost("/{id}", async (HttpRequest request, string id, [FromBody]object body, [FromQuery]int? returnStatus, [FromQuery]string? code = "") =>
	{
		if (!string.Equals(code, settings.AuthCode))
			return Results.Unauthorized();

		var prefix = string.IsNullOrWhiteSpace(id) ? string.Empty : $"{id}-";
		var localFilePath = Path.Combine(settings.GetUserTempPath(), $"{prefix}{Guid.NewGuid()}.txt");
		var requestInfo = $"{id}{request.QueryString}{Environment.NewLine}{Environment.NewLine}{body}";

		await File.WriteAllTextAsync(localFilePath, requestInfo);

		if (returnStatus.HasValue && Enum.IsDefined(typeof(HttpStatusCode), returnStatus.Value))
			return Results.StatusCode(returnStatus.Value);

		return Results.Ok();
	});


app.Run();
