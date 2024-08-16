using Microsoft.AspNetCore.Http;

namespace Web.Entites.Models
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }
		//public string ImageName {  get; set; }
		public DateTime CreatedDate { get; set; }= DateTime.Now;
		[NotMapped]
		public IFormFile ?ImageCover { get; set; }
		[ValidateNever]
		public string ImageName {  get; set; }
	}
}
