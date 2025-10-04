using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;
using SEP490_G154_Service.Service;
using SEP490_G154_Service.sHub;
using System.Text;

namespace SEP490_G154_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ========== Cấu hình Controller + JSON ==========
            builder.Services.AddControllers()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.PropertyNamingPolicy = null; 
                });

            builder.Services.AddEndpointsApiExplorer();

            // ========== Swagger + JWT ==========
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DongVan Ecoverse API",
                    Version = "v1",
                    Description = "REST API cho hệ thống thương mại & du lịch Đồng Văn"
                });

                // Cấu hình JWT trong Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập token JWT (chỉ cần dán token, không cần viết 'Bearer ')"
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
                        Array.Empty<string>()
                    }
                });
            });

            // ========== Database ==========
            builder.Services.AddDbContext<G154context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

            // ========== JWT Authentication ==========
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment(); 
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            })
            .AddCookie()
            .AddFacebook(options =>
            {
                options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                options.CallbackPath = "/signin-facebook";
                options.Scope.Add("email");
                options.Fields.Add("name");
                options.Fields.Add("email");
                options.Fields.Add("picture");
            });

            // ========== CORS ==========
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:5173",
                        "https://localhost:5173"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            // ========== Dependency Injection ==========
            builder.Services.AddScoped<ILogin, LoginService>();
            builder.Services.AddScoped<IHomeStay, HomeStayService>();
            builder.Services.AddScoped<IProducts, ProductService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddMemoryCache();

            // ========== ElasticSearch ==========
            builder.Services.AddSingleton<IElasticClient>(sp =>
            {
                var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                    .DefaultIndex("homestays");
                return new ElasticClient(settings);
            });

            // ========== Build App ==========
            var app = builder.Build();

            // ========== Middleware Pipeline ==========
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Remove("Cross-Origin-Opener-Policy");
                context.Response.Headers.Remove("Cross-Origin-Embedder-Policy");
                await next();
            });

            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
