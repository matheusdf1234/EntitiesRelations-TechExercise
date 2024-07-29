using EntitiesRelations.API;
using EntitiesRelations.API.Providers;
using EntitiesRelations.API.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IOwnershipService, OwnershipService>();


builder.Services.AddScoped<IPersonProvider, PersonProvider>();
builder.Services.AddScoped<ICompanyProvider, CompanyProvider>();    


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
