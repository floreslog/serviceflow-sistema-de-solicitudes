namespace ServiceFlow.Web.ViewModels
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}