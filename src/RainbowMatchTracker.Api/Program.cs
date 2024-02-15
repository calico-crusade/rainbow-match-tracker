using Microsoft.AspNetCore.Diagnostics;
using RainbowMatchTracker;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

await builder.Services.AddServices(builder.Configuration, c =>
{
    c.AddCore()
     .AddDatabase()
     .AddMatches()
     .AddRollup();
});

builder.Services.AddHostedService<MergeBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(err =>
{
    err.Run(async ctx =>
    {
        Exception? resolveException(WebApplication app)
        {
            if (!app.Environment.IsDevelopment()) return null;

            var feature = ctx.Features.Get<IExceptionHandlerFeature>();
            if (feature != null && feature.Error != null)
                return feature.Error;

            feature = ctx.Features.Get<IExceptionHandlerPathFeature>();
            return feature?.Error;
        };

        ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
        ctx.Response.ContentType = Application.Json;
        ctx.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");
        var error = resolveException(app) ?? new Exception("An error has occurred, please contact an administrator for more information");

        await ctx.Response.WriteAsJsonAsync(Requests.Exception(error));
    });
});

app.Use(async (ctx, next) =>
{
    await next();

    if (ctx.Response.StatusCode == StatusCodes.Status401Unauthorized)
    {
        ctx.Response.ContentType = Application.Json;
        await ctx.Response.WriteAsJsonAsync(Requests.Unauthorized());
    }
});

app.UseCors(c =>
{
    c.AllowAnyHeader()
     .AllowAnyMethod()
     .AllowAnyOrigin()
     .WithExposedHeaders("Content-Disposition");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
