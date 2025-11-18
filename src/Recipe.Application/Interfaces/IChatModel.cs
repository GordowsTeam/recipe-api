namespace Recipe.Application.Interfaces
{
    public interface IChatModel
    {
        Task<string> GetChatCompletionAsync(string systemPrompt, string userPrompt);
        Task<string> GetStructuredJsonAsync<T>(string systemPrompt, string userPrompt);
    }
}
