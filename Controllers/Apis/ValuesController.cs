using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TeamManagerAPI.Controllers.Apis
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _user;
        public ValuesController (UserManager<IdentityUser> user)
        {
            _user = user;
        }

        [HttpPost]
        [Route("Get")]
        public async Task<string> GetUsers()
        {
            var User1 = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return "Hello "+User1 ;
        }
    }
}
