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

var app = builder.Build();

// Configure the HTTP request pipeline.
// MIDDLEWARE HERE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();