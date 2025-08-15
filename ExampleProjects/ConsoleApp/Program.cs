using AIDataGen;
using System.Diagnostics;
using TestAIGenConsoleApp;



// Load models
AIGen.DownloadModels("<API key>");



var generator = AIGen.GetGenerator();
var cars = generator.Generate<Movie>(2);
var time = new Stopwatch();
await foreach (var car in cars)
{    
    Console.WriteLine(car.Description);
}