using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using NZWalks.API.Data;
using NZWalks.API.Infrastructure;
using NZWalks.API.Mappings;
using NZWalks.API.Repositories;
using System.Text;
using Microsoft.OpenApi.Models;
using Serilog;
using NZWalks.API.Middlewares;
using NZWalks.API.Swagger;

var builder = WebApplication.CreateBuilder(args);

var applicationConnectionString = builder.Configuration.GetConnectionString("NZWalksConnectionString")
    ?? throw new InvalidOperationException("ConnectionStrings:NZWalksConnectionString is not configured.");
var authConnectionString = builder.Configuration.GetConnectionString("NZWalksAuthConnectionString");

if (string.IsNullOrWhiteSpace(authConnectionString))
{
    authConnectionString = applicationConnectionString;
}

var enableFileLogging = builder.Configuration.GetValue<bool>("LoggingTargets:EnableFile");
var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console();

if (enableFileLogging)
{
    var logsPath = Path.Combine(builder.Environment.ContentRootPath, "Logs");
    Directory.CreateDirectory(logsPath);

    loggerConfiguration = loggerConfiguration.WriteTo.File(
        Path.Combine(logsPath, "NzWalks_Log.txt"),
        rollingInterval: RollingInterval.Day);
}

var logger = loggerConfiguration.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "NZ Walks API",
        Version = "v1"
    });
    option.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    option.OperationFilter<AuthorizeCheckOperationFilter>();
});
builder.Services.AddDbContext<NZWalksDbContext>(options =>
    options.UseSqlServer(applicationConnectionString, sql =>
        sql.MigrationsHistoryTable("__EFMigrationsHistory_NZWalks")));

builder.Services.AddDbContext<NZWalksAuthDbContext>(options =>
    options.UseSqlServer(authConnectionString, sql =>
        sql.MigrationsHistoryTable("__EFMigrationsHistory_NZWalksAuth")));

builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();
builder.Services.AddScoped<IWalksRepository, SQLWalksRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks")
    .AddEntityFrameworkStores<NZWalksAuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtIssuer = builder.Configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is not configured.");
        var jwtAudience = builder.Configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience is not configured.");
        var jwtKey = builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is not configured.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var imagesFolder = builder.Configuration["Storage:ImagesFolder"];
if (string.IsNullOrWhiteSpace(imagesFolder))
{
    imagesFolder = "Images";
}

var imagesPath = Path.Combine(builder.Environment.ContentRootPath, imagesFolder);
Directory.CreateDirectory(imagesPath);

var app = builder.Build();
var swaggerEnabled = app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled");

// Configure the HTTP request pipeline.
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/Images"
});

app.UseAuthentication();
app.UseAuthorization();

await app.InitializeDeploymentAsync();

app.MapGet("/", () => Results.Ok(new
{
    Name = "NZWalks.API",
    Environment = app.Environment.EnvironmentName,
    Status = "Running",
    Health = "/health",
    Swagger = swaggerEnabled ? "/swagger" : null
}));

app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Utc = DateTime.UtcNow
}));

app.MapControllers();

app.Run();
