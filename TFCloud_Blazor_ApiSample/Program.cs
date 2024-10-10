using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TFCloud_Blazor_ApiSample.Repos;
using TFCloud_Blazor_ApiSample.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<SqlConnection>(sp => 
    new SqlConnection(builder.Configuration.GetConnectionString("default")));
builder.Services.AddScoped<JwtGenerator>();
builder.Services.AddScoped<UserRepo>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtGenerator.secretKey)),
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidIssuer = "monapi.com",
                ValidateAudience = false,
            };
        }
    );

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("adminRequired", policy => policy.RequireRole("Admin"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
