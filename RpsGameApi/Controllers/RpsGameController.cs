using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RpsGameApi.Exceptions;
using RpsGameApi.Models;
using RpsGameApi.Services;

namespace RpsGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RpsGameController : ControllerBase
    {
        private readonly RpsGameService _rpsGameService;

        public RpsGameController(RpsGameService rpsGameService)
        {
            _rpsGameService = rpsGameService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGameAsync([FromBody] CreateRpsGameRequest createRpsGameRequest)
        {            
            var rpsGame = await _rpsGameService.CreateRpsGameAsync(createRpsGameRequest.Name);
            
            var response = _rpsGameService.ConvertRpsGameToResponse(rpsGame);
            return Ok(response);
        }

        [HttpPatch("{id}/join")]
        public async Task<IActionResult> JoinRpsGameAsync(int id, [FromBody] JoinRpsGameRequest joinRpsGameRequest)
        {
            try
            {
                var rpsGame = await _rpsGameService.JoinRpsGameAsync(id, joinRpsGameRequest.Name);  

                var response = _rpsGameService.ConvertRpsGameToResponse(rpsGame);
                return Ok(response);
            }
            catch (RpsGameAlreadyJoinedException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (RpsGameNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex){
                return BadRequest(new { message = ex.Message });
            }            
        }

        [HttpPatch("{id}/move")]
        public async Task<IActionResult> MakeMoveAsync(int id, [FromBody] MoveRequest moveRequest)
        {
            try
            {
                Move move = Enum.Parse<Move>(moveRequest.Move);
                var rpsGame = await _rpsGameService.MakeMoveAsync(id, moveRequest.Name, move);

                var response = _rpsGameService.ConvertRpsGameToResponse(rpsGame);
                return Ok(response);
            }
            catch (RpsGameNotFoundException ex)
            {
                return NotFound(new {message = ex.Message});
            }
            catch (RpsGameNotReadyForMoves ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (Exception ex){
                return BadRequest(new { message = ex.Message });
            } 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameAsync(int id)
        {
            try
            {
                var rpsGame = await _rpsGameService.GetRpsGameAsync(id);
                
                RpsGameResponse response = _rpsGameService.ConvertRpsGameToResponse(rpsGame);
                return Ok(response);
            }
            catch (RpsGameNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        public class JoinRpsGameRequest
        {
            [Required]
            [MinLength(1)]
            public string Name { get; set; }
        }

        public class CreateRpsGameRequest
        {
            [Required]
            [MinLength(1)]
            public string Name { get; set; }
        }

        public class MoveRequest
        {
            [Required]
            [MinLength(1)]
            public string Name { get; set; }

            [Required]
            [EnumDataType(typeof(Move))]
            public string Move { get; set; }
        }
    }
}