namespace ServiceFlow.Class.Models
{
    public class RequestModel : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public Priority Priority { get; set; }
        public Status Status { get; set; }
        public DateTime Creation { get; set; }
        public DateTime? LastUpdated { get; set; } = null;
        public DateTime? Closed { get; set; } = null;
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; } = null!;

        //Aqui ya usando identity 
        public string RequesterId { get; set; } = null!;
        public ApplicationUser Requester { get; set; } = null!;

        public string? AssigneeId { get; set; }
        public ApplicationUser? Assignee { get; set; }
    }
}
