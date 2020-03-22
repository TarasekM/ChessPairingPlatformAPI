using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformaTurniejowaAPI.Models;
using PlatformaTurniejowaAPI.PairingAlgorithms.Tournaments;
using System.Text.Json;

namespace PlatformaTurniejowaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentModelsController : ControllerBase
    {
        private readonly TournamentContext _context;

        public TournamentModelsController(TournamentContext context)
        {
            _context = context;
        }

        // GET: api/TournamentModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentModel>>> GetTournaments()
        {
            return await _context.Tournaments
                .Include(tournamentModel => tournamentModel.Players)
                .ToListAsync();
        }

        // GET: api/TournamentModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentModel>> GetTournamentModel(Guid id)
        {
            var tournamentModel = await _context.Tournaments.FindAsync(id);

            if (tournamentModel == null)
            {
                return NotFound();
            }

            return tournamentModel;
        }

        // PUT: api/TournamentModels/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentModel(Guid id, [FromBody]TournamentModel tournamentModel)
        {
            if (id != tournamentModel.ID)
            {
                return BadRequest();
            }

            _context.Entry(tournamentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentModelExists(id))
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

        //POST: api/TournamentModels
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TournamentModel>> PostTournamentModel(TournamentModel tournamentModel)
        {
            tournamentModel.CurrentRound = 1;
            _context.Tournaments.Add(tournamentModel);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetTournamentModel", new { id = tournamentModel.ID }, tournamentModel);
        }

        // DELETE: api/TournamentModels/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TournamentModel>> DeleteTournamentModel(Guid id)
        {
            var tournamentModel = await _context.Tournaments
                .FindAsync(id);
            if (tournamentModel == null)
            {
                return NotFound();
            }

            _context.Tournaments.Remove(tournamentModel);
            await _context.SaveChangesAsync();

            return tournamentModel;
        }

        [HttpGet("{tournamentID}/pair")]
        public async Task<ActionResult<List<List<PlayerModel>>>> GetPairing(Guid tournamentID)
        {
            TournamentModel model = await _context.Tournaments
                .FindAsync(tournamentID);


            var playersModels = await _context.Players
                .Where(p => p.TournamentGuid == tournamentID)
                .Include(p => p.Opponents)
                .ToListAsync();

            if (model == null || playersModels == null)
            {
                return BadRequest();
            }

            if (playersModels.Count % 2 == 1)
            {
                playersModels = await AddByePlayer(playersModels);
            }
            model.Players = playersModels;
            Tournament tournament = TournamentFactory.GetTournamentFromModel(model, playersModels);
            List<List<int>> pairings = tournament.Pair();
            List<List<PlayerModel>> playersModelsPairing = GetPlayerModelsByIdInTournament(model, pairings);
            return playersModelsPairing;
        }

        private async Task<List<PlayerModel>> AddByePlayer(List<PlayerModel> playersModels)
        {
            PlayerModel byeModel = GetByePlayer();
            var bye = await _context.Players
                .Where(p => p.Name == byeModel.Name)
                .FirstOrDefaultAsync();
            if (bye == null)
            {
                _context.Players.Add(byeModel);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception) { }

                bye = await _context.Players
                .Where(p => p.Name == byeModel.Name)
                .FirstOrDefaultAsync();
                Opponent opponent = new Opponent()
                {
                    OpponentID = bye.GUID
                };
                _context.Opponents.Add(opponent);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception) { }
                bye = await _context.Players
                .Where(p => p.Name == byeModel.Name)
                .FirstOrDefaultAsync();
                bye.Opponents = new List<Opponent>() { opponent };
                _context.Entry(bye).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            bye = await _context.Players
                .Where(p => p.Name == byeModel.Name)
                .Include(p => p.Opponents)
                .FirstOrDefaultAsync();
            playersModels.Add(bye);
            return playersModels;
        }

        [HttpPost("{tournamentID}/setScores")]
        public async Task<IActionResult> SetScores(Guid tournamentID, List<Dictionary<string, Dictionary<string, string>>> JSONscores)
        {

            TournamentModel model = await _context.Tournaments
                .FindAsync(tournamentID);

            var playersModels = await _context.Players
                .Where(p => p.TournamentGuid == tournamentID)
                .Include(p => p.Opponents)
                .ToListAsync();

            if (model == null || playersModels == null)
            {
                return BadRequest();
            }

            foreach(Dictionary<string, Dictionary<string, string>> pair in JSONscores)
            {
                Dictionary<string, string> white = pair["white"];
                Dictionary<string, string>  black = pair["black"];

                Guid whiteID = Guid.Parse(white["id"]);
                Guid blackID = Guid.Parse(black["id"]);
                float whiteScore = float.Parse(white["score"]);
                float blackScore = float.Parse(black["score"]);
                PlayerModel cWhiteModel = playersModels.Find(p => p.GUID == whiteID);
                PlayerModel cBlackModel = playersModels.Find(p => p.GUID == blackID);
                if (cWhiteModel != null)
                {
                    // White
                    cWhiteModel.GamesAsWhite++;
                    cWhiteModel.TotalGames++;
                    cWhiteModel.Score += whiteScore;

                    Opponent whiteOpponent = new Opponent()
                    {
                        OpponentID = blackID
                    };
                    cWhiteModel.Opponents.Add(whiteOpponent);

                    _context.Entry(cWhiteModel).State = EntityState.Modified;
                    _context.Opponents.Add(whiteOpponent);
                    await _context.SaveChangesAsync();

                }
                if (cBlackModel != null)
                { 
                    // Black
                    cBlackModel.GamesAsBlack++;
                    cBlackModel.TotalGames++;
                    cBlackModel.Score += blackScore;
                    await _context.SaveChangesAsync();

                    Opponent blackOpponent = new Opponent()
                    {
                        OpponentID = whiteID
                    };
                    cBlackModel.Opponents.Add(blackOpponent);

                    _context.Entry(cBlackModel).State = EntityState.Modified;
                    _context.Opponents.Add(blackOpponent);
                    await _context.SaveChangesAsync();
                }
            }
            model.CurrentRound++;
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{tournamentID}/highscore")]
        public async Task<ActionResult<List<PlayerModel>>> GetTournamentHighscore(Guid tournamentID)
        {
            TournamentModel model = await _context.Tournaments
                .FindAsync(tournamentID);

            var playersModels = await _context.Players
                .Where(p => p.TournamentGuid == tournamentID)
                .OrderByDescending(p => p.Score)
                .ToListAsync();
            return playersModels;
        }


        private List<List<PlayerModel>> GetPlayerModelsByIdInTournament(TournamentModel tournamentModel, List<List<int>> pairings)
        {
            List<List<PlayerModel>> playerModels = new List<List<PlayerModel>>();
            foreach (List<int> pair in pairings)
            {
                PlayerModel playerModelWhite = tournamentModel.Players.Find(p => p.ID == pair[0]);
                PlayerModel playerModelBlack = tournamentModel.Players.Find(p => p.ID == pair[1]);
                if (playerModelWhite == null)
                {
                    playerModelWhite = GetByePlayer();
                }
                if (playerModelBlack == null)
                {
                    playerModelBlack = GetByePlayer();
                }
                List<PlayerModel> playerModelsPair = new List<PlayerModel>()
                {
                    playerModelWhite,
                    playerModelBlack
                };
                playerModels.Add(playerModelsPair);
            }
            return playerModels;
        }

        private PlayerModel GetByePlayer()
        {
            PlayerModel byePlayer = new PlayerModel
            {
                Name = "-----",
                Score = 0,
                ID = 0
            };
            return byePlayer;
        }

        private bool TournamentModelExists(Guid id)
        {
            return _context.Tournaments.Any(e => e.ID == id);
        }
    }
}
