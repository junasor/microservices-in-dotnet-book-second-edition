using Polly;
using ShoppingCart;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container
ConfigureServices(builder);
WebApplication application = builder.Build();

// Configure the HTTP request pipeline
ConfigureMiddleware(application);
await application.RunAsync();



void ConfigureServices(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.Scan(selector => selector
        .FromAssemblyOf<Program>()
        .AddClasses(c => c.Where(t => t != typeof(ProductCatalogClient) && t.GetMethods().All(m => m.Name != "<Clone>$")))
        .AsImplementedInterfaces());
    webApplicationBuilder.Services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
        .AddTransientHttpErrorPolicy(p =>
            p.WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => Console.WriteLine(ex.ToString())));
    webApplicationBuilder.Services.AddControllers();
}


void ConfigureMiddleware(WebApplication app)
{


    app.UseHttpsRedirection();
    app.UseRouting();

    app.MapControllers();
    app.UseRewriter();
}