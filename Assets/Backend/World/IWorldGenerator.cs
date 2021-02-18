namespace BlockGame.Backend.World
{
    public interface IWorldGenerator
    {
        void Initialize ();
        void GenerateChunk (ref Chunk chunk);
    }
}