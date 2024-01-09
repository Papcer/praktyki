using System.Reflection;
using System.Text;
using App.Class;
using App.Context;
using App.Services;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Domain.Settings;
using Gotenberg.Sharp.API.Client.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using StackExchange.Redis;
using App.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;

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
        Title = "Serwer ASP.NET",
        Description = "Serwer obsługujący generowanie plików PDF",
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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Podaj token JWT ",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

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
    var rabbitMqConfig = builder.Configuration.GetConnectionString("RabbitMQConnection");
    return RabbitHutch.CreateBus(rabbitMqConfig);
});

//---RabbitMQ connect---//
var factory = new ConnectionFactory()
{
    HostName = "rabbitmq",
    UserName = "guest",
    Password = "guest",
    VirtualHost = "/",
    RequestedHeartbeat = new TimeSpan(60),
};

var connection = factory.CreateConnection();
var channel = connection.CreateModel();

builder.Services.AddSingleton<IModel>(channel);
builder.Services.AddSingleton<IConnection>(connection);


builder.Services.AddSingleton<RedisService>();

builder.Services.AddGotenbergSharpClient();

builder.Services.AddScoped<PdfGenerationConsumer>();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();

var SECRET_KEY = "URSEtbL5giT1P0um_E0wEUF55-BhIm6ngFpJAoliA4s";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var rabbitMqService = scope.ServiceProvider.GetRequiredService<IRabbitMqService>();
    await rabbitMqService.SubscribeToQueue();
}


// Configure the HTTP request pipeline.

app.UseCors(builder => builder
    .WithOrigins("http://127.0.0.1:5500")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<PdfHub>("/pdfHub");
});

app.UseHttpsRedirection();


app.MapControllers();


app.Run();