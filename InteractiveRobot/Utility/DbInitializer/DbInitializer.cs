﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InteractiveRobot.Utility.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                if (_roleManager.Roles is not null)
                {
                    _roleManager.CreateAsync(new(SD.SuperAdmin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Parent)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Doctor)).GetAwaiter().GetResult();

                    _userManager.CreateAsync(new()
                    {
                        UserName = "SuperAdmin",
                        Email = "SuperAdmin@gmail.com",
                        Name = "SuperAdmin",
                        EmailConfirmed = true
                    }, "Admin123*").GetAwaiter().GetResult();

                    var user = _userManager.FindByEmailAsync("SuperAdmin@gmail.com").GetAwaiter().GetResult();

                    if (user is not null)
                    {
                        _userManager.AddToRoleAsync(user, SD.SuperAdmin).GetAwaiter().GetResult();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
