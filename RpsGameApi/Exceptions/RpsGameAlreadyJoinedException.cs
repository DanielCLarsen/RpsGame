namespace RpsGameApi.Exceptions
{
    public class RpsGameAlreadyJoinedException : Exception
    {
        public RpsGameAlreadyJoinedException(int gameId)
            : base($"Game with ID '{gameId}' has already been joined by another player.")
        {
        }
    }
}