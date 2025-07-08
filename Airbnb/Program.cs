using Airbnb.Infrastructure.Data;
using Airbnb.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Airbnb.Application.Validators;
using FluentValidation.AspNetCore;
using Mapster;
using Airbnb.Middlewares;
using Airbnb.Extensions;
 
var builder = WebApplication.CreateBuilder(args);

TypeAdapterConfig.GlobalSettings.Scan(typeof(ApplicationUser).Assembly);
TypeAdapterConfig.GlobalSettings.Scan(typeof(Apartment).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.AddMapApartmentsEndpoints();

app.MapControllers();

app.Run();