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
    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicaionUserRepository
	{
        private readonly ApplicationDbContext _context;
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(ApplicationUser user)
        {
            var oldUser = _context.ApplicationUsers.FirstOrDefault(x => x.Id == user.Id);
            if (oldUser != null)
            {
                oldUser.Name = user.Name;
                oldUser.Email = user.Email;
                oldUser.Phone = user.Phone;
                oldUser.City = user.City;
            }
        }

    }
}
