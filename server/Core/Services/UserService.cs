using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Core.Domain.Entities;
using WebApi.Helpers;

namespace WebApi.Core.Services {
    public interface IUserService {
        User Authenticate (string email, string password);
        IEnumerable<User> GetAll ();
        User GetById (int id);
        User Create (User user, string email, string password);
        void Update (User user, string password = null);
        void Delete (int id);

        decimal getBalanceForUser (int id);

        bool setVerificationCode (string code, User user);

        User checkVerificationCode (string code, User user);
    }

    public class UserService : IUserService {
        private DataContext _context;

        public UserService (DataContext context) {
            _context = context;
        }

        public User Authenticate (string email, string password) {
            if (string.IsNullOrEmpty (email) || string.IsNullOrEmpty (password))
                return null;

            var user = _context.Users.SingleOrDefault (x => x.Email == email);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash (password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll () {
            return _context.Users;
        }

        public User GetById (int id) {
            return _context.Users.Find (id);
        }

        public User Create (User user, string email, string password) {
            // validation
            if (string.IsNullOrWhiteSpace (password))
                throw new AppException ("Password is required");

            if (_context.Users.Any (x => x.Username == user.Username))
                throw new AppException ("Username '" + user.Username + "' is already taken");

            if (_context.Users.Any (x => x.Email == email))
                throw new AppException ("Email '" + user.Email + "' is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash (password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add (user);
            _context.SaveChanges ();

            return user;
        }

        public void Update (User userParam, string password = null) {
            var user = _context.Users.Find (userParam.Id);

            if (user == null)
                throw new AppException ("User not found");

            if (userParam.Username != user.Username) {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any (x => x.Username == userParam.Username))
                    throw new AppException ("Username " + userParam.Username + " is already taken");
            }

            // update user properties
            user.Username = userParam.Username;

            if (user.VerificationCode != null) {
                user.VerificationCode = userParam.VerificationCode;
            }

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace (password)) {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash (password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update (user);
            _context.SaveChanges ();
        }

        public void Delete (int id) {
            var user = _context.Users.Find (id);
            if (user != null) {
                _context.Users.Remove (user);
                _context.SaveChanges ();
            }
        }

        // private helper methods

        private static void CreatePasswordHash (string password, out byte[] passwordHash, out byte[] passwordSalt) {
            if (password == null) throw new ArgumentNullException ("password");
            if (string.IsNullOrWhiteSpace (password)) throw new ArgumentException ("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512 ()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash (System.Text.Encoding.UTF8.GetBytes (password));
            }
        }

        private static bool VerifyPasswordHash (string password, byte[] storedHash, byte[] storedSalt) {
            if (password == null) throw new ArgumentNullException ("password");
            if (string.IsNullOrWhiteSpace (password)) throw new ArgumentException ("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException ("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException ("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512 (storedSalt)) {
                var computedHash = hmac.ComputeHash (System.Text.Encoding.UTF8.GetBytes (password));
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public decimal getBalanceForUser (int id) {
            // Function for all transactions to User
            var transactions = _context.Transactions.ToList ();

            // balance = debit - credit
            var user_debit = _context.Transactions
                .Where (x => x.isValidated == true)
                .Where (x => x.toId == id)
                .ToList ()
                .Sum (x => x.amount);

            var user_credit = _context.Transactions
                .Where (x => x.fromId == id)
                .Where (x => x.isValidated == true)
                .ToList ()
                .Sum (x => x.amount);

            var balance = user_debit - user_credit;

            return balance;
        }

        public bool setVerificationCode (string code, User user) {
            try {
                //_context.Users.Update (user);
                //_context.SaveChanges ();
                return true;
            } catch (Exception exception) {
                return false;
            }
        }

        public User checkVerificationCode (string code, User user) {
            try {
                User selected_user = _context.Users.Where (x => x.Id == user.Id).FirstOrDefault ();

                if (selected_user.VerificationCode == code) {
                    selected_user.isVerified = true;
                    _context.SaveChanges ();
                    return selected_user;
                }

                return selected_user;
            } catch (Exception exception) {
                return null;
            }
        }
    }
}