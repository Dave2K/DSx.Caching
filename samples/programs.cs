using DSx.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Carica la configurazione
builder.Configuration.AddJsonFile("appsettings.json");

// Configura i provider di cache
builder.Services.AddConfiguredCacheProviders(builder.Configuration);

var app = builder.Build();

// Esempio di utilizzo
var cacheProvider = app.Services.GetRequiredService<ICacheProvider>();
await cacheProvider.SetAsync("test_key", "test_value");

app.Run();