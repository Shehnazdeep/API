using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private Database _db = new Database();


        [HttpPost]

        public async Task<IActionResult> CreateUser(User user)

        {

            //todo: add validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the email is unique by compairing the emails of user in the database with the enterd email
            if (_db.Users.Any(u => u.Email == user.Email))
            {
                return BadRequest(new
                {

                    Message = "Email is not unique"
                });
            }



            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return Created("", user);

        }
    }
}