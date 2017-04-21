using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    [Authorize]
    public class AppController: Controller
    {
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private IWorldRepository _repository;
        private ILogger<AppController> _logger;

        public AppController(IMailService mailService, IConfigurationRoot config, IWorldRepository repository, ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _repository = repository;
            _logger = logger;
        }
        public IActionResult Index()
        {

                return View();

        }

        [Authorize]
        public IActionResult Trips()
        {
            try
            {

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get trips in Index page: {ex.Message}");
                return Redirect("/error");
            }
        }
        [Authorize]
        public IActionResult Contact()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            try
            {
                if (model.Email.Contains("aol.com"))
                { ModelState.AddModelError("", "We dont support AOL"); }
                if (ModelState.IsValid)
                {
                    var emailMessage = new MimeMessage();

                    emailMessage.From.Add(new MailboxAddress("TripPlan", "randomemailaspnet@gmail.com"));
                    emailMessage.To.Add(new MailboxAddress("", "randomemailaspnet@gmail.com"));
                    emailMessage.Subject = "From TripPlan User: " + model.Name + " email: " + model.Email;
                    emailMessage.Body = new TextPart("plain") { Text = model.Message };

                    using (var client = new SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 465);
                        client.AuthenticationMechanisms.Remove("XOAUTH2"); // Must be removed for Gmail SMTP
                        client.Authenticate(_config["MailSettings:SmtpServerUser"], _config["MailSettings:SmtpServerPass"]);
                        client.Send(emailMessage);
                        client.Disconnect(true);
                    }


                    ModelState.Clear();
                    ViewBag.UserMessage = "Message Sent";
                }
            }
            catch (Exception ex)
            {
                ViewBag.UserMessage = ex;
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
