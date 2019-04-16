using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OTP_WEBSERVER.Models
{
    public interface IApplicationRepository
    {
        Task<Application> Get(ObjectId id, bool includeDeleted = false);
        Task Add(Application a);
        Task<bool> Update(Application a);
        Task<bool> Delete(ObjectId id, bool DeletePermanent = false);
        Task<IEnumerable<Application>> GetUsersApplications(ObjectId userId, bool includeDeleted = false);
    }
}