using System.Collections.Generic;

namespace Shared.ResponseModels
{
    public class PaginationModel<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long TotalItemCount { get; set; }
        public int TotalPages { get; set; }
    }
}
