using AIDataGen.Core;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace AIDataGen.Models.Inference
{
    public class TextModel
    {
        public LLamaWeights Model { get; private set; }

        private LLamaContext _context;

        public async Task LoadModel()
        {
            // Load model
            var parameters = new ModelParams(Constants.TextModelPath)
            {
                ContextSize = 4096, // The longest length of chat as memory.
                GpuLayerCount = 5, // How many layers to offload to GPU. Please adjust it according to your GPU memory.
                BatchSize = 128
            };
            Model = await LLamaWeights.LoadFromFileAsync(parameters);
            _context = Model.CreateContext(parameters);
        }

        public TextModelSession CreateSession()
        {
            var executor = new InteractiveExecutor(_context);
            var history = new ChatHistory();
            var session = new ChatSession(executor, history);
            session.History.AddMessage(AuthorRole.System, Prompts.GetSystemChainOfThoughtPrompt());
            return new TextModelSession(_context, session);
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

            //Console.ForegroundColor = ConsoleColor.DarkGreen;
            //Console.WriteLine(prompt);
            //Console.ForegroundColor = ConsoleColor.White;

            var result = "";
            var chat = _session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), inferenceParams);
            await foreach (var text in chat)
            {
                result += text;
                //Console.Write(text);
            }

            //var start = result.IndexOf("<final_answer>") + 14;
            //var end = result.LastIndexOf("</final_answer>");

            var start = result.IndexOf("Assistant:") + 10;
            var end = result.Length - 1;

            result = result.Substring(start, end - start).Trim();

            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine(result);
            //Console.ForegroundColor = ConsoleColor.White;

            return result;
        }

    }
}
