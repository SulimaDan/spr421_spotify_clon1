using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Serilog;
using spr421_spotify_clone.BLL.Services.Auth;
using spr421_spotify_clone.BLL.Services.Genre;
using spr421_spotify_clone.BLL.Services.Storage;
using spr421_spotify_clone.BLL.Services.Track;
using spr421_spotify_clone.BLL.Settings;
using spr421_spotify_clone.DAL;
using spr421_spotify_clone.DAL.Entities.Identity;
using spr421_spotify_clone.DAL.Initializer;
using spr421_spotify_clone.DAL.Repositories.Genre;
using spr421_spotify_clone.DAL.Repositories.Track;
using spr421_spotify_clone.Infrastructure;
using spr421_spotify_clone.Jobs;
using spr421_spotify_clone.Middlewares;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add dbcontext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultDb"));
});

// Add identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();

// Add logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/.log", rollingInterval: RollingInterval.Minute)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add quartz
var jobs = new (Type type, string schedule)[]
{
    (typeof(HelloJob), "0 0 0/1 * * ?"),
    (typeof(LogsCleaningJob), "0 * * * * ?"),
    (typeof(EmailJob), "30 * * * * ?")
};

//builder.Services.AddJobs(jobs);
//builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Add automapper
builder.Services.AddAutoMapper(options =>
{
    options.LicenseKey = builder.Configuration["Automapper:LicenseKey"];
}, AppDomain.CurrentDomain.GetAssemblies());

// Add authentication
// Налаштування JWT аутентифікації та авторизації для захисту API
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if(jwtSettings is null || string.IsNullOrEmpty(jwtSettings.SecretKey))
{
    throw new Exception("JwtSettings is not configured properly.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        RequireExpirationTime = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// Add repositories
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();

// Add services
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Налаштування Swagger для документування API та підтримки JWT аутентифікації в Swagger UI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "spr421_spotify_clone API",
        Version = "v1"
    });
    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add cors
string corsPolicy = "allowall";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Static files
app.AddStaticFiles(app.Environment);

app.MapControllers();

app.UseCors(corsPolicy);

app.Seed();

app.Run();