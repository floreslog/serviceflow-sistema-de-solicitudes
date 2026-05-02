namespace ServiceFlow.Web.ViewModels
{
    public class UserDetailViewModel
    {
        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string PaternalSurname { get; set; } = null!;
        public string MaternalSurname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = null!;
        public int AccessFailedCount { get; set; }
        public int TotalRequests { get; set; }
        public List<RequestListViewModel> Requests { get; set; } = new();
    }
}
