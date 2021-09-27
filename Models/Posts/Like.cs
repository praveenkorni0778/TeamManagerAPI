﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManagerAPI.Models.Posts
{
    public class Like
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserName { get; set; }
    }
}
