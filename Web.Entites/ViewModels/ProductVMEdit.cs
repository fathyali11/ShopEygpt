﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Entites.Models;

namespace Web.Entites.ViewModels
{
    public class ProductVMEdit:ProductVM
    {
        [Display(Name = "Image")]
        public IFormFile ?ImageFile { get; set; }
    }
}