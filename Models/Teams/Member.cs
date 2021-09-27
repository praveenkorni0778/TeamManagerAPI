using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManagerAPI.Models
{
    public class Member
    {        
        public int Id { get; set; }
        [Required]
        public string TeamId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Role { get; set; }
        public string GameRole { get; set; }
        public int RosterPosition { get; set; }
    }
}
