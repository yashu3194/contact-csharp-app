using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Contact Management REST API Documentation",
        Description = "Contact Management application",
        Version = "1.0",
        License = new OpenApiLicense
        {
            Name = "LGPL",
            Url = new Uri("http://lgpl.com")
        },
        Contact = new OpenApiContact
        {
            Name = "Ajinkz",
            Email = "ajinkz@example.com"
        }
    });
});

builder.Services.AddSingleton<IContactService, ContactService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();