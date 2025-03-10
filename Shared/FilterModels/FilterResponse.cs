﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.FilterModels
{
    public class FilterResponse <T>
    {
        public IQueryable<T> Query { get; set; }
        public int PageNumber { get; set; }
        public long Count { get; set; }
    }
}
