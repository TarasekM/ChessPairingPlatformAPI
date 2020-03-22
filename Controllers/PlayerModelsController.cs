using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformaTurniejowaAPI.Models;

namespace PlatformaTurniejowaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerModelsController : ControllerBase
    {
        private readonly TournamentContext _context;

        public PlayerModelsController(TournamentContext context)
        {
            _context = context;
        }

        // GET: api/PlayerModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerModel>>> GetPlayers()
        {
            return await _context.Players
                .Include(playerModel => playerModel.Opponents)
                .ToListAsync();
        }

        [HttpGet("{tournamentID}")]
        public async Task<ActionResult<IEnumerable<PlayerModel>>> GetPlayerModelsBytournament(Guid tournamentID)
        {
            var tournament = await _context.Tournaments.FindAsync(tournamentID);
            if (tournament == null)
            {
                return NotFound();
            }
            return await _context.Players
                .Where(pm => pm.TournamentGuid == tournamentID)
                .ToListAsync();
        }

        // GET: api/PlayerModels/tournamentID/PlayerID
        [HttpGet("{tournamentID}/{playerID}")]
        public async Task<ActionResult<PlayerModel>> GetPlayerModel(Guid tournamentID, Guid playerID)
        {
            var playerModel = await _context.Players
                .FirstOrDefaultAsync(
                    i => i.GUID == playerID &&
                    i.TournamentGuid == tournamentID);

            if (playerModel == null)
            {
                return NotFound();
            }

            return playerModel;
        }

        // PUT: api/PlayerModels/PlayerID
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{playerID}")]
        public async Task<IActionResult> PutPlayerModel(Guid playerID, string Name)
        {
            var playerModel = await _context.Players
                .FirstOrDefaultAsync(
                    i => i.GUID == playerID);

            playerModel.Name = Name;

            _context.Entry(playerModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerModelExists(playerID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        
        // Tworzy model gracza i przypisuje mu klucz obcy turnieju 
        [HttpPost("{id}")]
        public async Task<ActionResult<PlayerModel>> PostPlayerModel(Guid id, string Name)
        {
            var model = await _context.Tournaments.FindAsync(id);
            var players = await _context.Players
                .Where(pm => pm.TournamentGuid == model.ID)
                .ToListAsync();
            if (model == null)
            {
                return BadRequest();
            }
            model.Players = players;
            PlayerModel playerModel = CreatePlayerModel(model, Name);
            
            _context.Players.Add(playerModel);
            await _context.SaveChangesAsync();

            Guid guid = playerModel.GUID;
            Opponent opponent = new Opponent() { OpponentID = playerModel.GUID };
            _context.Opponents.Add(opponent);

            playerModel = await _context.Players
                .Where(p => p.GUID == guid).FirstOrDefaultAsync();
            playerModel.Opponents = new List<Opponent>() { opponent };
            _context.Entry(playerModel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception exception)
            {
                var exc = exception;
            }

            return CreatedAtAction("GetPlayerModel",
                new { tournamentID = model.ID,
                    playerID = playerModel.GUID }, playerModel);
        }

        // DELETE: api/PlayerModels/5
        [HttpDelete("{playerID}")]
        public async Task<ActionResult<PlayerModel>> DeletePlayerModel(Guid playerID)
        {
            var playerModel = await _context.Players
                .Where(p => p.GUID == playerID)
                .Include(playerModel => playerModel.Opponents)
                .FirstOrDefaultAsync();

            if (playerModel == null)
            {
                return NotFound();
            }

            var opponents = playerModel.Opponents;
            if (opponents == null)
            {
                _context.Players.Remove(playerModel);
                await _context.SaveChangesAsync();
                return playerModel;

            }

            foreach (Opponent opponent in opponents)
            {
                _context.Opponents.Remove(opponent);

            }

            _context.Players.Remove(playerModel);
            await _context.SaveChangesAsync();

            return playerModel;
        }

        private bool PlayerModelExists(Guid id)
        {
            return _context.Players.Any(e => e.GUID == id);
        }
        
        private PlayerModel CreatePlayerModel(TournamentModel model, string Name)
        {
            PlayerModel playerModel = new PlayerModel
            {
                ID = model.Players.Count + 1,
                Name = Name,
                TotalGames = 0,
                GamesAsWhite = 0,
                GamesAsBlack = 0,
                Score = 0
            };

            playerModel.Opponents = new List<Opponent>();

            playerModel.TournamentGuid = model.ID;
            return playerModel;
        }
        private PlayerModel ByePlayer()
        {
            PlayerModel byePlayer = new PlayerModel
            {
                Name = "-----",
                Score = 0,
                ID = 0
            };
            return byePlayer;
        }
    }
}
