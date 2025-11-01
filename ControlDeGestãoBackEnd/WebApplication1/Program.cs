using Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using WebApplication1.Services;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Adiciona servi�os ao cont�iner.
builder.Services.AddDbContext<GestaoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esta op��o diz ao serializador para ignorar os ciclos
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });



builder.Services.AddControllers();

builder.Services.AddScoped<AlertaService>();
builder.Services.AddScoped<VendasService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<IReceitaService, ReceitaService>();
builder.Services.AddScoped<RelatorioService>(); // Repetido, pode ser removido

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- IN�CIO DA CONFIGURA��O DO CORS ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                     policy =>
                     {
                         policy.AllowAnyMethod()
                                 .AllowAnyHeader()
                                 .AllowCredentials()
                                 .WithOrigins("http://localhost:5173");
                     });
});

// Configura��o do JWT
var secretKey = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

var app = builder.Build();

// Configura o pipeline de requisi��o HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- CORRE��O AQUI ---
// O middleware de CORS deve vir ANTES da autentica��o e autoriza��o.
app.UseCors(MyAllowSpecificOrigins);

// Use os servi�os de autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();