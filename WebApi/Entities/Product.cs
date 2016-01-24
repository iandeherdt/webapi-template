using System.ComponentModel.DataAnnotations;

namespace WebApiTemplate.Entities
{
    public class Product : Entity
    {
        [Required]
        [MaxLength(512)]
        public string Name { get; set; }    
    }
}