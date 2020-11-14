using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public async static Task SeedAppUser(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

            //read file from json using
            var usersfile = await System.IO.File.ReadAllTextAsync("Data/UserSeed.json");
            var users = JsonSerializer.Deserialize<IEnumerable<AppUser>>(usersfile);

            foreach (var user in users)
            {
                var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                user.PasswordSalt = hmac.Key;
            }
            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}