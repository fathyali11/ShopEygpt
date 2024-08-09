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

        public void Update(Category model)
        {
            var category=GetBy(x=>x.Id==model.Id);
            if (category != null)
            {
                category.Name = model.Name;
            }
        }
    }
}
