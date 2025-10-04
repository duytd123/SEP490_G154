using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;
using SEP490_G154_Service.Service;
using SEP490_G154_Service.sHub;
using Nest;
using System.Text;

namespace SEP490_G154_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // ================= Swagger + JWT =================
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "G154 API",
                    Version = "v1"
                });

                // 🔑 Cấu hình JWT Bearer
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http, // đổi sang Http
                    Scheme = "Bearer",                                       // lowercase "bearer" cũng được
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Nhập JWT token (chỉ cần dán token, không cần viết Bearer)"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });


            // ================= Database =================
            builder.Services.AddDbContext<G154context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

            // ================= JWT Authentication =================
            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // chỉ bật true khi deploy thật
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            })
            .AddCookie()
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                facebookOptions.CallbackPath = "/signin-facebook";
                facebookOptions.Scope.Add("email");
                facebookOptions.Fields.Add("name");
                facebookOptions.Fields.Add("email");
                facebookOptions.Fields.Add("picture");
            });

            // ================= CORS =================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // ================= DI =================
            builder.Services.AddScoped<ILogin, LoginService>();
            builder.Services.AddScoped<IHomeStay, HomeStayService>();
            builder.Services.AddScoped<IProducts, ProductService>();
            builder.Services.AddHttpClient<INews, NewsService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddMemoryCache();

            // ================= ElasticSearch =================
            builder.Services.AddSingleton<IElasticClient>(sp =>
            {
                var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                    .DefaultIndex("homestays");
                return new ElasticClient(settings);
            });

            var app = builder.Build();

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

            // Middleware phải đúng thứ tự
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
