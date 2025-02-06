using Microsoft.AspNetCore.Hosting;

namespace Recipe.API
{
    /// <summary>
    /// This class extends from APIGatewayProxyFunction which contains the method FunctionHandlerAsync which is the 
    /// actual Lambda function entry point. The Lambda handler field should be set to
    /// 
    /// Weather.Api::Weather.Api.LambdaEntryPoint::FunctionHandlerAsync
    /// </summary>
    public class LambdaEntryPoint :

        // The base class must be set to match the AWS service invoking the Lambda function. If not Amazon.Lambda.AspNetCoreServer
        // will fail to convert the incoming request correctly into a valid ASP.NET Core request.
        //
        // API Gateway REST API                         -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
        // API Gateway HTTP API payload version 1.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
        // API Gateway HTTP API payload version 2.0     -> Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction
        // Application Load Balancer                    -> Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction
        // 
        // Note: When using the AWS::Serverless::Function resource with an event type of "HttpApi" then payload version 2.0
        // will be the default and you must make Amazon.Lambda.AspNetCoreServer.APIGatewayHttpApiV2ProxyFunction the base class.

        Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .UseStartup<Program>();
        }

        protected override void Init(IHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                    .AddEnvironmentVariables();
                });
        }
    }
}
