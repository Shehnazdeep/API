using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public List<String> ImagesUrls { get; set; }

    }
}