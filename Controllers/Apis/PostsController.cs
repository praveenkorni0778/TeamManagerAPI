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
using TeamManagerAPI.Models.Posts;

namespace TeamManagerAPI.Controllers.Apis
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Get")]
        public IActionResult Get()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var teams = _context.Members.Where(t => t.Name == user).Select(t => t.TeamId).ToList();
            List<Post> posts = new List<Post>();
            foreach(var team in teams)
            {
                var teamposts =  _context.Posts.Where(p => p.TeamId == team).ToList();
                posts.AddRange(teamposts);
            }
            return Ok(posts);
        }

        [HttpGet]
        [Route("Get/{id}")]
        public IActionResult Get(int Id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == Id);
            return Ok(post);
        }

        [HttpPost]
        [Route("Post")]
        public IActionResult Post(Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    response = "Invalid values",
                    StatusCode = 400,
                    status = "Fail" 
                });
            }
            try
            {
                _context.Posts.Add(post);
                _context.SaveChanges();
                return Ok(post);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return StatusCode(500, new{
                    Response = "Internal Server Error, Please contact Administrator."
                });
                throw;
            }          
            
        }

        [HttpPut]
        [Route("Put")]
        public IActionResult Put(Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    response = "Invalid values",
                    StatusCode = 400,
                    status = "Fail"
                });
            }
            try
            {
                _context.Posts.Update(post);
                _context.SaveChanges();
                return Ok(post);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return StatusCode(500, new
                {
                    Response = "Internal Server Error, Please contact Administrator."
                });
                throw;
            }

        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            if(post == null)
            {
                return BadRequest(new
                {
                    Response = "Post not found.",
                    StatusCode = 400
                });
            }
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(post.PostedBy != user)
            {
                return Unauthorized(new
                {
                    Response = "You are unauthorized to delete the post.",
                    StatusCode = 401
                });
            }
            try
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, new { 
                    Response = "Internal server error, please contact administrator."
                });
                throw;
            }

        }
        
        // Comment On Post
        [HttpPost]
        [Route("Comment")]
        public IActionResult Comment(Comment comment)
        {
            try
            {
                _context.Comments.Add(comment);
                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(500, new
                {
                    Response = "Internal server error, please contact administrator."
                });
                throw;
            }
        }
        // Delete Comment On Post
        // Like Post
        // Unlike Post
    }
}
