namespace ServiceFlow.Web.ViewModels
{
    public class CommentViewModel
    {
        public string Text { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

    }
}
