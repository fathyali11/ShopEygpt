namespace ShopEgypt.Web.Models
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime CreatedDate { get; set; }= DateTime.Now;
	}
}
