using AIDataGen.Core;

namespace AIDataGen.Models.Loaders
{
    internal static class ModelsLoader
    {
        private static async Task DownloadGemma(string token)
        {
            // Check model already download if use cache
            if (AIGen.CacheModels && File.Exists(Constants.TextModelPath))
            {
                return;
            }

            // Load model to TMP dir
            var modelUrl = "https://huggingface.co/google/gemma-3-4b-it-qat-q4_0-gguf/resolve/main/gemma-3-4b-it-q4_0.gguf?download=true";
            var localPath = Constants.TmpPath + "gemma-3-4b-it-q4_0.gguf";
            var loader = new FilesLoader(token);
            var progressBar = new ConsoleProgressBar(message: $"Download LLM model...");
            await loader.DownloadFileAsync(modelUrl, localPath, progressBar.GetProgress());

            // Move model to Models dir
            File.Move(localPath, Constants.TextModelPath, true);
        }

        public static void DownloadModels(string token)
        {            
            Directory.CreateDirectory(Constants.TmpPath);
            Directory.CreateDirectory(Constants.ModelsPath);

            var gemma = DownloadGemma(token);
            Task.WaitAll(gemma);
        }
    }
}
