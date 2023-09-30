using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FlatFinding.Data;
using FlatFinding.Models;
using Microsoft.AspNetCore.Identity;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FlatFindingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FlatFindingContext") ?? throw new InvalidOperationException("Connection string 'FlatFindingContext' not found.")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<FlatFindingContext>();
 builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
