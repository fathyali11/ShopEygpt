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
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
