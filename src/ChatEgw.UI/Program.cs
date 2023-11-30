using ChatEgw.UI.Persistence;
using ChatEgw.UI.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services
    .AddMudServices()
    .AddMudPopoverService();

builder.Services
    .AddHttpClient()
    .AddDbContextFactory<SearchDbContext>(
        o => o.UseNpgsql(builder.Configuration.GetConnectionString("Database"), c => c.UseVector())
            .EnableSensitiveDataLogging());

builder.Services.AddApplicationPart();
builder.Services.AddScoped<TreeService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();