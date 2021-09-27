using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManagerAPI.Models
{
    public class Post
    {
        
        public int Id { get; set; }
        public string PostedBy { get; set; }
        public string Content { get; set; }
        public DateTime PostDate { get; set; }
        public string TeamId { get; set; }
        public int Likes { get; set; }
    }
}
