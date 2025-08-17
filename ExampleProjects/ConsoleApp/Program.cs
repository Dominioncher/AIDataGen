using AIDataGen;
using SixLabors.ImageSharp;
using System.Diagnostics;
using TestAIGenConsoleApp;



// Load models
AIGen.DownloadModels("<API KEY>");



var generator = AIGen.GetGenerator();
var cars = generator.Generate<Car>(3);
var time = new Stopwatch();
await foreach (var car in cars)
{    
    Console.WriteLine(car.Description);
    car.Reviews[0].Photo.SaveAsJpeg(car.Model + ".jpeg");
    for (int i = 0; i < car.Reviews[0].Photos.Count; i++)
    {
        car.Reviews[0].Photos[i].SaveAsJpeg(car.Model + i + ".jpeg");
    }
}