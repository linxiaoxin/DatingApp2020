using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public async static Task SeedAppUser(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            //read file from json using
            var usersfile = await System.IO.File.ReadAllTextAsync("Data/UserSeed.json");
            var users = JsonSerializer.Deserialize<IEnumerable<AppUser>>(usersfile);

            if(users == null) return;

            var roleList = new AppRole[]{
                new AppRole{ Name= "Admin"},
                new AppRole{ Name= "Moderator"},
                new AppRole{ Name= "Member"}
            };

            foreach (var role in roleList)
            {
                await roleManager.CreateAsync(role);
            }    

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(await userManager.FindByNameAsync(user.UserName.ToLower()), "Member");
            }

            var admin = new AppUser{ UserName = "admin"};
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(await userManager.FindByNameAsync(admin.UserName.ToLower()), new [] {"Admin", "Moderator"});
        }
    }
}