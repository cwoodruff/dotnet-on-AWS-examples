using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace WeatherSampleLambda;

public class Function
{
    private static readonly string[] WEATHER_TYPES = { "Sunny", "Rainy", "Cloudy", "Windy", "Snowy" };
    private static readonly Random RANDOM = new Random();

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        string report;
        string? zipCode = request?.Body;
        
        
        // Validate the zip code (this is a basic validation, you should adjust as needed)
        if (string.IsNullOrWhiteSpace(zipCode) || zipCode.Length != 5)
        {
            report = $"Invalid zip code: {zipCode}";
        }
        else
        {
            // Pick a random weather type
            string weather = WEATHER_TYPES[RANDOM.Next(WEATHER_TYPES.Length)];

            // Generate a random temperature between -10 and 35 Celsius
            int temperature = RANDOM.Next(-10, 35);

            // Create a weather report
            report = $"The weather in {zipCode} is currently {weather} with a temperature of {temperature} degrees Celsius.";
        }
        
        var body = new Dictionary<string, string>
        {
            { "message", "Weather Report" },
            { "report", report }
        };

        return new APIGatewayProxyResponse
        {
            Body = JsonSerializer.Serialize(body),
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}