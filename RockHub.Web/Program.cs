using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RockHub.Web;
using RockHub.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped<CookieHandler>();
builder.Services.AddScoped<ArtistasAPI>();
builder.Services.AddScoped<MusicasAPI>();
builder.Services.AddScoped<GeneroAPI>();
builder.Services.AddScoped<AuthAPI>();

builder.Services.AddHttpClient("API", client => {
    client.BaseAddress = new Uri(builder.Configuration["APIServer:Url"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<CookieHandler>();

await builder.Build().RunAsync();
