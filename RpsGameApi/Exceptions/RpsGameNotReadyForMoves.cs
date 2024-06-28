namespace RpsGameApi.Exceptions
{
    public class RpsGameNotReadyForMoves : Exception
    {
        public RpsGameNotReadyForMoves(int gameId)
            : base($"Game with ID '{gameId}' is not ready for moves to be made. Likely awaiting second player.")
        {
        }
    }
}