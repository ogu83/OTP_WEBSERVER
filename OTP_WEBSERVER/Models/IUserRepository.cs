using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OTP_WEBSERVER.Models
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers(bool IncludeDeleted = false);
        Task<User> GetUser(ObjectId id);
        Task<User> GetUser(string username);
        Task AddUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(ObjectId id, bool DeletePermanent = false);
        Task<bool> CheckPassword(string username, string password);
        Task<string> InitiateDb();                       
    }


}