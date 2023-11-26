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


    }
}