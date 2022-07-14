namespace Core.Services
{
    public interface ISocialService : IService
    {
        void PostScore(int score);
        void ShowLeaderBoard();
    }
}
