using TimeZoneCoverage.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITimeZoneResolver, TimeZoneResolver>();
builder.Services.AddSingleton<ICoverageActivationService, CoverageActivationService>();
builder.Services.AddSingleton<IClock, SystemClock>();

var app = builder.Build();

// launchSettings.json in the API project is configured 
// to use "Development" environment, so Swagger will be
// enabled by default when running locally. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// no https redirection in local development 
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
