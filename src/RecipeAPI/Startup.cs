using AWS.Logger;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Infrastructure.Services;
using Recipe.Infrastructure.Services.Edamame;

namespace RecipeAPI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient<EdamameRecipeService>("EdamameAPI", client => 
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddLogging(logging =>
        {
            logging.AddAWSProvider(new AWSLoggerConfig
            {
                LogGroup = "recipe-logs",
                Region = "us-east-1"//TODO: remove this hardcoded
            });
        });

        services.AddCors(options =>
        {
            options.AddPolicy("ProdCorsPolicy", builder =>
                builder.WithOrigins("https://*.oursite.com", "https://*.anothersite.com")
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            options.AddPolicy("NonProdCorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IThirdPartyRecipeService, EdamameRecipeService>();
        services.Configure<EdamameAPISettings>(Configuration.GetSection("EdamameAPISettings"));
        services.AddSingleton<EdamameAPISettings>();
        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors(env.IsProduction() ? "ProdCorsPolicy" : "NonProdCorsPolicy");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}