using ServiceFlow.Class.Models;

namespace ServiceFlow.Web.ViewModels
{
    public class RequestDetailViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public Priority Priority { get; set; }
        public int CategoryId { get; set; }
        public Status Status { get; set; }
        public DateTime Creation { get; set; }
        public string? AssigneeName { get; set; }
        public string RequesterName { get; set; } = null!;
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();

    }
}
