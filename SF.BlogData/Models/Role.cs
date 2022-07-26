﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.BlogData.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Roles Value { get; set; }
        public string Description { get; set; }
        public List<User> Users { get; set; } = new();
    }

    public enum Roles
    {
        None = 0, Admin, User, Moderator
    }
}
