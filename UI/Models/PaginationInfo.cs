namespace UI.Models
{
    public class PaginationInfo
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
        public PaginationInfo(int page, int pageSize)
        {
            PageSize = pageSize;
            Page = page;
        }
    }
}
