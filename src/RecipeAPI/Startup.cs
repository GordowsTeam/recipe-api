using AWS.Logger;
using RecipeApp.Services;

namespace RecipeAPI;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRecipeAppServices(Configuration)
            .AddLogging(logging =>
            {
                logging.AddAWSProvider(new AWSLoggerConfig
                {
                    LogGroup = "recipe-logs",
                    Region = "us-east-1"//TODO: remove this hardcoded
                });
            })
            .AddCors(options =>
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
            })
            .AddControllers();
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