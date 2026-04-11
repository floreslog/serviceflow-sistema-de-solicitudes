namespace ServiceFlow.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalRequests { get; set; }            // (User, Admin)
        public int OpenRequests { get; set; }             // (User, Admin)
        public int ResolvedRequests { get; set; }         // (User, Admin)

        public int TotalAssignedRequests { get; set; }    // (Agent)
        public int InProgressRequests { get; set; }       // (Agent)
        public int PendingRequests { get; set; }          // (Agent)

        public int UrgentRequests { get; set; }           // (Admin)
    }
}
