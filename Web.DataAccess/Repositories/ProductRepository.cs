using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void AddProductVM(ProductVM Model)
        {
            Product product = Model.Product;
            product.ImageName=SaveImage(Model.ImageFile);
            Add(product);
        }

        public void DeleteWithImage(Product product)
        {

            var imagePath = Path.Combine("wwwroot", FileSettings.ImagePath, product.ImageName);
            if(System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
            Remove(product);
        }

        public void Update(ProductVM model)
        {
            var productDB=GetBy(x=>x.Id==model.Product.Id);
            var imageFile=model.ImageFile;
            var HasNewImage=imageFile is not null;
            var imageOldName = productDB.ImageName;
            if (HasNewImage)
            {
                productDB.ImageName=SaveImage(imageFile);
                var imagePath=Path.Combine("wwwroot",FileSettings.ImagePath,imageOldName);
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            productDB.Name = model.Product.Name;
            productDB.Description = model.Product.Description;
            productDB.Price = model.Product.Price;
        }

        private string SaveImage(IFormFile cover)
        {
                var coverName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";
                var imagesPath = Path.Combine("wwwroot", FileSettings.ImagePath);
                var path = Path.Combine(imagesPath, coverName);

                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                using var stream = new FileStream(path, FileMode.Create);
                cover.CopyTo(stream);

                return coverName;
            
        }
    }
}
