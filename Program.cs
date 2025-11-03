using ControleDespesas.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor;
using MudBlazor.Services;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Conexão com PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // Prevent JsonSerializer from throwing on cycles between navigation properties
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // Optional: don't emit null properties
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });  // Adiciona suporte a controllers

// Register HttpClient for Blazor Server components so they can inject HttpClient.
// Prerendering contexts may not provide NavigationManager, so register a simple
// fallback HttpClient using a configured base URL. You can override "AppBaseUrl"
// in appsettings.json or leave the default localhost address.
builder.Services.AddHttpClient();
var defaultBase = builder.Configuration.GetValue<string>("AppBaseUrl") ?? "http://localhost:5184";
builder.Services.AddScoped(sp => new System.Net.Http.HttpClient { BaseAddress = new Uri(defaultBase) });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Defina o caminho de login
        options.AccessDeniedPath = "/accessdenied"; // Defina o caminho de logout
    });

builder.Services.AddAuthorization();
// Swagger: registrar gerador e explorer de endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MudBlazor services (Snackbar/toasts)
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Em Development, habilita página de exceção e Swagger
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ControleDespesas API V1");
        // opcional: abrir a UI na raiz /swagger
        options.RoutePrefix = "swagger"; 
    });
}
else
{
    // Em produção, manter tratamento de exceções e HSTS
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();  // Mapeia as rotas dos controllers
app.UseStaticFiles();
app.UseRouting();

// Se você tiver controllers/api controllers, habilite MapControllers();
// app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
