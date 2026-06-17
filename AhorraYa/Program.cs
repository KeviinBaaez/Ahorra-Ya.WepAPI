using AhorraYa.Abstractions;
using AhorraYa.Application;
using AhorraYa.Application.Interfaces;
using AhorraYa.DataAccess;
using AhorraYa.Entities.MicrosoftIdentity;
using AhorraYa.Exceptions;
using AhorraYa.Repository.Interfaces;
using AhorraYa.Repository.Repositories;
using AhorraYa.Services.Interfaces;
using AhorraYa.Services.Services;
using AhorraYa.WebApi.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

////////////////////////API PRUEBA////////////////////////////////
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
////////////////////////API PRUEBA////////////////////////////////


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AhorraYa.WebApi", Version = "v1" });

    var jwtSecuriryScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put your Token below",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(jwtSecuriryScheme.Reference.Id, jwtSecuriryScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtSecuriryScheme, Array.Empty<string>() } });
});


//builder.Services.AddDbContext<DbDataAccess>(options =>
//{
//    options.UseSqlServer(builder.Configuration
//        .GetConnectionString("MyConnection"),
//        o => o.MigrationsAssembly("AhorraYa.WebApi"));
//    options.UseLazyLoadingProxies();
//});

builder.Services.AddDbContext<DbDataAccess>(options =>
{
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection"),
        o => o.MigrationsAssembly("AhorraYa.WebApi"));
    options.UseLazyLoadingProxies();
});

builder.Services.Configure<ServiceJwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
    };
});

builder.Services.AddIdentity<User, Role>(options =>
options.SignIn.RequireConfirmedAccount = true).
    AddDefaultTokenProviders().
    AddEntityFrameworkStores<DbDataAccess>().
    AddSignInManager<SignInManager<User>>().
    AddRoleManager<RoleManager<Role>>().
    AddUserManager<UserManager<User>>();
//DI
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IApplication<>), typeof(Application<>));
builder.Services.AddScoped(typeof(IDbContext<>), typeof(DbContext<>));
builder.Services.AddScoped(typeof(IServiceTokenHandler), typeof(ServiceTokenHandler));
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();
////////////////////////API PRUEBA////////////////////////////////
app.UseCors("AllowAll");


////////////////////////API PRUEBA////////////////////////////////

try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DbDataAccess>();
        if (!db.Database.CanConnect())
        {
            throw new ExceptionByServiceConnection();
        }
    }
}
catch (ExceptionByServiceConnection ex)
{
    Console.WriteLine(ex.Message);
    throw;
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
////////////////////////API PRUEBA////////////////////////////////
app.UseSwagger();
app.UseSwaggerUI();

////////////////////////API PRUEBA////////////////////////////////


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedAsync(services);
}

app.Run();