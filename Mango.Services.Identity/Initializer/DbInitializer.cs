﻿using IdentityModel;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mango.Services.Identity.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task Initialize()
        {
            if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            var adminMail = Faker.Internet.Email();

            var adminUser = new ApplicationUser()
            {
                UserName = adminMail,
                Email = adminMail,
                EmailConfirmed = true,
                PhoneNumber = Faker.Phone.Number(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last()
            };

            _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

            var temp1 = _userManager.AddClaimsAsync(adminUser, new Claim[]
            {
                new(JwtClaimTypes.Name, adminUser.FirstName + " " + adminUser.LastName),
                new(JwtClaimTypes.GivenName, adminUser.FirstName),
                new(JwtClaimTypes.FamilyName, adminUser.LastName),
                new(JwtClaimTypes.Role, SD.Admin)
            }).Result;

            var userEmail = Faker.Internet.Email();

            var customerUser = new ApplicationUser()
            {
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true,
                PhoneNumber = Faker.Phone.Number(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last()
            };

            _userManager.CreateAsync(customerUser, "Admin123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customerUser, SD.Customer).GetAwaiter().GetResult();

            var temp2 = _userManager.AddClaimsAsync(customerUser, new Claim[]
            {
                new(JwtClaimTypes.Name, customerUser.FirstName + " " + customerUser.LastName),
                new(JwtClaimTypes.GivenName, customerUser.FirstName),
                new(JwtClaimTypes.FamilyName, customerUser.LastName),
                new(JwtClaimTypes.Role, SD.Customer)
            }).Result;

            await Task.CompletedTask;
        }
    }
}