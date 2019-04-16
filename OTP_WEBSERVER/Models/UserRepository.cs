using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OTP_WEBSERVER.Models
{
    public class UserRepository : IUserRepository
    {
        private const string adminUsername = "admin";
        private const string adminPassword = "123456";
        private const string adminEmail = "admin@otpwebserver.com";
        public const string database_already_set = "Database is already set";
        private readonly DBContext _context = null;

        public UserRepository(IOptions<Settings> settings)
        {
            _context = new DBContext(settings);
        }

        private async Task validateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username required");
            else if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email required");
            else if (await GetUser(user.Username) != null)
                throw new ArgumentException("Username already exists");
            else if (await GetUser(user.Email) != null)
                throw new ArgumentException("Email already exists");
            else if (!IsValidEmail(user.Email))
                throw new ArgumentException("Email is not valid");
        }

        private bool IsValidEmail(string email)
        {

            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public async Task AddUser(User user)
        {
            try
            {
                await validateUser(user);
                await _context.Users.InsertOneAsync(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteUser(ObjectId id, bool DeletePermanent = false)
        {
            try
            {
                if (DeletePermanent)
                {
                    var actionResult = await _context.Users.DeleteOneAsync(x => x.Id == id);
                    return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
                }
                else
                {
                    var filter = Builders<User>.Filter.Eq(s => s.Id, id);
                    var update = Builders<User>.Update
                                    .Set(s => s.IsDeleted, true)
                                    .CurrentDate(s => s.UpdatedOn);
                    var actionResult = await _context.Users.UpdateOneAsync(filter, update);
                    return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> GetUser(ObjectId id)
        {
            try
            {
                var user = (await _context.Users.FindAsync(x => x.Id == id)).FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> GetUser(string username)
        {
            try
            {
                var user = (await _context.Users.FindAsync(x => x.Username == username)).FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<User>> GetUsers(bool IncludeDeleted = false)
        {
            try
            {
                if (IncludeDeleted)
                    return (await _context.Users.FindAsync(x => true)).ToEnumerable();
                else
                    return (await _context.Users.FindAsync(x => x.IsDeleted == false)).ToEnumerable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                await validateUser(user);
                var filter = Builders<User>.Filter.Eq(s => s.Id, user.Id);
                var actionResult = await _context.Users.ReplaceOneAsync(filter, user);
                return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> CheckPassword(string username, string password)
        {
            try
            {
                var result = await _context.Users.FindAsync(x => (x.Username == username || x.Email == username) && x.Password == password);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> InitiateDb()
        {
            try
            {

                var admin = (await _context.Users.FindAsync(u => u.IsMaster)).FirstOrDefault();
                if (admin != null)
                {
                    return database_already_set;
                }
                else
                {
                    await AddUser(new User
                    {
                        Email = adminEmail,
                        IsMaster = true,
                        Password = adminPassword,
                        UpdatedOn = DateTime.Now,
                        Username = adminUsername,
                    });
                    return "Database setup completed";
                }
            }
            catch (Exception ex) 
            {
                throw ex;
            }            
        }
    }
}