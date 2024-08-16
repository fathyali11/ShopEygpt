using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.DataAccess.Data;
using Web.Entites.IRepositories;

namespace Web.DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<SelectListItem> CategorySelectList()
        {
            return GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
        }
        public void AddWithImage(Category category)
        {
            category.ImageName=SaveImage(category.ImageCover);
            Add(category);
        }
        public void DeleteWithImage(Category category)
        {
            var imagePath=Path.Combine("wwwroot",SD.ImagePathCategories,category.ImageName);
            File.Delete(imagePath);
            Remove(category);
        }

        public void Update(Category model)
        {
            var category=GetBy(x=>x.Id==model.Id);
            if (category != null)
            {
                category.Name = model.Name;
                var hasNewImage=model.ImageCover!=null;
                var oldImageName = model.ImageName;
                if (hasNewImage)
                {
                    category.ImageName=SaveImage(model.ImageCover);
                    if(oldImageName!=null)
                    {
                        var imagePath = Path.Combine("wwwroot", SD.ImagePathCategories, oldImageName);
                        File.Delete(imagePath);
                    }
                    
                }
            }
        }
        private string SaveImage(IFormFile cover)
        {
            string imageName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";
            string imagePath=Path.Combine("wwwroot",SD.ImagePathCategories);
            string path=Path.Combine(imagePath,imageName);
            if(!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            using var stream=new FileStream(path, FileMode.Create);
            cover.CopyTo(stream);
            return imageName;
        }
    }
}
