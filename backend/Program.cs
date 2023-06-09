using backend.IServices;
using backend.Services;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddCors(p => p.AddPolicy("cors_api", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddScoped<IZipService, ZipService>();
builder.Services.AddScoped<ICultureService, CultureService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("cors_api");

app.Use(next => async context =>
{
    var stopWatch = new Stopwatch();
    stopWatch.Start();

    context.Response.OnStarting(() =>
    {
        stopWatch.Stop();
        context.Response.Headers.Add("X-ResponseTime-Ms", stopWatch.ElapsedMilliseconds.ToString());
        return Task.CompletedTask;
    });

    await next(context);
});

app.Run();
