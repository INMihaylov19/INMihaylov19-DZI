using Microsoft.EntityFrameworkCore;
using TMS.Data.Data;
using TMS.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using TMS.Services.Implementations;
using Microsoft.Extensions.Configuration;
using TMS.Services.Models;
using TMS.Data.Enums;

namespace TMS.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Add services to the container.

            builder.Services.AddDbContext<TMSContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")!, o =>
                {
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    o.MigrationsAssembly(typeof(Program).Assembly.FullName);
                });
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
                options.EnableThreadSafetyChecks();
            });

            builder.Services.AddIdentity<TMS.Data.Models.User, IdentityRole>()
                    .AddEntityFrameworkStores<TMSContext>()
                    .AddDefaultTokenProviders();
            builder.Services.AddMemoryCache();
            builder.Services.AddSession();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "TMS";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequiredAdmin", policy => policy.RequireClaim("Permission", "IsAdmin"));
                options.AddPolicy("RequiredEmployee", policy => policy.RequireClaim("Permission", "IsEmployee"));
                options.AddPolicy("RequiredEmployer", policy => policy.RequireClaim("Permission", "IsEmployer"));
                options.AddPolicy("RequiredEmployeeAdminEmployer", policy => policy.RequireRole(nameof(UserRole.Employee), nameof(UserRole.Admin), nameof(UserRole.Employer)));
                options.AddPolicy("RequiredAdminEmployer", policy => policy.RequireRole(nameof(UserRole.Admin), nameof(UserRole.Employer)));
            });

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(typeof(TMS.WebHost.Models.Mapper));
            builder.Services.AddScoped<IGroupService, TMS.Services.Implementations.GroupService>();
            builder.Services.AddScoped<ITaskService, TMS.Services.Implementations.TaskService>();
            builder.Services.AddScoped<IUserService, TMS.Services.Implementations.UserService>();
            builder.Services.AddTransient<IEmailSender, EmailService>();
            builder.Services.AddTransient<IPDFDownloader, PDFDownloader>();
            builder.Services.AddScoped<ICurrentUser, CurrentUser>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCookiePolicy();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();

        }
    }
}