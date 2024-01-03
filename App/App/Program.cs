using System.Reflection;
using App.Context;
using App.Services;
using EasyNetQ;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Domain.Settings;
using Gotenberg.Sharp.API.Client.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


/*builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));*/


builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//builder.Services.AddSingleton<MongoDbService>();

builder.Services.AddOptions<GotenbergSharpClientOptions>()
    .Bind(builder.Configuration.GetSection(nameof(GotenbergSharpClient)));

builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var redisConfig = builder.Configuration.GetConnectionString("RedisConnection");
    var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfig);
    connectionMultiplexer.ConnectionFailed += (_, e) => Console.WriteLine($"Connection failed: {e.Exception}");
    connectionMultiplexer.ConnectionRestored += (_, e) => Console.WriteLine("Connection restored");
    
    return connectionMultiplexer;
});

builder.Services.AddSingleton<IBus>(provider =>
{
    var rabbitMqConfig = builder.Configuration.GetConnectionString("RabbitConnection");
    return RabbitHutch.CreateBus(rabbitMqConfig);
});


builder.Services.AddSingleton<RedisService>();

builder.Services.AddGotenbergSharpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();