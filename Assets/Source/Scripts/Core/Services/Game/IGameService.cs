namespace Core.Services
{
    public interface IGameService : IService
    {
        LevelConfig CurrentLevel { get; }
    }
}