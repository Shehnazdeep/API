using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Models.Entities;
using API.Models.Helpers;
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

        // POST /api/users/{id}/image
        [HttpPost("{userId}/image")]
        public async Task<IActionResult> AddImageToUser([FromRoute] Guid userId, [FromBody] Image image)
        {


            // if (string.IsNullOrEmpty(url))
            // {
            //     return BadRequest(new
            //     {
            //         Message = "URL cannot be null or empty."
            //     });
            // }
            var user = await _db.Users.FindAsync(userId);


            if (user == null)
                return BadRequest(new
                {

                    Message = "Provided Id seems invalid!"
                });

            image.User = user;
            image.PostingDate = DateTime.Now;


            var tagNames = ImageHelper.GetTags(image.Url).ToList();
            // Convert tag names to Tag objects
            var tags = tagNames.Select(tagName => new Tag
            {
                Id = Guid.NewGuid(), // Generate a new Guid for the tag
                Text = tagName,
                Images = new List<Image> { image } // Link the image to the tag
            }).ToList();

            _db.Tags.AddRange(tags);

            image.Tags = tags;

            //  user.Images ??= new List<Image>();
            user.Images.Add(image);
            _db.Images.Add(image);
            await _db.SaveChangesAsync();

            var last10Images = user.Images.OrderByDescending(i => i.PostingDate).Take(10).ToList();
            var userDto = new UserDTO()
            {
                //advantage of using Dto classes is to promote consistency. 
                //we can also use anonymus onjects but that is not good for consistnecy
                Id = user.Id,
                Username = user.Name,
                Email = user.Email,
                ImagesUrls = last10Images.Select(i => i.Url).ToList()
            };

            return Ok(userDto);


        }

    }
}