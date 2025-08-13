using AIDataGen.Core.Attributes;
using AIDataGen.Models.Inference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AIDataGen.Core
{
    public class Generator
    {
        private TextModel _model;

        public Generator()
        {
            _model = new TextModel();
            _model.LoadModel().Wait();
        }

        public async IAsyncEnumerable<T> Generate<T>(int count, string context = null) where T : new()
        {
            if (count <= 0)
            {
                throw new Exception("Count must be > 0");
            }

            var type = typeof(T);
            var attribute = type.GetCustomAttribute<PromptAttribute>();
            var prompt = Prompts.GetListFirstElementGenerationPrompt(attribute.Description, attribute.Modifiers, context);

            using var session = _model.CreateSession();

            var modelResults = new List<string>();
            modelResults.Add(await session.Generate(prompt));

            for (int i = 1; i < count; i++)
            {
                prompt = Prompts.GetListNextElementGenerationPrompt(attribute.Description, attribute.Modifiers, context);
                modelResults.Add(await session.Generate(prompt));
            }

            foreach (var result in modelResults)
            {
                var element = await GetInstanceFromModelResultText(type, result, session);
                yield return (T)element;
            }
        }


        #region Generation rules

        private async ValueTask<object> GenerateProperty(PropertyInfo info, string context, TextModelSession session)
        {
            if (info.PropertyType.IsCollection())
            {
                return await GenerateCollection(info, context, session);
            }

            return await GenerateObject(info, context, session);
        }

        private async ValueTask<object> GenerateObject(PropertyInfo info, string context, TextModelSession session)
        {
            var attribute = info.GetCustomAttribute<PromptAttribute>();
            var prompt = Prompts.GetObjectGenerationPrompt(attribute.Description, attribute.Modifiers, context);
            var modelResult = await session.Generate(prompt);
            return await GetInstanceFromModelResultText(info.PropertyType, modelResult, session);
        }

        private async ValueTask<object> GenerateCollection(PropertyInfo info, string context, TextModelSession session)
        {
            var randomAttribute = info.GetCustomAttribute<RandomAttribute>();
            var rnd = new Random();
            var collectionCount = rnd.Next((int)randomAttribute.Min, (int)randomAttribute.Max);
            var collectionInstance = Activator.CreateInstance(info.PropertyType);
            var attribute = info.GetCustomAttribute<PromptAttribute>();
            var elementType = info.PropertyType.GetGenericArguments()[0];

            if (collectionCount <= 0)
            {
                return collectionInstance;
            }

            // Generate first element
            var prompt = Prompts.GetListFirstElementGenerationPrompt(attribute.Description, attribute.Modifiers, context);
            var modelResults = new List<string>();
            modelResults.Add(await session.Generate(prompt));

            // Generate next elements
            for (int i = 1; i < collectionCount; i++)
            {
                prompt = Prompts.GetListNextElementGenerationPrompt(attribute.Description, attribute.Modifiers, context);
                modelResults.Add(await session.Generate(prompt));
            }

            // Generate all elements initital data
            foreach (var result in modelResults)
            {
                var element = await GetInstanceFromModelResultText(elementType, result, session);
                info.PropertyType.GetMethod("Add").Invoke(collectionInstance, [element]);
            }

            return collectionInstance;
        }

        private async ValueTask<object> GetInstanceFromModelResultText(Type type, string modelResult, TextModelSession session)
        {
            if (type == typeof(string))
            {
                return modelResult.Trim().Replace("\n", "").Replace("\"", "");
            }

            var instance = Activator.CreateInstance(type);
            foreach (var property in type.GetProperties())
            {
                var propertyInstance = await GenerateProperty(property, modelResult, session);
                property.SetValue(instance, propertyInstance);
            }

            return instance;
        }

        #endregion
    }
}
