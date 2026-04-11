using ServiceFlow.Class.Models;

namespace ServiceFlow.Web.ViewModels
{
    public class RequestListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string RequesterName { get; set; } = null!;
        public string? AssigneeName { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public DateTime Creation { get; set; }
    }
}
