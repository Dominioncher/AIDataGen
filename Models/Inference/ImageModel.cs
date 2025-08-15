using AIDataGen.Core;
using OnnxStack.Core.Image;
using OnnxStack.StableDiffusion.Config;
using OnnxStack.StableDiffusion.Enums;
using OnnxStack.StableDiffusion.Pipelines;
using SixLabors.ImageSharp;

namespace AIDataGen.Models.Inference
{
    internal class ImageModel
    {
        public StableDiffusionPipeline Model { get; private set; }

        public void LoadModel()
        {
            Model = StableDiffusionXLPipeline.CreatePipeline(Constants.ImageModelPath, ModelType.Turbo, 0, OnnxStack.Core.Config.ExecutionProvider.Cuda);
        }

        public async Task<Image> Generate(string prompt)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = prompt,
                DiffuserType = DiffuserType.TextToImage                
            };

            var image = await Model.GenerateImageAsync(promptOptions);
            return image.GetImage();
        }

        public async IAsyncEnumerable<Image> Generate(string prompt, int count)
        {
            var promptOptions = new PromptOptions
            {
                Prompt = prompt,
                DiffuserType = DiffuserType.TextToImage                
            };

            var batchOptions = new BatchOptions
            {
                ValueTo = count,
                BatchType = BatchOptionType.Seed
            };

            await foreach (var result in Model.RunBatchAsync(batchOptions, promptOptions))
            {
                yield return new OnnxImage(result.Result).GetImage();
            }
        }
    }
}
