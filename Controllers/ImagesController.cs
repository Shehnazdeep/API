using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Models.Entities;
using API.Models.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Responses;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private Database _db = new Database();


        // GET /api/images
        [HttpGet]
        public async Task<IActionResult> GetAllImages([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {

            var totalRecords = await _db.Images.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var result = await _db.Images
                   .OrderByDescending(i => i.PostingDate)
                   .Skip((pageNumber - 1) * pageSize).Include(x => x.User)
                   .Take(pageSize)
                   .ToListAsync();

            if (result.Count == 0)
            {
                return NotFound(new
                {
                    Message = "No images found."
                });
            }

            var imageDtos = result.Select(image => new ImageDTO
            {
                Id = image.Id,
                Url = image.Url,
                Username = image.User?.Name, // Accessing the Name property inside the User

            }).ToList();


            var response = new PageResponse<ImageDTO>(imageDtos);
            //   var totalRecords = _db.People.CountAsync();
            response.Meta.Add("TotalPages", totalPages);
            response.Meta.Add("TotalRecords", totalRecords);
            var links = LinksGenerator.GenerateLinks("/api/Images", pageNumber, totalRecords, pageSize);

            response.Links = links;
            return Ok(response);

        }


        // GET /api/images/{id}

        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetImageById(Guid imageId)
        {
            if (imageId == Guid.Empty)
            {
                return BadRequest(new
                {
                    Message = "Invalid ID format."
                });
            }

            // Retrieve the image from the database
            var image = await _db.Images.Include(x => x.User)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == imageId);

            //  var user = await _db.Users.Where(image)
            // Check if the image with the provided ID exists
            if (image == null)
            {
                return NotFound(new
                {
                    Message = "Image not found."
                });
            }
            if (image.User == null)
            {
                return BadRequest(new
                {
                    Message = "User associated with the image is null."
                });
            }


            var imageDetails = new
            {
                Id = image.Id,
                Url = image.Url,
                UserName = image.User?.Name,
                UserId = image.User.Id,
                Tags = image.Tags.Select(tag => tag.Text).ToList(),
            };

            return Ok(imageDetails);

        }

        // GET /api/images/byTag?tag=cars&pageNumber=1&pageSize=10
        [HttpGet("byTag")]
        public async Task<IActionResult> GetImagesByTag([FromQuery] string tag, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {



            if (!string.IsNullOrEmpty(tag))
            {


                var result = await _db.Images
     .Include(image => image.Tags)
     .Include(x => x.User)
     .Where(image => image.Tags.Any(t => t.Text == tag))
     .OrderByDescending(image => image.PostingDate)
     .Skip((pageNumber - 1) * pageSize)
     .Take(pageSize)
     .ToListAsync();





                var totalImages = await _db.Images.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalImages / pageSize);

                var imageDtos = result.Select(image => new ImageDTO
                {
                    Id = image.Id,
                    Url = image.Url,
                    Username = image.User?.Name, // Accessing the Name property inside the User

                }).ToList();

                var response = new PageResponse<ImageDTO>(imageDtos);
                //   var totalRecords = _db.People.CountAsync();
                response.Meta.Add("TotalPages", totalPages);
                response.Meta.Add("TotalRecords", totalImages);
                var links = LinksGenerator.GenerateLinks("/api/Images", pageNumber, totalImages, pageSize);

                response.Links = links;
                return Ok(response);

            }
            else
            {

                return NotFound(new
                {
                    Message = "Tag does not exists."
                });
            }





        }
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomImages()
        {
            var totalImages = await _db.Images.CountAsync();
            var count = 2;

            var result = await _db.Images.Include(x => x.User).ToListAsync();

            // Order by a random GUID to introduce randomness
            var orderedResult = result.OrderBy(image => Guid.NewGuid()).Take(count).ToList();

            var imageDtos = orderedResult.Select(image => new ImageDTO
            {
                Id = image.Id,
                Url = image.Url,
                Username = image.User?.Name,
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalImages / count);
            var response = new PageResponse<ImageDTO>(imageDtos);
            response.Meta.Add("TotalPages", totalPages);
            response.Meta.Add("TotalRecords", totalImages);
            var links = LinksGenerator.GenerateLinks("/api/Images", 1, totalImages, 10);

            response.Links = links;
            return Ok(response);
        }




    }
}