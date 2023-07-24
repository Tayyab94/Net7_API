namespace LearnAPI_Net7.Services
{
    public interface IRefreshHandler
    {
        Task<String> GenerateToken(string userName);
    }
}
