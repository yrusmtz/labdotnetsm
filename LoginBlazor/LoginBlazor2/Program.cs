using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LoginBlazor2;
using Blazored.LocalStorage;
using LoginBlazor2.Security.Services;
using LoginShared.Security.Entities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

if (builder.HostEnvironment.IsDevelopment())
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5001/") });
}
else
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
}

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHttpClient<UserService>(); 
builder.Services.AddHttpClient<RoleService>();
builder.Services.AddHttpClient<UserRoleService>();
builder.Services.AddHttpClient<PantallaRoleService>();
builder.Services.AddHttpClient<PatrocinadorRoleService>();
builder.Services.AddHttpClient<SucursalRoleService>();
await builder.Build().RunAsync();
