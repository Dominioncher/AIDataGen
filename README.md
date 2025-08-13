# AIDataGen Package

Realistic data generator

```csharp
var cars = generator.Generate<Car>(10);
```


### Base usage guide:
Step 1: Create classes that you want to generate

Step 2: Mark classes and properties with attributes

```csharp
[Prompt("Cars")]
public class Car
{
    [Prompt("Brand name")]
    public string Brand { get; set; }

    [Prompt("Model name")]
    public string Model { get; set; }

    [Prompt("Manufacturer company that develop car")]
    public Company Company { get; set; }

    [Prompt("Description")]
    public string Description { get; set; }

    [Random(3, 5)]
    [Prompt("Reviews from automobile journals", "Get only review text")]
    public List<Review> Reviews { get; set; }
}

```

Step 3: Download models with your Huggingface API key (https://huggingface.co/docs/hub/security-tokens)

```csharp
AIGen.DownloadModels("<your huggingface API key>");
```

Step 4: Create generator and call generate method

```csharp
var generator = AIGen.GetGenerator();
var cars = generator.Generate<Car>(10);
await foreach (var car in cars)
{
    Console.WriteLine(car.Model);
}

```