using System.Reflection;
using FluentValidation.AspNetCore;
using Helsi.Todo.Api.Middlewares;
using Helsi.Todo.Application;
using Helsi.Todo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlPath = Path.Combine(
        AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

// Extension methods can be redundant here, but I want to show the way that I prefer to register dependencies.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
// Is this case validation my look redundant, but I want to show that I know that it is important and I know how to use it.
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseMiddleware<ProblemDetailsMiddleware>();
app.MapControllers();
app.Run();