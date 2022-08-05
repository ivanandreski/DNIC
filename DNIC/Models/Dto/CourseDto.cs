using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace DNIC.Models.Dto
{
    public class CourseDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        //

        public Guid CourseId { get; set; }
        public byte[] ImageArray { get; set; }
    }
}
