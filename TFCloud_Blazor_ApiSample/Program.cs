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
    options.AddPolicy("userRequired", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddCors(options => options.AddPolicy("MyPolicy", 
    o => o.AllowCredentials()
          .WithOrigins("https://localhost:7041")
          .AllowAnyHeader()
          .AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// OBLIGATOIREMENT DANS CE SENS
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("MyPolicy");
//app.UseCors(o => o.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
app.MapControllers();

app.Run();
