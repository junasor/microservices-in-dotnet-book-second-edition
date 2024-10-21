WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container
ConfigureServices(builder);
WebApplication application = builder.Build();

// Configure the HTTP request pipeline
ConfigureMiddleware(application);
await application.RunAsync();



void ConfigureServices(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.Scan(selector => selector.FromAssemblyOf<Program>()
        .AddClasses((c => c.Where(t => t.GetMethods().All(m => m.Name != "<Clone>$"))))
        .AsImplementedInterfaces());
   
    webApplicationBuilder.Services.AddControllers();
}


void ConfigureMiddleware(WebApplication app)
{
    app.UseHttpsRedirection();
    app.UseRouting();

    app.MapControllers();
    app.UseRewriter();
}
