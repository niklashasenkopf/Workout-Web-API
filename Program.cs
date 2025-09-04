using System.Text;
using C_Sharp_Web_API.Authentication;
using C_Sharp_Web_API.DbContexts;
using C_Sharp_Web_API.Features.TemplateExercises.Persistence;
using C_Sharp_Web_API.Features.Workouts.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// DEPENDECY INJECTION HERE
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<AppDatabaseContext>(dbContextOptions => 
    dbContextOptions.UseNpgsql(builder.Configuration["ConnectionStrings:WorkoutDb"]));
builder.Services.AddScoped<ITemplateExerciseRepository, TemplateExerciseRepository>();
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddAutoMapper(cfg => {}, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddIdentity<ApiUser, IdentityRole<Guid>>(options =>
    {
        // (Optional) relaxed password rules for dev
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<AppDatabaseContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
}).AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretForKey"] ?? ""))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// MIDDLEWARE HERE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

if (app.Environment.IsDevelopment())
{
    await AppSeeder.SeedAsync(app.Services);
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();