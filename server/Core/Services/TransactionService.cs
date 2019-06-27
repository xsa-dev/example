using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Core.Domain.Entities;
using WebApi.Helpers;

namespace WebApi.Core.Services {
    public interface ITransactionService {

        Transaction Create (int from, int to, bool state, Account account, decimal amount);

        Transaction Create (int from, int to, bool state, Account account, decimal amount, string description);

        IEnumerable<Transaction> GetAll ();

        Transaction GetById (int id);

        void Update (int id, bool state);

        void Delete (int id);

        IEnumerable<Transaction> GetAllByUser (User user);
    }

    public class TransactionService : ITransactionService {
        private DataContext _context;
        private IUserService _userservice;

        public TransactionService (DataContext context, IUserService userservice) {
            _context = context;
            _userservice = userservice;
        }

        public Transaction Create (int from, int to, bool state, Account account, decimal amount) {
            var created = new Transaction ();
            created.from = _userservice.GetById (from);
            created.fromId = from;
            created.to = _userservice.GetById (to);
            created.toId = to;
            created.isValidated = state;
            created.account = account;
            created.amount = amount;
            created.date = DateTime.Now;

            created.to = _userservice.GetById (to);



            _context.Add (created);
            _context.SaveChanges ();

            return created;
        }

        public Transaction Create (int from, int to, bool state, Account account, decimal amount, string description) {
            var created = new Transaction ();
            created.fromId = from;
            created.from = _userservice.GetById (from);
            created.toId = to;
            created.to = _userservice.GetById (to);
            created.isValidated = state;
            created.account = account;
            created.amount = amount;
            created.description = description;
            created.date = DateTime.Now;

            _context.Add (created);
            _context.SaveChanges ();

            return created;
        }

        public void Delete (int id) {
            throw new NotImplementedException ();
        }

        public IEnumerable<Transaction> GetAll () {
            return _context.Transactions;
        }

        public IEnumerable<Transaction> GetAllByUser (User user) {
            return _context.Transactions.ToList ().Where (x => x.toId == user.Id || x.fromId == user.Id);
        }

        public Transaction GetById (int id) {
            return _context.Transactions.SingleOrDefault (x => x.id == id);
        }

        public void Update (int id, bool state) {
            throw new NotImplementedException ();
        }

    }
}