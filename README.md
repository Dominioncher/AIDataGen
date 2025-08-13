# AIDataGen Package

Simple realistic data generator

### Usage:
```
AIGen.DownloadModels("<your huggingface API key>");
var generator = AIGen.GetGenerator();
var cars = generator.Generate<Car>(10);
await foreach (var car in cars)
{
    Console.WriteLine(car.Model);
}


[Prompt("car")]
public class Car
{
    [Prompt("car model name")]
    public string Model { get; set; }

    [Prompt("car brand name")]
    public string Brand { get; set; }

    [Prompt("car description")]
    public string Description { get; set; }
}
```