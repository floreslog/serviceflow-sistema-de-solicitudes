namespace ServiceFlow.Class.Models
{
    public class CategoryModel : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
