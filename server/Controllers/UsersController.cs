using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Core.Domain.Dtos;
using WebApi.Core.Domain.Entities;
using WebApi.Core.Services;
using WebApi.Helpers;

namespace WebApi.Controllers {
    [Authorize]
    [Route ("[controller]")]
    public class UsersController : Controller {
        private IUserService _userService;
        private ITransactionService _transactionService;
        private IEmailService _emailService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController (
            IEmailService emailSerivice,
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings, ITransactionService transactionSerivice) {
            _userService = userService;
            _transactionService = transactionSerivice;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailSerivice;
        }

        [AllowAnonymous]
        [HttpPost ("authenticate")]
        public IActionResult Authenticate ([FromBody] UserDto userDto) {
            var user = _userService.Authenticate (userDto.Email, userDto.Password);

            if (user == null)
                return BadRequest ("Email or password is incorrect");

            var tokenHandler = new JwtSecurityTokenHandler ();
            var key = Encoding.ASCII.GetBytes (_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (new Claim[] {
                new Claim (ClaimTypes.Name, user.Id.ToString ())
                }),
                Expires = DateTime.UtcNow.AddDays (7),
                SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken (tokenDescriptor);
            var tokenString = tokenHandler.WriteToken (token);

            // return basic user info (without password) and token to store client side
            return Ok (new {
                Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Token = tokenString,
                    isVerified = user.isVerified
            });
        }

        [AllowAnonymous]
        [HttpPost ("register")]
        public IActionResult Register ([FromBody] UserDto userDto) {
            // map dto to entity
            var user = _mapper.Map<User> (userDto);

            try {
                // save 
                _userService.Create (user, userDto.Email, userDto.Password);
                _transactionService.Create (1, user.Id, true, Account.PW, 500.0m, "Registration Bonus Transaction");

                return Ok ();
            } catch (AppException ex) {
                // return error message if there was an exception
                return BadRequest (ex.Message);
            }
        }

        [HttpGet ("allFrom/{userId:int}")]
        public IActionResult GetAll (int userId) {
            var users = _userService.GetAll ();
            IList<UserDto> userDtos = _mapper.Map<IList<UserDto>> (users).ToList ();

            return Ok (userDtos.Where (x => x.Id != userId && x.Id != 1));
        }

        [HttpGet ("GetById/{id:int}")]
        [Route ("GetById/id:int")]
        public IActionResult GetById (int id) {
            var user = _userService.GetById (id);
            var userDto = _mapper.Map<UserDto> (user);
            return Ok (userDto);
        }

        [HttpPut ("{id}")]
        public IActionResult Update (int id, [FromBody] UserDto userDto) {
            // map dto to entity and set id
            var user = _mapper.Map<User> (userDto);
            user.Id = id;

            try {
                // save 
                _userService.Update (user, userDto.Password);
                return Ok ();
            } catch (AppException ex) {
                // return error message if there was an exception
                return BadRequest (ex.Message);
            }
        }

        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {
            _userService.Delete (id);
            return Ok ();
        }

        // todo sinth ...
        [HttpGet ("getBalance/{id:int}")]
        [Route ("getBalance/{id:int}")]
        public IActionResult getBalance (int id) {
            decimal balance = _userService.getBalanceForUser (id);
            return Ok (balance);
        }

        [AllowAnonymous]
        [HttpGet ("resetPassword")]
        public IActionResult resetPassword (string email) {
            var user = _userService.GetAll ().ToList ().Where (x => x.Email == email).FirstOrDefault ();
            if (user == null) {
                return BadRequest ("User not found.");
            }

            var result = _emailService.ResetPassword (email);
            return Ok ("Message send.");
        }

        [HttpPost ("verifyEmail")]
        public IActionResult verifyEmail (string email) {
            var user = _userService.GetAll ().ToList ().Where (x => x.Email == email).FirstOrDefault ();
            if (user == null) {
                return BadRequest ("User not found.");
            }

            var result = _emailService.VerifyEmail (email);

            return Ok ("Verification message send.");
        }

        [HttpGet ("verifyEmailCode")]
        public IActionResult verifyEmailCode (string code, int userid) {
            var user = _userService.GetAll ().ToList ().Where (x => x.Id == userid).FirstOrDefault ();

            if (user == null) {
                return BadRequest ("User not found.");
            }

            var selected_user = _userService.checkVerificationCode (code, user);
            selected_user.PasswordHash = null;
            selected_user.PasswordSalt = null;

            return Ok (selected_user);
        }
    }
}