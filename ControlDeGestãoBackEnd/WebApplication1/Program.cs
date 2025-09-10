using Database;
using WebApplication1.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.
builder.Services.AddDbContext<GestaoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();

builder.Services.AddScoped<AlertaService>();

builder.Services.AddScoped<VendasService>();

builder.Services.AddScoped<RelatorioService>();

builder.Services.AddScoped<IReceitaService, ReceitaService>();

builder.Services.AddScoped<RelatorioService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- INÍCIO DA CONFIGURAÇÃO DO CORS ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; // Nome da sua política CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                     policy =>
                     {
                         // Permite qualquer método (GET, POST, PUT, DELETE, etc.)
                         policy.AllowAnyMethod()
                                 // Permite qualquer cabeçalho na requisição
                                 .AllowAnyHeader()
                                 // Permite credenciais (cookies, autenticação HTTP) 
                                 .AllowCredentials()
                                 // Define as origens permitidas
                                 .WithOrigins("http://localhost:5173");
                     });
});



// Configuração do JWT
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

// Use os serviços de autenticação e autorização
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


// Configura o pipeline de requisição HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
