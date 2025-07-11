using Microsoft.EntityFrameworkCore;
using Sport_centrum;
using SportCentrum.Configuration;
using SportCentrum.Context;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SportCentrumContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Connection")));

var app = builder.Build();

using(var scope  = app.Services.CreateScope())
{
    var context =scope.ServiceProvider.GetService<SportCentrumContext>();
    
    //Muzete pouzit tuto metodu pro cisteni tabulky rezervaci
    //DbInitializer.ClearOldReservations(context);
    DbSeeder.Seed(context, "Data/data.xml");
    
}

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
