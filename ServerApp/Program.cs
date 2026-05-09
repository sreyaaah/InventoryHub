using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Activity 4: Added Memory Cache service for performance optimization
builder.Services.AddMemoryCache();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Use CORS
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.MapGet("/api/productlist", (IMemoryCache cache) =>
{
    // Activity 4: Performance optimization using caching strategy
    const string cacheKey = "productList";

    if (!cache.TryGetValue(cacheKey, out object? products))
    {
        // Activity 3: Returning structured JSON with nested Category
        products = new[]
        {
            new
            {
                Id = 1,
                Name = "Laptop",
                Price = 1200.50,
                Stock = 25,
                Category = new { Id = 101, Name = "Electronics" }
            },
            new
            {
                Id = 2,
                Name = "Headphones",
                Price = 50.00,
                Stock = 100,
                Category = new { Id = 102, Name = "Accessories" }
            }
        };

        // Cache for 5 minutes
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));

        cache.Set(cacheKey, products, cacheOptions);
        
        // Copilot efficiency improvement: Minimal API DI for IMemoryCache
        Console.WriteLine("Cache missed. Fetching fresh data.");
    }
    else
    {
        Console.WriteLine("Cache hit. Returning cached data.");
    }

    return products;
});

app.Run();
