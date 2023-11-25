using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


            //  var result = await _db.People.OrderBy(x => x.LastName).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = await _db.Images
                   .OrderByDescending(i => i.PostingDate)
                   .Skip((pageNumber - 1) * pageSize)
                   .Take(pageSize)
                   .ToListAsync();

            if (result.Count == 0)
            {
                return NotFound(new
                {
                    Message = "No images found."
                });
            }
            var response = new PageResponse<Image>(result);
            //   var totalRecords = _db.People.CountAsync();
            response.Meta.Add("TotalPages", 20);
            response.Meta.Add("TotalRecords", 200);

            var links = LinksGenerator.GenerateLinks("/api/People", pageNumber, 1000, pageSize);
            return Ok(response);

        }


    }
}