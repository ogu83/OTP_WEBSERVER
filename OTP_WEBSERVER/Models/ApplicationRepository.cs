using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OTP_WEBSERVER.Models
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly DBContext _context = null;

        public ApplicationRepository(IOptions<Settings> settings)
        {
            _context = new DBContext(settings);
        }

        private async Task validate(Application a)
        {
            if (string.IsNullOrWhiteSpace(a.Name))
                throw new ArgumentException("Name required");
            else if (string.IsNullOrWhiteSpace(a.SharedKey))
                throw new ArgumentException("SharedKey required");
            else if (string.IsNullOrWhiteSpace(a.SecretKey))
                throw new ArgumentException("SecretKey");

            var sharedKeyApp = await Get(a.SharedKey);
            if (sharedKeyApp != null)
                if (sharedKeyApp.Id != sharedKeyApp.Id)
                    throw new ArgumentException("SharedKey already exists");
        }

        public async Task Add(Application a)
        {
            try
            {
                await validate(a);
                a.UpdatedOn = DateTime.Now;
                await _context.Applications.InsertOneAsync(a);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Delete(ObjectId id, bool DeletePermanent = false)
        {
            try
            {
                if (DeletePermanent)
                {
                    var actionResult = await _context.Applications.DeleteOneAsync(x => x.Id == id);
                    return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
                }
                else
                {
                    var filter = Builders<Application>.Filter.Eq(s => s.Id, id);
                    var update = Builders<Application>.Update
                                    .Set(s => s.IsDeleted, true)
                                    .CurrentDate(s => s.UpdatedOn);
                    var actionResult = await _context.Applications.UpdateOneAsync(filter, update);
                    return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Application> Get(ObjectId id, bool includeDeleted = false)
        {
            try
            {
                if (!includeDeleted)
                    return (await _context.Applications.FindAsync(x => x.IsDeleted == false && x.Id == id)).FirstOrDefault();
                else
                    return (await _context.Applications.FindAsync(x => x.Id == id)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Application> Get(string sharedKey, bool includeDeleted = false)
        {
            try
            {
                if (!includeDeleted)
                    return (await _context.Applications.FindAsync(x => x.IsDeleted == false && x.SharedKey == sharedKey)).FirstOrDefault();
                else
                    return (await _context.Applications.FindAsync(x => x.SharedKey == sharedKey)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Application>> GetUsersApplications(ObjectId userId, bool includeDeleted = false)
        {
            try
            {
                if (!includeDeleted)
                    return (await _context.Applications.FindAsync(x => x.IsDeleted == false && x.User_Id == userId)).ToEnumerable();
                else
                    return (await _context.Applications.FindAsync(x => x.User_Id == userId)).ToEnumerable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(Application a)
        {
            try
            {
                await validate(a);
                a.UpdatedOn = DateTime.Now;
                var filter = Builders<Application>.Filter.Eq(s => s.Id, a.Id);
                var actionResult = await _context.Applications.ReplaceOneAsync(filter, a);
                return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}