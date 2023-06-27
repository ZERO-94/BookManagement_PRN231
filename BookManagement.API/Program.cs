using BookManagement.API.Extensions;
using BookManagement.Infrastructure.Data;
using BookManagement.Infrastructure.Models;
using BookManagement.Infrastructure.Repositories.BookRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using System.Text;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

ODataConventionModelBuilder oDataBuilder = new ODataConventionModelBuilder();
oDataBuilder.EntitySet<Book>("Books");
oDataBuilder.EntitySet<Press>("Presses");
oDataBuilder.EntitySet<Account>("Accounts");

builder.Services.AddControllers().AddOData(option => option.Select().Filter().Count().OrderBy().Expand().SetMaxTop(100).AddRouteComponents("odata", oDataBuilder.GetEdmModel()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddScoped<JwtService>();

var configuration = builder.Configuration;

builder.Services.AddDbContext<BookManagementContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        builder => builder.MigrationsAssembly(typeof(BookManagementContext).Assembly.FullName));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // Validate JWT Token
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new ArgumentException("Jwt:Key is required")))
    };
}
);
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddScoped<IBookRepository, BookRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseODataBatching();

app.UseRouting();

app.Use((context, next) =>
{
    var endpoint = context.GetEndpoint();
    if(endpoint == null)
    {
        return next(context);
    }

    IEnumerable<string> templates;
    IODataRoutingMetadata metadata = endpoint.Metadata.GetMetadata<IODataRoutingMetadata>();
    if(metadata != null)
    {
        templates = metadata.Template.GetTemplates();
    }

    return next(context);
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
