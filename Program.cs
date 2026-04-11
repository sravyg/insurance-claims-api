using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using InsuranceClaimsApi.AuthRegister;
using InsuranceClaimsApi.Middleware;
using InsuranceClaimsApi.Models;
using InsuranceClaimsApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient",
        policy => policy.WithOrigins("http://localhost:4200") // ✅ Allow Angular frontend
                        .AllowAnyMethod() // ✅ Allow GET, POST, PUT, DELETE
                        .AllowAnyHeader() // ✅ Allow all headers
                        .AllowCredentials()); // ✅ Allow authentication cookies
});


builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a valid JWT bearer token."
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));


// builder.Services.AddApiVersioning(options =>
// {
//     options.DefaultApiVersion = new ApiVersion(1, 0); // Set default API version
//     options.AssumeDefaultVersionWhenUnspecified = true;
//     options.ReportApiVersions = true;
// });

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("🚨 Connection string is NULL. Check appsettings.json");
}

// Reduce HTTP client logging noise
builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);

builder.Services.AddScoped<IPasswordHasher<AuthUser>, PasswordHasher<AuthUser>>();
//builder.Services.AddScoped<ICustomIndicatorsService, CustomIndicatorsService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
//builder.Services.AddScoped<ICacheDailyAtrService, CacheDailyAtrService>();
builder.Services.AddValidatorsFromAssemblyContaining<AuthRegisterValidator>();
builder.Services.AddHttpContextAccessor();

var jwtKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is missing from configuration.");

var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "InsuranceClaimsApi";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "InsuranceClaimsApiUsers";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddMemoryCache();

// var connectionString1 = builder.Configuration.GetValue<string>("Redis:ConnectionString")
//     ?? throw new InvalidOperationException("Redis connection string is missing from configuration.");


// builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
// {
//     return ConnectionMultiplexer.Connect(redisConnectionString);
// });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
app.UseCors("AllowAngularClient");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<RequestAuditMiddleware>();
app.UseAuthorization();
app.MapControllers();


app.Run();
