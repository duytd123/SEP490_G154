
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.EntityFrameworkCore;
using Nest;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;
using SEP490_G154_Service.Service;
using SEP490_G154_Service.sHub;



namespace SEP490_G154_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<G154context>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
            })
.AddCookie() // Cookie để giữ session login
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];

    // Callback URL -> phải trùng với cái bạn điền trong Facebook Dev
    facebookOptions.CallbackPath = "/signin-facebook";

    // Yêu cầu thêm quyền
    facebookOptions.Scope.Add("email");
    facebookOptions.Fields.Add("name");
    facebookOptions.Fields.Add("email");
    facebookOptions.Fields.Add("picture");

});

            // Thêm CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()   // Cho phép mọi origin (hoặc bạn chỉ định: "http://127.0.0.1:5500")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            // Đăng ký DI cho LoginService
            builder.Services.AddScoped<ILogin, LoginService>();
            builder.Services.AddScoped<IHomeStay, HomeStayService>();

            // cấu hình ElasticSearch client
            builder.Services.AddSingleton<IElasticClient>(sp =>
            {
                var settings = new ConnectionSettings(new Uri("http://localhost:9200")) // URL Elastic
                    .DefaultIndex("homestays"); // index mặc định
                return new ElasticClient(settings);
            });


            builder.Services.AddScoped<EmailService>();

            builder.Services.AddMemoryCache();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // bật CORS
            app.UseCors("AllowAll");

            app.MapControllers();

            app.Run();
        }
    }
}
