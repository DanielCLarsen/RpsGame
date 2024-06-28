namespace RpsGameApi.Exceptions
{
    public class RpsGameIsNotJoinableException : Exception
    {
        public RpsGameIsNotJoinableException(int gameId)
            : base($"Game with ID '{gameId}' is not joinable")
        {
        }
    }
}