using RpsGameApi.Models;
using RpsGameApi.Services;
using Xunit;
using System.Threading.Tasks;
using RpsGameApi.Exceptions;

namespace RpsGameApi.Tests
{
    public class RpsGameServiceTests
    {
        private readonly RpsGameService _rpsGameService;

        public RpsGameServiceTests()
        {
            _rpsGameService = new RpsGameService();
        }

        public class CreateRpsGameAsyncTests : RpsGameServiceTests
        {
            [Fact]
            public async Task CreateRpsGameAsync_ShouldCreateRpsGameWithPlayer1AndWaitingForPlayerState()
            {
                // Assemble
                var player1Name = "Player1";
                var expectedState = RpsGameState.WaitingForPlayer;
                // Act
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                // Assert
                Assert.Equal(player1Name, rpsGame.Player1);
                Assert.Equal(expectedState, rpsGame.State);
            }
        }

        public class JoinRpsGameAsyncTests : RpsGameServiceTests
        {
            [Fact]
            public async Task JoinRpsGameAsync_ShouldUpdateGameToHavePlayer2AndWaitingForMovesState()
            {
                // Assemble
                var player1Name = "Player1";
                var player2Name = "Player2";
                var expectedState = RpsGameState.WaitingForMoves;
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                var gameId = rpsGame.Id;

                // Act
                var rpsGamePostJoin = await _rpsGameService.JoinRpsGameAsync(gameId, player2Name);

                // Assert
                Assert.Equal(player2Name, rpsGamePostJoin.Player2);
                Assert.Equal(player1Name, rpsGamePostJoin.Player1);
                Assert.Equal(expectedState, rpsGamePostJoin.State);
            }

            [Fact]
            public async Task JoinRpsGameAsync_ShouldThrowAlreadyJoinedException_IfAlreadyJoined()
            {
                // Assemble
                var player1Name = "Player1";
                var player2Name = "Player2";
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                var gameId = rpsGame.Id;

                // Act
                var rpsGamePostJoin = await _rpsGameService.JoinRpsGameAsync(gameId, player2Name);

                // Assert
                await Assert.ThrowsAsync<RpsGameAlreadyJoinedException>(async () =>
                {
                    await _rpsGameService.JoinRpsGameAsync(gameId, player2Name);
                });
            }

            [Fact]
            public async Task JoinRpsGameAsync_ShouldThrowRpsGameIsNotJoinableException_IfGameIsFull()
            {
                // Assemble
                var player1Name = "Player1";
                var player2Name = "Player2";
                var player3Name = "Player3";
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                var gameId = rpsGame.Id;

                // Act
                var rpsGamePostJoin = await _rpsGameService.JoinRpsGameAsync(gameId, player2Name);

                // Act and Assert
                await Assert.ThrowsAsync<RpsGameIsNotJoinableException>(async () =>
                {
                    await _rpsGameService.JoinRpsGameAsync(gameId, player3Name);
                });
            }

            [Fact]
            public async Task JoinRpsGameAsync_ShouldThrowRpsGameNotFoundException_IfGameNotFound()
            {
                // Assemble
                var player1Name = "Player1";
                var gameId = 99999999;

                // Act and Assert
                await Assert.ThrowsAsync<RpsGameNotFoundException>(async () =>
                {
                    await _rpsGameService.JoinRpsGameAsync(gameId, player1Name);
                });
            }
        }

        public class MakeMoveAsyncTests : RpsGameServiceTests
        {
            [Fact]
            public async Task MakeMoveAsync_ShouldThrowRpsGameNotReadyForMovesException_WhenGameNotReadyToAcceptMoves()
            {
                // Assemble
                var player1Name = "Player1";
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                var gameId = rpsGame.Id;
               
                // Act and Assert
                await Assert.ThrowsAsync<RpsGameNotReadyForMoves>(async () =>
                {
                    await _rpsGameService.MakeMoveAsync(gameId, player1Name, Move.Rock);
                });
            }

            [Fact]
            public async Task MakeMoveAsync_ShouldThrowRpsGameNotFoundException_WhenGameNotFound()
            {
                // Assemble
                var gameId = 999999999;
                var player1Name = "Player1";
                var rock = Move.Rock;
               
                // Act and Assert
                await Assert.ThrowsAsync<RpsGameNotFoundException>(async () =>
                {
                    await _rpsGameService.MakeMoveAsync(gameId, player1Name, rock);
                });
            }

            [Fact]
            public async Task MakeMoveAsync_ShouldUpdateGameWithTie_WhenBothPlayersUseRock()
            {
                // Assemble
                var player1Name = "Player1";
                var player2Name = "Player2";
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                var gameId = rpsGame.Id;
                var rock = Move.Rock;  
                var tie = "Tie";             

                // Act
                await _rpsGameService.JoinRpsGameAsync(gameId, player2Name);

                await _rpsGameService.MakeMoveAsync(gameId, player1Name, rock);
                var rpsGameUpdated = await _rpsGameService.MakeMoveAsync(gameId, player2Name, rock);

                // Assert
                Assert.Equal(rock, rpsGameUpdated.Player1Move);
                Assert.Equal(rock, rpsGameUpdated.Player2Move);
                Assert.Equal(RpsGameState.Completed, rpsGameUpdated.State);
                Assert.Equal(tie, rpsGameUpdated.Winner);
            }

            [Fact]
            public async Task MakeMoveAsync_ShouldUpdateGameWithPlayer1Winning_WhenBothPlayer1Wins()
            {
                // Assemble
                var player1Name = "Player1";
                var player2Name = "Player2";
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                var gameId = rpsGame.Id;
                var player1Move = Move.Rock;
                var player2Move = Move.Scissors;

                // Act
                await _rpsGameService.JoinRpsGameAsync(gameId, player2Name);

                await _rpsGameService.MakeMoveAsync(gameId, player1Name, player1Move);
                var rpsGameUpdated = await _rpsGameService.MakeMoveAsync(gameId, player2Name, player2Move);

                // Assert
                Assert.Equal(player1Move, rpsGameUpdated.Player1Move);
                Assert.Equal(player2Move, rpsGameUpdated.Player2Move);
                Assert.Equal(RpsGameState.Completed, rpsGameUpdated.State);
                Assert.Equal(player1Name, rpsGameUpdated.Winner);
            }
        }
    
        public class GetRpsGameAsync : RpsGameServiceTests
        {
            [Fact]
            public async Task GetRpsGameAsync_ShouldGetGame_WhenJustCreated()
            {
                // Assemble
                var player1Name = "Player1";               
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                var gameId = rpsGame.Id;                        

                // Act
                var rpsGameGet = await _rpsGameService.GetRpsGameAsync(gameId);

                // Assert
                Assert.Equal(rpsGame, rpsGameGet);
            }

            [Fact]
            public async Task GetRpsGameAsync_ShouldGetGame_WhenPlayer2Joined()
            {
                // Assemble
                var player1Name = "Player1";
                var player2Name = "Player2";
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                var gameId = rpsGame.Id;        

                // Act
                var rpsGameUpdated = await _rpsGameService.JoinRpsGameAsync(gameId, player2Name);
                var rpsGameGet = await _rpsGameService.GetRpsGameAsync(rpsGameUpdated.Id);


                // Assert
                Assert.Equal(rpsGameUpdated, rpsGameGet);
            }

            [Fact]
            public async Task GetRpsGameAsync_ShouldGetGame_WhenMovesHaveBeenMade()
            {
                // Assemble
                var player1Name = "Player1";
                var player2Name = "Player2";
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                var gameId = rpsGame.Id;
                var rock = Move.Rock; 

                // Act
                await _rpsGameService.JoinRpsGameAsync(gameId, player2Name);

                await _rpsGameService.MakeMoveAsync(gameId, player1Name, rock);
                var rpsGameUpdated = await _rpsGameService.MakeMoveAsync(gameId, player2Name, rock);
                var rpsGameGet = await _rpsGameService.GetRpsGameAsync(gameId);

                // Assert
                Assert.Equal(rpsGameUpdated, rpsGameGet);
            }
        }
    
        public class ConvertRpsGameToResponseTest : RpsGameServiceTests
        {
            [Fact]
            public async Task ConvertRpsGameToResponse_ShouldConvertGameToResponseCorrectly()
            {
                // Assemble
                var player1Name = "Player1";
                var player2Name = "Player2";
                var rpsGame = await _rpsGameService.CreateRpsGameAsync(player1Name);
                var gameId = rpsGame.Id;
                var rock = Move.Rock;
                var tie = "Tie";

                var expectedResponse = new RpsGameResponse
                {
                    Id = gameId,
                    Player1 = player1Name,
                    Player2 = player2Name,
                    Player1Move = rock.ToString(),
                    Player2Move = rock.ToString(),
                    State = RpsGameState.Completed.ToString(),
                    Winner = tie
                };

                // Act
                await _rpsGameService.JoinRpsGameAsync(gameId, player2Name);
                await _rpsGameService.MakeMoveAsync(gameId, player1Name, rock);
                var rpsGameUpdated = await _rpsGameService.MakeMoveAsync(gameId, player2Name, rock);

                var rpsGameResponse = _rpsGameService.ConvertRpsGameToResponse(rpsGameUpdated);
                
                // Assert
                Assert.Equivalent(expectedResponse, rpsGameResponse);
            }           
        }
    }
}
