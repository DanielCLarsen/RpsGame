namespace RpsGameApi.Exceptions
{
    public class RpsGameNotFoundException : Exception
    {
        public RpsGameNotFoundException(int gameId)
            : base($"Game with ID '{gameId}' was not found")
        {
        }
    }
}