using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using WebApi.Core.Domain.Entities;
using WebApi.Core.Services;
using WebApi.Helpers;

namespace WebApi.Core.Services {

    public interface IEmailService {
        bool SendEmailAsync (string email, string subject, string message);
        bool VerifyEmail (string email);
        bool ResetPassword (string email);

    }

    public class EmailService : IEmailService {
        IUserService _userService;

        private DataContext _context;

        const string smpt_server = "smtp.jino.ru";
        const string smpt_user = "test@alekseysavin.com";
        const string smpt_pass = "tn[RLMc7m4PD";

        public EmailService (IUserService userService, DataContext context) {
            _userService = userService;
            _context = context;
        }

        public bool ResetPassword (string email) {
            try {
                var users = _userService.GetAll ().ToList ();

                var user = users.Find (x => x.Email == email);
                var newTempPassword = RandomString (6);
                _userService.Update (user, newTempPassword);
                if (SendEmailAsync (email, "Password reset!", "Your password was successufelly Reset! <br> New password is: " + newTempPassword)) {
                    return true;
                } else {
                    return false;
                }
            } catch (Exception exception) {
                Console.WriteLine (exception.ToString ());
                return false;
            }
        }

        public bool VerifyEmail (string email) {
            try {
                var users = _userService.GetAll ().ToList ();
                var user = users.Find (x => x.Email == email);
                var verificationCode = RandomString (4);
                user.VerificationCode = verificationCode;
                var emailSend = SendEmailAsync (email, "Your verification code!", "Your verification code is: " + verificationCode);

                if (emailSend) {
                    _userService.Update (user);
                    return true;
                }
                return false;

            } catch (Exception exception) {
                return false;
            }
        }

        public static string RandomString (int length) {
            Random random = new Random ();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string (Enumerable.Repeat (chars, length)
                .Select (s => s[random.Next (s.Length)]).ToArray ());
        }

        public bool SendEmailAsync (string email, string subject, string message) {
            try {
                var emailMessage = new MimeMessage ();
                emailMessage.From.Add (new MailboxAddress ("Administration ", smpt_user));
                emailMessage.To.Add (new MailboxAddress ("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart (MimeKit.Text.TextFormat.Html) {
                    Text = message
                };

                using (var client = new SmtpClient ()) {
                    client.Connect (smpt_server, 25, false);
                    client.Authenticate (smpt_user, smpt_pass);
                    client.Send (emailMessage);

                    client.Disconnect (true);
                    return true;
                }
            } catch (Exception exception) {
                return false;
            }
        }

    }
}