﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IzinFormu.Models
{
    public class Departments
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string DDepartmentName { get; set; }
    }
}
