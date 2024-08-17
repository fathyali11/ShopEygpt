﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Entites.Models;

namespace Web.Entites.IRepositories
{
    public interface IApplicaionUserRepository : IGenericRepository<ApplicationUser>
    {
        void Update(ApplicationUser user);
    }
}
