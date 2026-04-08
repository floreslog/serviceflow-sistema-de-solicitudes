using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceFlow.Class.Data;
using ServiceFlow.Class.Models;
using ServiceFlow.Class.Repositories;

var builder = WebApplication.CreateBuilder(args);

// AGREGAR LOS SERVICIOS QUE VAYA NECESITANDO.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ServiceFlowDB>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ServiceFlowDB>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
