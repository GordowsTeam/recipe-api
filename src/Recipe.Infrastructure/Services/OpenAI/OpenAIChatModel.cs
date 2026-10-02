using OpenAI.Chat;
using Recipe.Application.Exceptions;
using Recipe.Application.Interfaces;
using System.ClientModel;

namespace Recipe.Infrastructure.Services.OpenAI
{
    public class OpenAIChatModel : IChatModel
    {
        private const int TooManyRequestsStatus = 429;

        private readonly ChatClient _chatClient;

        public OpenAIChatModel(ChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public async Task<string> GetChatCompletionAsync(string systemPrompt, string userPrompt)
        {
            var messages = new ChatMessage[]
            {
                ChatMessage.CreateSystemMessage(systemPrompt),
                ChatMessage.CreateUserMessage(userPrompt)
            };

            var response = await CompleteChatSafeAsync(messages);
            var text = response?.Content.FirstOrDefault()?.Text;
            return text ?? string.Empty;
        }

        public async Task<string> GetStructuredJsonAsync<T>(string systemPrompt, string userPrompt)
        {
            var messages = new ChatMessage[]
            {
                ChatMessage.CreateSystemMessage(systemPrompt),
                ChatMessage.CreateUserMessage(userPrompt)
            };

            var response = await CompleteChatSafeAsync(messages);
            var text = response?.Content.ToString();
            return text ?? string.Empty;
        }

        private async Task<ChatCompletion?> CompleteChatSafeAsync(ChatMessage[] messages)
        {
            try
            {
                var response = await _chatClient.CompleteChatAsync(messages);
                return response.Value;
            }
            catch (ClientResultException ex) when (ex.Status == TooManyRequestsStatus)
            {
                throw new AiQuotaExceededException(
                    "The OpenAI account has no remaining quota/credits (HTTP 429).", ex);
            }
            catch (ClientResultException ex)
            {
                Console.WriteLine($"[OpenAIChatModel] OpenAI request failed with status {ex.Status}: {ex.Message}");
                return null;
            }
        }
    }
}
