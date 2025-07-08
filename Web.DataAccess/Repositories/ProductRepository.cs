

//namespace Web.DataAccess.Repositories
//{
//    public class ProductRepository : GenericRepository<Product>, IProductRepository
//    {
//        private readonly ApplicationDbContext _context;
//        public ProductRepository(ApplicationDbContext context) : base(context)
//        {
//            _context = context;
//        }

//        public void AddProductVM(ProductVMCreate Model)
//        {
//            Product product = Model.Product;
//            product.ImageName=SaveImage(Model.ImageFile);
//            Add(product);
//        }

//        public void DeleteWithImage(Product product)
//        {

//            var imagePath = Path.Combine("wwwroot", SD.ImagePathProducts, product.ImageName);
//            if(System.IO.File.Exists(imagePath))
//                System.IO.File.Delete(imagePath);
//            Remove(product);
//        }

//        public void Update(ProductVMEdit model)
//        {
//            var productDB=GetBy(x=>x.Id==model.Product.Id);
//            var imageFile=model.ImageFile;
//            var HasNewImage=imageFile is not null;
//            var imageOldName = productDB.ImageName;
//            if (HasNewImage)
//            {
//                productDB.ImageName=SaveImage(imageFile);
//                var imagePath=Path.Combine("wwwroot",SD.ImagePathProducts,imageOldName);
//                if(System.IO.File.Exists(imagePath))
//                {
//                    System.IO.File.Delete(imagePath);
//                }
//            }
//            productDB.Name = model.Product.Name;
//            productDB.Description = model.Product.Description;
//            productDB.Price = model.Product.Price;
//        }

//        private string SaveImage(IFormFile cover)
//        {
//                var coverName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";
//                var imagesPath = Path.Combine("wwwroot", SD.ImagePathProducts);
//                var path = Path.Combine(imagesPath, coverName);

//                if (!Directory.Exists(imagesPath))
//                {
//                    Directory.CreateDirectory(imagesPath);
//                }

//                using var stream = new FileStream(path, FileMode.Create);
//                cover.CopyTo(stream);

//                return coverName;
            
//        }
//    }
//}
