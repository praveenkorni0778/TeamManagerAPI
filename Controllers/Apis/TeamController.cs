using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TeamManagerAPI.Data;
using TeamManagerAPI.Models;

namespace TeamManagerAPI.Controllers.Apis
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        public readonly ApplicationDbContext _db;
        public TeamController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Get all teams - for admin purpose
        public IActionResult Get()
        {
            return null;
        }

        // Get all teams for a user
        [HttpGet]
        [Route("Getfor/{userName}")]        
        public IActionResult Get(string userName)
        {
            try
            {
                var TeamIdList = _db.Members.Where(m => m.Name == userName).Select(t => t.TeamId).ToList();
                if (TeamIdList == null) return Ok(null);
                var TeamList = new List<Team>();
                foreach (var teamId in TeamIdList)
                {
                    TeamList.Add(_db.Teams.First(t => t.Id == teamId));
                }
                return Ok(TeamList);
            }
            catch(Exception e)
            {
                return StatusCode(500, new
                {
                    Response = "Internal server error, please contact administrator."
                });
                Console.WriteLine(e);
            }
        }

        // Get team by Id
        [HttpGet]
        [Route("Get")]
        [Route("Getteam/{TeamId}")]
        public IActionResult GetTeam(string TeamId)
        {
            try
            {
                var team = _db.Teams.Find(TeamId);
                if(team == null)
                {
                    return BadRequest(new
                    {
                        Response = "No teams found for the given TeamId."
                    });
                }
                return Ok(team);
            }
            catch(Exception e)
            {
                return StatusCode(500, new
                {
                    Response = "Internal server error. Please contact administrator."
                });
            }
        }

        // Create Team
        [HttpPost]
        [Route("Create")]
        public IActionResult Create(Team team)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Response = "Invalid data passed.",
                    StatusCode = 400
                });
            }
            var sameName = _db.Teams.Find(team.Id);
            if(sameName != null)
            {
                return BadRequest(new
                {
                    Response = "The team id is not unique, try again.",
                    StatusCode = 400
                });
            }
            try
            {
                _db.Teams.Add(team);
                _db.Members.Add(new Member
                {
                    TeamId = team.Id,
                    Name = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Role = "Manager",
                    RosterPosition = 0,
                    GameRole = "Manager"
                });
                _db.SaveChanges();
                return Ok(team);
            }
            catch(Exception e)
            {
                return StatusCode(500, new
                {
                    Response = "Internal server error, please contact administrator."
                });
                Console.WriteLine(e);
            }
        }

        [HttpPut]
        [Route("Put")]
        public IActionResult Update(Team team)
        {            
            if (!IsAuthorized(team.Id))
            {
                return Unauthorized(new
                {
                    Response = "You are unauthorized to make the changes."
                });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Response = "Please verify the data passed.",
                    StatusCode = 400
                });
            }
            var teamExists = _db.Teams.Where(t => t.Id == team.Id);
            if(teamExists == null)
            {
                return BadRequest(new
                {
                    Response = "No teams found to update.",
                    StatusCode = 400
                });
            }
            try
            {
                _db.Teams.Update(team);
                _db.SaveChanges();
                return Ok(team);
            }
            catch(Exception e)
            {
                return StatusCode(500, new
                {
                    Response = "Internal server error, please contact administrator."
                });
            }
        }

        [HttpDelete]
        [Route("Delete")]
        [Route("Delete/{Id}")]
        public IActionResult Delete(string Id)
        {            
            if (!IsAuthorized(Id))
            {
                return Unauthorized(new
                {
                    Response = "You are unauthorized."
                });
            }
            var teamExists = _db.Teams.Find(Id);
            if (teamExists == null)
            {
                return BadRequest(new
                {
                    Response = "No teams found to Delete.",
                    StatusCode = 400
                });
            }
            try
            {
                _db.Remove(teamExists);
                _db.SaveChanges();
                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(500, new
                {
                    Response = "Internal server error, please contact administrator."
                });
            }
        }

        private bool IsAuthorized(string Id)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = _db.Members.Where(m => m.TeamId == Id && m.Name == user);            
            var role = userRole.FirstOrDefault().Role; 
            if (role == "Manager") return true;
            return false;
        }
    }
}
