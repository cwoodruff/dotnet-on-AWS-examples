using System.Text.Json;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using WeatherSampleLambda;
using Xunit.Abstractions;

namespace WEatherSampleLambda.Test;

public class FunctionTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private static readonly HttpClient client = new HttpClient();

    public FunctionTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private static async Task<string> GetCallingIP()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

        var stringTask = client.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext:false);

        var msg = await stringTask;
        return msg.Replace("\n","");
    }

    [Fact]
    public async Task TestWeatherSampleFunctionNoZipCodeHandler()
    {
        var request = new APIGatewayProxyRequest();
        var context = new TestLambdaContext();
        Dictionary<string, string> body = new Dictionary<string, string>
        {
            { "message", "Weather Report" },
            { "report", "Invalid zip code: " },
        };
        
        var expectedResponse = new APIGatewayProxyResponse
        {
            Body = JsonSerializer.Serialize(body),
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };

        var function = new Function();
        var response = await function.FunctionHandler(request, context);

        _testOutputHelper.WriteLine("Lambda Response: \n" + response.Body);
        _testOutputHelper.WriteLine("Expected Response: \n" + expectedResponse.Body);

        Assert.Equal(expectedResponse.Body, response.Body);
        Assert.Equal(expectedResponse.Headers, response.Headers);
        Assert.Equal(expectedResponse.StatusCode, response.StatusCode);
    }
}