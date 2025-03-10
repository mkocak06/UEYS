using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class PagedModel<T>
    {
        const int MaxPageSize = 500;
        private int _pageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public int Page { get; set; }
        public int TotalItemCount { get; set; }
        public int TotalPages { get; set; }
        public IList<T> Items { get; set; }

        public PagedModel()
        {
            Items = new List<T>();
        }
    }
}
