﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Violetum.Domain.Entities;

namespace Violetum.IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("Account/{id}")]
        public async Task<IActionResult> GetAccount(string id)
        {
            UserViewModel vm = await GetUserWithClaims(id);

            return Ok(vm);
        }

        public async Task<IActionResult> UserInfo(string id)
        {
            UserViewModel vm = await GetUserWithClaims(id);

            return View(vm);
        }

        private async Task<UserViewModel> GetUserWithClaims(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);

            return new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Name = GetClaimByType(userClaims, "name"),
                GivenName = GetClaimByType(userClaims, "given_name"),
                FamilyName = GetClaimByType(userClaims, "family_name"),
                Picture = GetClaimByType(userClaims, "picture"),
                Gender = GetClaimByType(userClaims, "gender"),
                Birthdate = GetClaimByType(userClaims, "birthdate"),
                Website = GetClaimByType(userClaims, "website"),
            };
        }

        private static string GetClaimByType(IEnumerable<Claim> claims, string type)
        {
            return claims.Where(x => x.Type == type).Select(x => x.Value).FirstOrDefault();
        }
    }
}