using Microsoft.Extensions.Options;
using NETCore.MailKit.Core;
using OfficeOpenXml;
using ScoreManagementSystem.Models;
using ScoreManagementSystem.Services;
using System.Configuration;

namespace ScoreManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<Prn221Context>(option =>
            {
                option.EnableSensitiveDataLogging();
            });
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}