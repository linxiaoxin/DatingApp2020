using System;
using System.Collections.Generic;
using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; } 
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastActive { get; set; }
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interest { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }

        public ICollection<UserLikes> LikedUsers { get; set; }

        public ICollection<UserLikes> LikedByUsers { get; set; }
        public int Age {
            get { return this.DateOfBirth.CalculateAge();}
        }
    }
}