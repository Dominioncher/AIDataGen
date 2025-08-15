using AIDataGen.Core;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace AIDataGen.Models.Inference
{
    public class TextModel
    {
        public LLamaWeights Model { get; private set; }

        private ModelParams _parameters;

        public async Task LoadModel()
        {
            // Load model
            _parameters = new ModelParams(Constants.TextModelPath + "model.gguf")
            {
                ContextSize = 8192, // The longest length of chat as memory.
                GpuLayerCount = -1, // How many layers to offload to GPU. Please adjust it according to your GPU memory.
                BatchSize = 128
            };
            Model = await LLamaWeights.LoadFromFileAsync(_parameters);
        }

        public TextModelSession CreateSession()
        {
            var context = Model.CreateContext(_parameters);
            var executor = new InteractiveExecutor(context);
            var history = new ChatHistory();
            var session = new ChatSession(executor, history);
            session.History.AddMessage(AuthorRole.System, Prompts.GetSystemSimplePrompt());
            return new TextModelSession(context, session);
        }
    }

    public class TextModelSession : IDisposable
    {
        private LLamaContext _context;

        private ChatSession _session;

        public TextModelSession(LLamaContext context, ChatSession session)
        {
            _session = session;
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<string> Generate(string prompt)
        {
            // Our optimized inference parameters
            var inferenceParams = new InferenceParams
            {
                SamplingPipeline = new DefaultSamplingPipeline()
                {
                    // Balance of creativity and coherence
                    Temperature = 0.7f,
                    TopP = 0.9f,
                    TopK = 40,

                    // Anti-repetition measures
                    RepeatPenalty = 1.1f,
                    PenaltyCount = 64,
                    FrequencyPenalty = 0.02f,
                    PresencePenalty = 0.01f,

                },

                // Generation limits
                MaxTokens = 2048,
                AntiPrompts = new List<string> { "###", "User:", "\n\n\n" },
            };

            var result = "";
            var chat = _session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), inferenceParams);
            await foreach (var text in chat)
            {
                result += text;
            }

            return result;
        }

    }
}
