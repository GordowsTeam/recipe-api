using OpenAI.Chat;
using Recipe.Application.Interfaces;

namespace Recipe.Infrastructure.Services.OpenAI
{
    public class OpenAIChatModel : IChatModel
    {
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

            var response = await _chatClient.CompleteChatAsync(messages);
            var text = response.Value.Content.FirstOrDefault()?.Text;
            return text ?? string.Empty;
        }

        public async Task<string> GetStructuredJsonAsync<T>(string systemPrompt, string userPrompt)
        {
            var messages = new ChatMessage[]
            {
                ChatMessage.CreateSystemMessage(systemPrompt),
                ChatMessage.CreateUserMessage(userPrompt)
            };

            var response = await _chatClient.CompleteChatAsync(messages);
            var text = response.Value.Content.ToString();
            return text ?? string.Empty;
        }
    }
}
