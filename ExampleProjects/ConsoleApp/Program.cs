using AIDataGen;
using TestAIGenConsoleApp;



// Cache and load models
AIGen.CacheModels = true;
AIGen.DownloadModels("<API key>");



var generator = AIGen.GetGenerator();
var cars = generator.Generate<Car>(5, "Get only Ferrari car collection");
var list = new List<Car>();
await foreach (var car in cars)
{
    list.Add(car);
}