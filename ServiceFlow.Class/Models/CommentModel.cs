namespace ServiceFlow.Class.Models
{
    public class CommentModel : IEntity
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTime CreatedAt {  get; set; }
        public int RequestId { get; set; }
        public RequestModel Request {  get; set; } = null!;

        //Aqui ya usando identity
        public string AuthorId { get; set; } = null!;
        public ApplicationUser Author {  get; set; } = null!;
    }
}
