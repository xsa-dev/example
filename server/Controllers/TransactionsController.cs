using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using WebApi.Core.Domain.Entities;
using WebApi.Core.Services;
using WebApi.Helpers;
using WebApi.Hubs;

namespace WebApi.Controllers {
    // [Authorize]
    [Route ("[controller]")]
    public class TransactionsController : Controller {
        private ITransactionService _transactionService;
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IHubContext<ChatHub> _hubContext;

        public TransactionsController (
            ITransactionService transactionService,
            IUserService userService,
            IOptions<AppSettings> appSettings,
            IHubContext<ChatHub> hubContext
        ) {
            _transactionService = transactionService;
            _userService = userService;
            _appSettings = appSettings.Value;
            _hubContext = hubContext;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] Transaction transaction)
        { 
            var current_balance = _userService.getBalanceForUser(transaction.fromId);
            var plan_balance = current_balance - transaction.amount;
            if (plan_balance < 0) {
                return BadRequest("Not valid amount");
            }

            var created = _transactionService.Create (transaction.fromId, transaction.toId, transaction.isValidated, transaction.account, transaction.amount);

            var user = _userService.GetById (transaction.fromId);

            var balance = _userService.getBalanceForUser (transaction.fromId);

            _hubContext.Clients.All.SendAsync ("ReceiveMessage", user.Username, balance);

            return Ok (created);
        }

        [HttpGet ("get")]
        public IActionResult Get (int userId) {
            IList<Transaction> transactions = _transactionService.GetAll ().ToList ();

            foreach (var transaction in transactions) {

                if (transaction.fromId == 0) {
                    User user = new User ();
                    user.Id = 0;
                    user.Username = "PW";
                    user.Email = "info@pw.com";
                    transaction.from = user;
                } else {
                    transaction.from = _userService.GetById (transaction.fromId);
                    if (transaction.fromId == userId) {
                        transaction.direction = Direction.Debit;
                    } else if (transaction.fromId != userId) {
                        transaction.direction = Direction.Credit;
                    }
                }

                transaction.to = _userService.GetById (transaction.toId);

                // todo passwords hash && salt remove here
                // byte[] scrup = BitConverter.GetBytes (201805978);
                // transaction.from.PasswordHash = scrup;
                // transaction.from.PasswordSalt = scrup;
                // transaction.to.PasswordHash = scrup;
                // transaction.to.PasswordSalt = scrup;
            }

            return Ok (transactions);
        }

        [HttpGet ("{id}")]
        public IActionResult GetById (int id) {
            var transaction = _transactionService.GetById (id);
            return Ok (transaction);
        }

        [HttpGet ("GetTransactionsByUser/{userId:int}")]
        public IActionResult GetTransactionsByUser (int userId) {
            User user = _userService.GetById (userId);
            if (user == null) {
                return BadRequest ();
            }
            var transactions = _transactionService.GetAllByUser (user).ToList ();

            // todo to -> service this
            var subtotal = 0.0m;
            foreach (var transaction in transactions) {
                if (transaction.fromId == 0) {
                    User userStub = new User ();
                    user.Id = 0;
                    user.Username = "PW";
                    user.Email = "info@pw.com";
                    transaction.from = user;
                } else {
                    transaction.from = _userService.GetById (transaction.fromId);
                }

                transaction.to = _userService.GetById (transaction.toId);

                if (transaction.fromId == userId) {
                    transaction.direction = Direction.Credit;
                    subtotal -= transaction.amount;
                    transaction.correspondent = transaction.to.Username;
                } else if (transaction.toId == userId) {
                    transaction.direction = Direction.Debit;
                    transaction.correspondent = transaction.from.Username;
                    subtotal += transaction.amount;
                }
                transaction.subtotal = subtotal;

            }
            return Ok (transactions.OrderByDescending(x => x.id));
        }

        [HttpPost ("CheckTransaction")]
        public IActionResult CheckTransaction (int userId, decimal amount) {
            User user = _userService.GetById (userId);
            if (user == null) {
                return Ok (false);
            }
            decimal balance = _userService.getBalanceForUser (user.Id);

            if (balance - amount <= 0) {
                return Ok (false);
            } else {
                return Ok (true);
            }
        }
    }
}