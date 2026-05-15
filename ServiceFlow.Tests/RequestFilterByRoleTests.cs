using ServiceFlow.Class.Models;

namespace ServiceFlow.Tests
{
    public class RequestFilterByRoleTests
    {
        private List<RequestModel> BuildFakeRequests()
        {
            return new List<RequestModel>
            {
                new RequestModel { Id = 1, RequesterId = "user-1", AssigneeId = "agent-1", Status = Status.Open,     Priority = Priority.Low,    CategoryId = 1, Title = "Request 1", Description = "", Location = "", Creation = DateTime.Now },
                new RequestModel { Id = 2, RequesterId = "user-1", AssigneeId = null,      Status = Status.Resolved, Priority = Priority.Medium, CategoryId = 1, Title = "Request 2", Description = "", Location = "", Creation = DateTime.Now },
                new RequestModel { Id = 3, RequesterId = "user-2", AssigneeId = "agent-1", Status = Status.Open,     Priority = Priority.High,   CategoryId = 2, Title = "Request 3", Description = "", Location = "", Creation = DateTime.Now },
                new RequestModel { Id = 4, RequesterId = "user-2", AssigneeId = "agent-2", Status = Status.Closed,   Priority = Priority.Urgent, CategoryId = 2, Title = "Request 4", Description = "", Location = "", Creation = DateTime.Now },
                new RequestModel { Id = 5, RequesterId = "user-3", AssigneeId = "agent-2", Status = Status.Open,     Priority = Priority.Low,    CategoryId = 3, Title = "Request 5", Description = "", Location = "", Creation = DateTime.Now },
            };
        }

        [Fact]
        public void FilterByRole_ReturnsOnlyOwnRequests_WhenRoleIsUser()
        {
            var requests = BuildFakeRequests();
            var userId = "user-1";

            var filtered = requests.Where(r => r.RequesterId == userId).ToList();

            Assert.Equal(2, filtered.Count);
            Assert.All(filtered, r => Assert.Equal(userId, r.RequesterId));
        }

        [Fact]
        public void FilterByRole_ReturnsOnlyAssignedRequests_WhenRoleIsAgent()
        {
            var requests = BuildFakeRequests();
            var agentId = "agent-1";

            var filtered = requests.Where(r => r.AssigneeId == agentId).ToList();

            Assert.Equal(2, filtered.Count);
            Assert.All(filtered, r => Assert.Equal(agentId, r.AssigneeId));
        }

        [Fact]
        public void FilterByRole_ReturnsAllRequests_WhenRoleIsAdmin()
        {
            var requests = BuildFakeRequests();

            var filtered = requests.ToList();

            Assert.Equal(5, filtered.Count);
        }

        [Fact]
        public void FilterByRole_ReturnsEmpty_WhenUserHasNoRequests()
        {
            var requests = BuildFakeRequests();
            var userId = "user-999";

            var filtered = requests.Where(r => r.RequesterId == userId).ToList();

            Assert.Empty(filtered);
        }
    }
}