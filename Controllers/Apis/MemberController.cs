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
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public MemberController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("Get")] // api/member/get
        public IActionResult Get(string TeamId)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isMember = _db.Members.FirstOrDefault(m => m.TeamId == TeamId && m.Name == user);
            if(isMember != null)
            {
                try
                {
                    var Members = _db.Members.Where(m => m.TeamId == TeamId).ToList();
                    return Ok(Members);
                }
                catch(Exception e)
                {
                    return StatusCode(500, new
                    {
                        Response = "Internal server error."
                    });
                }                
            }
            return Unauthorized(new
            {
                Response = "You are unauthorized to view this team's members."
            });
        }
        [HttpPost]
        [Route("Add")] // api/member/add
        public IActionResult Add(Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Response = "Invalid parameters."
                });
            }
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = _db.Members.FirstOrDefault(m => m.TeamId == member.TeamId && m.Name == user);
            var isManager = userRole.Role.Equals("Manager");
            if (isManager)
            {                
                try
                {
                    _db.Members.Add(member);
                    _db.SaveChanges();
                    return Ok(member);
                }
                catch(Exception e)
                {
                    return StatusCode(500);
                }
            }
            return Unauthorized(new
            {
                Response = "Only managers can add members."
            });
        }

        [HttpPut]
        [Route("put")] // api/member/put
        public IActionResult Edit(Member member)
        {            
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Response = "Invalid parameters."
                });
            }
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isManager = _db.Members.FirstOrDefault(m => m.TeamId == member.TeamId && m.Name == user).Role.Equals("Manager");
            // check if the current user is the manager of the team they are trying to edit.
            if (isManager)
            {
                // check if the member exist.
                var memberExist = _db.Members.Find(member.Id);
                if(memberExist != null)
                {                    
                    // check if attempt is made to alter the name
                    if( memberExist.Name == member.Name)
                    {
                        // check is attempt is made to change the member's team
                        if (memberExist.TeamId != member.TeamId)
                        {
                            var memberInTeam = _db.Members.FirstOrDefault(m => m.Name == member.Name && m.TeamId == member.TeamId);
                            if(memberInTeam != null)
                            {
                                return BadRequest(new
                                {
                                    Response = "The member already exists in the specified team."
                                });
                            }
                            // change roster position to not set
                            member.RosterPosition = -1;
                        }
                        try
                        {
                            _db.Members.Update(member);
                            _db.SaveChanges();
                            return Ok(member);
                        }
                        catch (Exception e)
                        {
                            return StatusCode(500);
                        }
                    }
                    return BadRequest(new
                    {
                        Response = "Member's name can not be edited"
                    });
                }
                return BadRequest(new
                {
                    Response = "The member does not exist."
                });
            }
            return Unauthorized(new
            {
                Response = "Only managers can add members."
            });
        }

        [HttpDelete]
        [Route("Delete/{Id}")] // api/member/delete/{id}
        public IActionResult Delete(int Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Response = "Invalid parameters."
                });
            }
            var memberExist = _db.Members.Find(Id);
            if(memberExist == null)
            {
                return BadRequest(new
                {
                    Response = "The member does not exist."
                });
            }
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = _db.Members.FirstOrDefault(m => m.TeamId == memberExist.TeamId && m.Name == user);
            var isManager = userRole.Role.Equals("Manager");
            if (isManager)
            {
                try
                {
                    _db.Members.Remove(memberExist);
                    _db.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    return StatusCode(500);
                }
            }
            return Unauthorized(new
            {
                Response = "Only managers can add members."
            });

        }

    }
}
