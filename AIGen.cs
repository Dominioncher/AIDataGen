using AIDataGen.Core;
using AIDataGen.Models.Loaders;
using LLama.Native;

namespace AIDataGen
{
    public static class AIGen
    {
        public static bool CacheModels { get; set; } = false;

        static AIGen() 
        {
            NativeLogConfig.llama_log_set((level, message) =>
            {
            });
        }

        public static void DownloadModels(string token)
        {
            ModelsLoader.DownloadModels(token);
        }

        public static Generator GetGenerator()
        {
            return new Generator();
        }

    }
}
