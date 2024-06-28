namespace RpsGameApi.Models{
    public class RpsGame
    {
        public int Id { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public Move? Player1Move { get; set; }
        public Move? Player2Move { get; set; }
        public RpsGameState State { get; set; }
        public string Winner { get; set; }
    }
}