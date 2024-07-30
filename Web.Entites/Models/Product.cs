

namespace Web.Entites.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        [Required]
        [Display(Name ="Category")]
        public int CategoryId {  get; set; }
        [ValidateNever]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        [ValidateNever]
        public string ImageName {  get; set; }
    }
}
