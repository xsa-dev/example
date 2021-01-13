using System;
using System.Collections.Generic;

namespace WebApi.Core.Domain.Entities {
    public class Transaction {

        public Transaction (int from, int to, Account account, bool state, decimal amount, string description) {
            this.fromId = from;
            this.toId = to;
            this.account = account;
            this.isValidated = state;
            this.amount = amount;
            this.date = DateTime.Now;
            this.description = description;
        }

        public Transaction () {

        }

        public int id { get; set; }
        public virtual User from { get; set; }
        public int fromId { get; set; }
        public virtual User to { get; set; }
        public int toId { get; set; }

        public Account account { get; set; }

        public decimal amount { get; set; }

        public bool isValidated { get; set; }

        public DateTime date {
            get;
            set;
        }

        public string correspondent { get; set; }

        public string formated_date {
            get {
                return this.date.ToString ("dd/MM/yyyy H:mm");
            }
        }

        public Direction direction { get; set; }

        public string description { get; set; }

        public virtual decimal subtotal { get; set; }
    }

    public enum Account {
        PW
    }

    public enum Direction {
        Credit,
        Debit
    }

}