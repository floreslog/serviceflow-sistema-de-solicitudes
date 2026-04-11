using Microsoft.EntityFrameworkCore;
using ServiceFlow.Class.Data;
using ServiceFlow.Class.Models;
namespace ServiceFlow.Class.Repositories
{
    public class RequestRepository : Repository<RequestModel>, IRepository<RequestModel>
    {
        public RequestRepository(ServiceFlowDB db) : base(db)
        {
        }
        public override async Task<ICollection<RequestModel>> GetAll()
        {
            return await dbSet
                .Include(r => r.Category)
                .Include(r => r.Requester)
                .Include(r => r.Assignee)
                .ToListAsync();
        }
    }
}