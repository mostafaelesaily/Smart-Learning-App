namespace Smart_Learning_App.Integrations.Ai
{
    public interface IAiService
    {
        Task<string> GenerateResponseAsync(string prompt);
    }
}
