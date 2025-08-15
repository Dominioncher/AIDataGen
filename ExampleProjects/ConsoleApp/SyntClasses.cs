using AIDataGen.Core.Attributes;
using OnnxStack.Core.Image;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAIGenConsoleApp
{
    [Prompt("Movies", "Get detailed description")]
    public class Movie
    {
        [Prompt("Movie Title")]
        public string Name { get; set; }

        [Prompt("Movie description")]
        public string Description { get; set; }

        [Prompt("Movie poster")]
        public Image Poster { get; set; }

        [Random(5, 10)]
        [Prompt("Movie screenshoots")]
        public List<Image> Screenshots { get; set; }
    }

    [Prompt("Companies")]
    public class Company
    {
        [Prompt("Company fullname")]
        public string Name { get; set; }

        [Prompt("Address where company headquater placed", "Get only full adress name")]
        public string Address { get; set; }

        [Prompt("Country name where company headquater placed", "Get only country name")]
        public string Country { get; set; }

        [Random(3, 5)]
        [Prompt("Products that company developed", "Get only product name")]
        public List<string> Products { get; set; } = new List<string>();

        [Random(3, 5)]
        [Prompt("Reviews about company ecological politic")]
        public List<Review> Reviews { get; set; }
    }

    [Prompt("Cars")]
    public class Car
    {
        [Random(3, 5)]
        [Prompt("Reviews from automobile journals", "Get only rewiew text")]
        public List<Review> Reviews { get; set; }

        [Prompt("Brand name")]
        public string Brand { get; set; }

        [Prompt("Model name")]
        public string Model { get; set; }

        [Prompt("Manufacturer company that develop car")]
        public Company Company { get; set; }

        [Prompt("Description")]
        public string Description { get; set; }

    }

    [Prompt("Reviews")]
    public class Review
    {
        [Prompt("Review author photoportrait", "Photorealistic", "Real photo")]
        public Image Photo { get; set; }

        [Random(5, 10)]
        [Prompt("Rewiew author car for daily usage", "Photorealistic", "Real photo")]
        public List<Image> Photos { get; set; }

        [Prompt("Review author name and surname", "You can create any name and surname", "Get only name and surname")]
        public string Author { get; set; }

        [Prompt("Review text", "Only 10 words")]
        public string Text { get; set; }

        [Prompt("Review raiting", "Number from 1 to 10")]
        public string Raiting { get; set; }
    }
}
