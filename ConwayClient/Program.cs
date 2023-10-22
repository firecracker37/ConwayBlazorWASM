using ConwayClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();

if (builder.HostEnvironment.IsDevelopment())
{
    var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
    await jsRuntime.InvokeVoidAsync("setBaseUrl", "/");
}
else
{
    var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
    await jsRuntime.InvokeVoidAsync("setBaseUrl", "/ConwayBlazorWASM/");
}

await host.RunAsync();
