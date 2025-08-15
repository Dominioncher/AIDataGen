using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIDataGen.Core
{
    internal static class Constants
    {
        internal const string TmpPath = "GenAI/tmp/";

        internal const string ModelsPath = "GenAI/Models/";

        internal const string TextModelPath = "GenAI/Models/Gemma/";

        internal const string ImageModelPath = "GenAI/Models/SDTurbo/";

        internal const string TextModelRepo = "https://huggingface.co/google/gemma-3-4b-it-qat-q4_0-gguf/resolve/main/gemma-3-4b-it-q4_0.gguf";

        internal const string ImageModelRepo = "https://huggingface.co/onnxruntime/sdxl-turbo/resolve/main/";
    }
}
