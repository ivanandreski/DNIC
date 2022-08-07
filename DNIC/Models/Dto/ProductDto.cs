using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DNIC.Models.Dto
{
    public class ProductDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int Price { get; set; }
        
        [Required]
        public string Url { get; set; }

        [Required]
        public int RealId { get; set; }
    }
}
