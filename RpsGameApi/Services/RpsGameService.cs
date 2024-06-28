using System.Collections.Concurrent;
using RpsGameApi.Models;
using RpsGameApi.Exceptions;

namespace RpsGameApi.Services
{
    public class RpsGameService
    {
        private readonly ConcurrentDictionary<int, RpsGame> _games = new();
        private static int _nextGameId = 0;

        public async Task<RpsGame> CreateRpsGameAsync(string player1)
        {
            var game = new RpsGame 
            { 
                Id = _nextGameId++, 
                Player1 = player1, 
                State = RpsGameState.WaitingForPlayer
            };
            _games[game.Id] = game;
            return await Task.FromResult(game);
        }

        public async Task<RpsGame> JoinRpsGameAsync(int gameId, string playerName)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                if (game.State != RpsGameState.WaitingForPlayer && game.Player2 == playerName)
                {
                    throw new RpsGameAlreadyJoinedException(gameId);
                }
                else if (game.State != RpsGameState.WaitingForPlayer){
                    throw new RpsGameIsNotJoinableException(gameId);
                }

                game.Player2 = playerName;
                game.State = RpsGameState.WaitingForMoves;
                return await Task.FromResult(game);
            }
            throw new RpsGameNotFoundException(gameId);
        }

        public async Task<RpsGame> MakeMoveAsync(int gameId, string playerName, Move move)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                if (game.State != RpsGameState.WaitingForMoves)
                {
                    throw new RpsGameNotReadyForMoves(gameId);
                }

                if (game.Player1 == playerName)
                {
                    game.Player1Move = move;
                }
                else if (game.Player2 == playerName)
                {
                    game.Player2Move = move;
                }

                if (game.Player1Move.HasValue && game.Player2Move.HasValue)
                {
                    game.State = RpsGameState.Completed;
                    game.Winner = DetermineWinner(game);
                }
                return await Task.FromResult(game);
            }
            throw new RpsGameNotFoundException(gameId);
        }

        public async Task<RpsGame> GetRpsGameAsync(int gameId)
        {
            if (_games.TryGetValue(gameId, out var game))
            {
                return await Task.FromResult(game);
            }
            throw new RpsGameNotFoundException(gameId);
        }

        private string DetermineWinner(RpsGame game)
        {
            if (game.Player1Move == game.Player2Move) return "Tie";

            if ((game.Player1Move == Move.Rock && game.Player2Move == Move.Scissors) ||
                (game.Player1Move == Move.Scissors && game.Player2Move == Move.Paper) ||
                (game.Player1Move == Move.Paper && game.Player2Move == Move.Rock))
            {
                return game.Player1;
            }
            return game.Player2;
        }

        public RpsGameResponse ConvertRpsGameToResponse(RpsGame rpsGame)
        {
            var response = new RpsGameResponse
            {
                Id = rpsGame.Id,
                Player1 = rpsGame.Player1,
                Player2 = rpsGame.Player2,
                Player1Move = rpsGame.Player1Move?.ToString(),
                Player2Move = rpsGame.Player2Move?.ToString(),
                State = rpsGame.State.ToString(),
                Winner = rpsGame.Winner
            };
            return response;
        }
    }
}
