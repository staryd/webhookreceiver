using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebhookReceiver;

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.GetSection("Settings").Get<Settings>() ?? new Settings();
builder.Services.AddSingleton(settings);
builder.Services.AddHostedService<DeleteOldFiles>();
var app = builder.Build();

if (!Directory.Exists(settings.GetUserTempPath()))
	Directory.CreateDirectory(settings.GetUserTempPath());

Console.WriteLine($"Temp file root path: {settings.GetUserTempPath()}");

IResult? ValidateRequest(HttpRequest request, string? code)
{
	if (!request.Headers.ContainsKey("Authorization"))
	{
		if (!string.Equals(code, settings.AuthCode))
			return Results.Unauthorized();

		return null;
	}

	var authHeader = request.Headers["Authorization"][0];
	if (authHeader?.StartsWith("Bearer ") == true)
	{
		var token = authHeader.Substring("Bearer ".Length);
		if (!string.Equals(token, settings.AuthCode))
			return Results.Unauthorized();

		return null;
	}

	return Results.Unauthorized();
}

async Task<IResult> StoreRequestAsync(HttpRequest request, string id, int? returnStatus, string? body = null)
{
	var prefix = string.IsNullOrWhiteSpace(id) ? string.Empty : $"{Utils.SanitizeFileName(id)}-";
	var localFilePath = Path.Combine(settings.GetUserTempPath(), $"{prefix}{Guid.NewGuid()}.txt");
	var headers = string.Join(Environment.NewLine, request.Headers.Select(header => $"{header.Key}: {header.Value}"));
	var requestInfo = body is null
		? $"{id}{request.QueryString}{Environment.NewLine}{Environment.NewLine}{headers}"
		: $"{id}{request.QueryString}{Environment.NewLine}{Environment.NewLine}{headers}{Environment.NewLine}{Environment.NewLine}{body}";

	await File.WriteAllTextAsync(localFilePath, requestInfo);

	if (returnStatus.HasValue && Enum.IsDefined(typeof(HttpStatusCode), returnStatus.Value))
		return Results.StatusCode(returnStatus.Value);

	return Results.Ok();
}

app.MapGet("/{*id}", async (HttpRequest request, string id, [FromQuery]int? returnStatus, [FromQuery]string? code = "") =>
{
	var unauthorizedResult = ValidateRequest(request, code);
	if (unauthorizedResult is not null)
		return unauthorizedResult;

	return await StoreRequestAsync(request, id, returnStatus);
});


app.MapPost("/{*id}", async (HttpRequest request, string id, [FromBody]object body, [FromQuery]int? returnStatus, [FromQuery]string? code = "") =>
	{
		var unauthorizedResult = ValidateRequest(request, code);
		if (unauthorizedResult is not null)
			return unauthorizedResult;

		return await StoreRequestAsync(request, id, returnStatus, body.ToString());
	});



app.Run();