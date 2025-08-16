using AIDataGen;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using TestAIGenConsoleApp;



var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var key = config["API_KEY"];
AIGen.DownloadModels(key);


var generator = AIGen.GetGenerator();
var cars = generator.Generate<Movie>(2);
var time = new Stopwatch();
await foreach (var car in cars)
{    
    Console.WriteLine(car.Description);
}