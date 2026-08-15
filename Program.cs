
using Scalar.AspNetCore;
using TmsApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using TmsApi.Middleware;
using TmsApi.Filters;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Application.Enrollments.Commands;
using FluentValidation;
using MediatR;
using TmsApi.Application.Behaviors;
using TmsApi.Api.ExceptionHandlers;
using TmsApi.Application.Interfaces;
using Microsoft.Extensions.Caching.Hybrid;
using TmsApi.Infrastructure.Transcripts;
using System.Threading.Channels;
using TmsApi.Application.Transcripts;
using TmsApi.Infrastructure.Workers;
using TmsApi.Api.Hubs;
using TmsApi.Api.Notifications;
using TmsApi.Application.Notifications;
using Microsoft.AspNetCore.Antiforgery;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditLogFilter>();
});

builder.Services.AddMediatR(cfg =>cfg.RegisterServicesFromAssembly(typeof(EnrollStudentHandler).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(EnrollStudentValidator).Assembly);
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(10), LocalCacheExpiration = TimeSpan.FromMinutes(2)
    };
});
builder.Services.AddSingleton < ITranscriptStatusStore, InMemoryTranscriptStatusStore > ();
builder.Services.AddSingleton < ITranscriptNotificationService, SignalRTranscriptNotificationService > ();
// LoggingBehavior FIRST—it must wrap ValidationBehavior
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<TmsDbContext>(options =>options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase")));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.AddOpenApi("v1", options =>
{
    options.ShouldInclude = description =>
    description.GroupName == "v1";
});
builder.Services.AddAntiforgery(options => { options.HeaderName = "X-XSRF-TOKEN"; });
builder.Services.AddOpenApi("v2", options =>
{
    options.ShouldInclude = description =>
    description.GroupName == "v2";
});
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
// Load allowed origins from appsettings.Development.json
var allowedOrigins = builder.Configuration
                         .GetSection("AllowedOrigins").Get<string[]>()
                     ?? ["http://localhost:4200"];
// Register the CORS policy in the Dependency Injection container
builder.Services.AddCors(options =>
{
    options.AddPolicy("TmsClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials() // Vital for HttpOnly auth cookies in Session 2
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});
//builder.Services.AddSingleton<EnrollmentWorker>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddHostedService<TranscriptWorker>();
builder.Services.AddSingleton(Channel.CreateBounded<TranscriptRequest>(
    new BoundedChannelOptions(100)
    {
        FullMode = BoundedChannelFullMode.Wait
    }));
builder.Services.AddDbContext<TmsDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase"))
.LogTo(Console.WriteLine, LogLevel.Information) // Log SQL to output window
.EnableSensitiveDataLogging()); // Show parameters in querylogs (dev only)
// Register TmsDbContext scoped for incoming HTTP requests
// builder.Services.AddDbContext<TmsDbContext>(options =>
// options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase")));
builder.Services.AddCors(options =>
{
options.AddPolicy("AllowAngular", policy =>
policy.WithOrigins("http://localhost:4200")
.AllowAnyHeader()
.AllowAnyMethod());
});
// ...


builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
{
    options.WithTitle("TMS API Reference")
    .WithTheme(ScalarTheme.DeepSpace)
    .WithDefaultHttpClient(ScalarTarget.CSharp,
    ScalarClient.HttpClient);
    // Tell Scalar to pull both documents into its sidebar dropdown
    options
    .AddDocument("v1", "API Version 1.0")
    .AddDocument("v2", "API Version 2.0");
});

}
// CRITICAL: Middleware order matters!
// UseRouting -> UseCors -> UseAuthentication -> UseAuthorization
app.UseCors("TmsClient");
app.MapHub<TmsHub>("/hubs/tms");
// // update your scalar config
// app.MapScalarApiReference();
app.UseCors("AllowAngular");
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true || context.Request.Cookies.ContainsKey("tms_auth"))
    {
        var antiforgery = context.RequestServices
            .GetRequiredService<IAntiforgery>();
        var tokens = antiforgery.GetAndStoreTokens(context);
        context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
        new CookieOptions
        {
            HttpOnly = false, // MUST be false so Angular JavaScript can read it!
            Secure = !builder.Environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict
        });
    }

    await next(context);
});
app.UseCors("AllowAngular");
app.MapControllers();
app.UseMiddleware<V1DeprecationMiddleware>();
app.Run();

internal interface ICachedCourseService
{
}