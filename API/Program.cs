using API.Models.Contexts;
using Microsoft.EntityFrameworkCore;
using API.Models.Services;
using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "API.xml"), true);

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "API", Version = "v2" });


    //c.DocInclusionPredicate((doc, apiDescription) =>
    //{
    //    if (!apiDescription.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

    //    var version = methodInfo.DeclaringType
    //        .GetCustomAttributes<ApiVersionAttribute>(true)
    //        .SelectMany(attr => attr.Versions);

    //    return version.Any(v => $"v{v.ToString()}" == doc);
    //});

});
builder.Services.AddEntityFrameworkSqlServer().AddDbContext<DataBaseContext>(option => 
                                                                             option.UseSqlServer("Server=.;Database=webApi;Integrated Security=True;TrustServerCertificate=True;"
                                                                             ));
builder.Services.AddApiVersioning(Options =>
{
    Options.AssumeDefaultVersionWhenUnspecified = true;
    Options.DefaultApiVersion = new ApiVersion(1, 0);
    Options.ReportApiVersions = true;
});
builder.Services.AddScoped<TodoRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserTokenRepository>();

builder.Services.AddAuthentication(Options =>
{
    Options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
            .AddJwtBearer(configureOptions =>
            {
                configureOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = Configuration["JWtConfig:issuer"],
                    ValidAudience = Configuration["JWtConfig:audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWtConfig:Key"])),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                };
                configureOptions.SaveToken = true; // HttpContext.GetTokenAsunc();
                configureOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //log 
                        //........
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        //log
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        return Task.CompletedTask;

                    },
                    OnMessageReceived = context =>
                    {
                        return Task.CompletedTask;

                    },
                    OnForbidden = context =>
                    {
                        return Task.CompletedTask;

                    }
                };

            });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
