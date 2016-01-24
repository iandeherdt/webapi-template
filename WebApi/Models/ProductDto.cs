using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiTemplate.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(512)]
        public string Name { get; set; }
    }
}