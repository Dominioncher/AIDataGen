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

        private static async Task DownloadSDTurbo()
        {
            if (Directory.Exists(Constants.ImageModelPath))
            {
                return;
            }

            var progressBar = new ConsoleProgressBar("Download TextToImage model...", 7);
            await LoadFileFromRepo(Constants.ImageModelRepo, "text_encoder/model.onnx", "SDTurbo/text_encoder/model.onnx", progressBar);
            await LoadFileFromRepo(Constants.ImageModelRepo, "text_encoder_2/model.onnx", "SDTurbo/text_encoder_2/model.onnx", progressBar);
            await LoadFileFromRepo(Constants.ImageModelRepo, "text_encoder_2/model.onnx.data", "SDTurbo/text_encoder_2/model.onnx.data", progressBar);
            await LoadFileFromRepo(Constants.ImageModelRepo, "unet/model.onnx", "SDTurbo/unet/model.onnx", progressBar);
            await LoadFileFromRepo(Constants.ImageModelRepo, "unet/model.onnx.data", "SDTurbo/unet/model.onnx.data", progressBar);
            await LoadFileFromRepo(Constants.ImageModelRepo, "vae_decoder/model.onnx", "SDTurbo/vae_decoder/model.onnx", progressBar);
            await LoadFileFromRepo(Constants.ImageModelRepo, "vae_encoder/model.onnx", "SDTurbo/vae_encoder/model.onnx", progressBar);
            Directory.Move(Constants.TmpPath + "SDTurbo", Constants.ImageModelPath);
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
            DownloadSDTurbo().Wait();
        }
    }
}
