using System;
using System.Collections.Generic;
using System.Linq;
using BookShopApi.Models;
using MongoDB.Driver;
using System.Security.Cryptography;

namespace BookShopApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Roles> _roles;

        public UserService(IBookstoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>("users");
            _roles = database.GetCollection<Roles>("roles");
        }

        public List<User> Get() =>
            _users.Find(user => true).ToList();

        public User Get(string id) =>
            _users.Find<User>(user => user._id == id).FirstOrDefault();

        public ResponseQueryUsers GetUsersWithQuery(int page, int pageSize)
        {
            var count = _users.Find(user => true).CountDocuments();
            var queryUsers = _users.Find(book => true)
                .Skip(page * pageSize).Limit(pageSize).ToList();
            var response = new ResponseQueryUsers();
            response.data = queryUsers;
            response.usersLength = count;
            return response;
        }

        public User GetByEmail(string email) =>
           _users.Find<User>(user => user.email == email).FirstOrDefault();

        public void Remove(string id) =>
            _users.DeleteOne(user => user._id == id);

        public User Create(User user)
        {
            var hashed = HashPassword(user.password);

            user.password = hashed;

            _users.InsertOne(user);
            updateRoles(user.email);

            return user;
        }

        public void Update(string id, User updatingUser)
        {
            _users.ReplaceOne(user => user._id == id, updatingUser);
        }

        public string GetRole(string email)
        {
            var roles = _roles.Find(roles => true).ToList();
            for(var i = 0; i < roles[0].admins.Count; i++)
            {
                if (roles[0].admins[i] == email)
                {
                    return "admin";
                }
            }
            return "user";
        }

        public void updateRoles(string email)
        {
            var currentRoles = _roles.Find(roles => true).ToList();
            currentRoles[0].users.Add(email);
            _roles.ReplaceOne(data => data._id == currentRoles[0]._id, currentRoles[0]);
        }

        public string GetUserAvatar(string id)
        {
            User currentUser = _users.Find<User>(user => user._id == id).FirstOrDefault();
            return currentUser.image;
        }

        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }
    }
}
