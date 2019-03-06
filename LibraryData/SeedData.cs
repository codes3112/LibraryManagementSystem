using LibraryData.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibraryData
{
    public class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context,
                                  UserManager<ApplicationUser> userManager,
                                  RoleManager<ApplicationRole> roleManager)
        {
            context.Database.EnsureCreated();

            String adminId1 = "";
            String adminId2 = "";

            string role1 = "Admin";
            string desc1 = "This is the administrator role";

            string role2 = "Member";
            string desc2 = "This is the members role";

            string password = "Na@123456";

            if (await roleManager.FindByNameAsync(role1) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(role1, desc1, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(role2) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(role2, desc2, DateTime.Now));
            }

            if (await userManager.FindByNameAsync("admin@email.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@email.com",
                    Email = "admin@email.com",
                    FirstName = "Adam",
                    LastName = "Aldridge",

                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, role1);
                }
                adminId1 = user.Id;
            }

            if (await userManager.FindByNameAsync("user1@email.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "user1@email.com",
                    Email = "user1@email.com",
                    FirstName = "Bob",
                    LastName = "Barker",

                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, role2);
                }
                adminId2 = user.Id;
            }

            if (await userManager.FindByNameAsync("user2@email.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "user2@email.com",
                    Email = "user2@email.com",
                    FirstName = "Mike",
                    LastName = "Myers",

                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, role2);
                }
            }

            if (await userManager.FindByNameAsync("user3@email.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "user3@email.com",
                    Email = "user3@email.com",
                    FirstName = "Donald",
                    LastName = "Duck",

                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, role2);
                }
            }
        }
    }
}