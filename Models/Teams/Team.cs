using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManagerAPI.Models
{
    public class Team
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string Description {get;set;}
    }
}
