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

// Adiciona servi�os ao cont�iner.
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

// --- IN�CIO DA CONFIGURA��O DO CORS ---
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; // Nome da sua pol�tica CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                     policy =>
                     {
                         // Permite qualquer m�todo (GET, POST, PUT, DELETE, etc.)
                         policy.AllowAnyMethod()
                                 // Permite qualquer cabe�alho na requisi��o
                                 .AllowAnyHeader()
                                 // Permite credenciais (cookies, autentica��o HTTP) 
                                 .AllowCredentials()
                                 // Define as origens permitidas
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

// Use os servi�os de autentica��o e autoriza��o
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


// Configura o pipeline de requisi��o HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
