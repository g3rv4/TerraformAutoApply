using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using AutoApply.Models;
using Jil;
using System.Linq;
using PipelinesAgentManager;
using System;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AutoApply
{

    public class Function
    {
        private static string GetEnvVar(string name) =>
            Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            try
            {
                var notification = JSON.Deserialize<TerraformNotification>(apigProxyEvent.Body, Options.ISO8601CamelCase);
                if (notification.Notifications.Any(n => n.Trigger == "run:needs_attention"))
                {
                    var terraformToken = GetEnvVar("TERRAFORM_TOKEN");
                    Provisioner.Init(terraformToken, null, null);
                    var response = await Provisioner.ApplyTerraformRunAsync(notification.RunId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return new APIGatewayProxyResponse
            {
                StatusCode = 201
            };
        }
    }
}
