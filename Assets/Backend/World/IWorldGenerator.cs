namespace BlockGame.Backend
{
    public interface IWorldGenerator
    {
        void Initialize ();
        void GenerateChunk (ref Chunk chunk);
    }
}