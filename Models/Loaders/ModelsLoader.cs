using AIDataGen.Core;

namespace AIDataGen.Models.Loaders
{
    internal static class ModelsLoader
    {
        private static async Task DownloadGemma(string token)
        {
            if (Directory.Exists(Constants.TextModelPath))
            {
                return;
            }

            var progressBar = new ConsoleProgressBar("Download LLM model...");
            await LoadFileFromRepo(Constants.TextModelRepo, "", "Gemma/model.gguf", progressBar, token);
            Directory.Move(Constants.TmpPath + "Gemma", Constants.TextModelPath);
        }

        private static async Task DownloadJuggernaut()
        {
            if (Directory.Exists(Constants.ImageModelPath))
            {
                return;
            }

            var progressBar = new ConsoleProgressBar("Download TextToImage model...", 6);
            await LoadFileFromRepo(Constants.ImageModelRepo, "text_encoder/model.onnx", "Juggernaut/text_encoder/model.onnx", progressBar);
            await LoadFileFromRepo(Constants.ImageModelRepo, "unet/model.onnx", "Juggernaut/unet/model.onnx", progressBar);
            await LoadFileFromRepo(Constants.ImageModelRepo, "vae_decoder/model.onnx", "Juggernaut/vae_decoder/model.onnx", progressBar);
            await LoadFileFromRepo(Constants.ImageModelRepo, "vae_encoder/model.onnx", "Juggernaut/vae_encoder/model.onnx", progressBar);
            Directory.Move(Constants.TmpPath + "Juggernaut", Constants.ImageModelPath);
        }

        private static async Task LoadFileFromRepo(string repo, string repoPath, string localPath, ConsoleProgressBar progress, string token = null)
        {
            localPath = Constants.TmpPath + localPath;
            Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            var loader = new FilesLoader(token);
            await loader.DownloadFileAsync(repo + repoPath + "?download=true", localPath, progress.GetProgress());
        }

        public static void DownloadModels(string token)
        {            
            Directory.CreateDirectory(Constants.TmpPath);
            Directory.CreateDirectory(Constants.ModelsPath);
            DownloadGemma(token).Wait();
            DownloadJuggernaut().Wait();
        }
    }
}
