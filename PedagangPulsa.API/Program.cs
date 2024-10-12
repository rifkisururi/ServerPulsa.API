using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PedagangPulsa.API.Database;
using PedagangPulsa.API.Interface;
using PedagangPulsa.API.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Tambahkan service Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Konfigurasi Bearer Token untuk Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Masukkan token JWT Anda di sini: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


string db = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(db));

var key = Encoding.ASCII.GetBytes("your_jwt_secret_key_hereyour_jwt_secret_key_here");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidateLifetime = true
    };
});



// Register Service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IProdukService, ProdukService>();
builder.Services.AddScoped<ITransaksiService ,TransaksiService>();
builder.Services.AddScoped<IDuitkuService, DuitkuService>();
builder.Services.AddHttpClient<ProdukService>();

// Tambahkan konfigurasi MQTT service
builder.Services.AddSingleton<MqttService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// Membuat database secara otomatis jika belum ada
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Pastikan bahwa database sudah ada, jika tidak akan dibuat
    dbContext.Database.EnsureCreated();
    // Alternatif menggunakan migrasi jika menggunakan migration (lebih dianjurkan untuk produksi)
    // dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

// Aktifkan Authentication dan Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
