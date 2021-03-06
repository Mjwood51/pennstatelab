﻿using PennState.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Data.Entity.Validation;
using PennState.ViewModels;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Web.Configuration;
using AutoMapper;
using PennState.App_Start;

namespace PennState.Controllers
{
    
    public class AccountController : Controller
    {
        //This is the Entity Framework data context class that is used to query tables within the SQL Server database
        //
        private PennStateDB _context;

        public AccountController()
        {
            _context = new PennStateDB();
        }

        public ActionResult Index()
        {
            return View();
        }

        //GET: Account
        //This is the method to "Get Users" but is not applied to anything in the database
        public ActionResult GetUsers()
        {
            using (PennStateDB _context = new PennStateDB())
            {
                var user = _context.Tbl_Users.Include(c => c.Tbl_Roles).ToList();
                return View(Mapper.Map<IEnumerable<User>>(user));
            }
        }


        //This is a method to view Account Details, but a view page is not set up for the function
        //
        public ActionResult AccountDetails(int id)
        {
            var user = new Tbl_Users();
            using (PennStateDB _context = new PennStateDB())
            {
                user = _context.Tbl_Users.SingleOrDefault(c => c.Id == id);
            }
            if (user == null)
                return HttpNotFound();

            return View(Mapper.Map<Tbl_Users, User>(user));
        }

        //This method will dispose of used objects after processes have completed
        //
        protected override void Dispose(bool disposing)
        {
            using (PennStateDB _context = new PennStateDB())
            {
                _context.Dispose();
            }
        }

        //This method retrieves a hashed version of the passwords that are submitted with registration
        //
        private string GetHashedPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
        
        //Validation to check if username exists
        //
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingUsername(string userName)
        {
            try
            {
                return Json(!IsUsernameExists(userName));
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        //^^ Called from the method above to check if Username exists
        //
        private bool IsUsernameExists(string userName)
            => _context.Tbl_Users.Where(x => x.UserName == userName).FirstOrDefault() != null;


        //Validation to check if email exists
        //
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingEmail(string Email)
        {
            try
            {
                return Json(!IsEmailExists(Email));
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        //^^ Called from the method above to check if email exists
        private bool IsEmailExists(string email)
            => _context.Tbl_Users.Where(x=>x.Email == email).FirstOrDefault() != null;

        //This is the function that returns the Viewpage for the Login
        //It returns a partial view for the modal
        [HttpGet]
        public ActionResult Login(string ReturnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return LogOut();
            }
            if(ReturnUrl != null)
            ViewBag.ReturnUrl = ReturnUrl;
            return PartialView("Login");
        }


        //This is the post method for Logging in
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.LoginView loginView, string ReturnUrl = "")
        {
            if (ModelState.IsValid)
            {

                if (Membership.ValidateUser(loginView.UserName, loginView.Password))
                {
                    using (PennStateDB _context = new PennStateDB())
                    {
                        var user = Mapper.Map<Tbl_Users, User>(_context.Tbl_Users.Where(x => x.UserName == loginView.UserName).FirstOrDefault());
                        if (user != null)
                        {
                            CustomSerializeModel userModel = new CustomSerializeModel()
                            {
                                UserId = user.Id,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                RoleName = user.Roles.RoleName
                            };

                            string userData = JsonConvert.SerializeObject(userModel);
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket
                                (
                                1, loginView.UserName, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData
                                );

                            string enTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpCookie faCookie = new HttpCookie("Cookie1", enTicket);
                            Response.Cookies.Add(faCookie);
                        }
                    }

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Something Wrong : Username or Password invalid ^_^ ");
            return View(loginView);
        }

        //This is the method that retrieves the form which Administrators can invite users to the website
        //
        [CustomAuthorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddUser()
        {
            using (PennStateDB _context = new PennStateDB())
            {
                var roletypes = Mapper.Map<IEnumerable<Role>>(_context.Tbl_Roles.ToList());
                var viewModel = new AddUserViewModel
                {
                    RoleTypes = roletypes
                };
                return View(viewModel);
            }
        }

        //This is the post action method after an Admin submits an invitation to a new User
        //
        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddUser(AddUserViewModel model)
        {

            using (PennStateDB _context = new PennStateDB())
            {
                var roleName = Mapper.Map<Role>(_context.Tbl_Roles.Where(x => x.Id == model.User.RoleId).FirstOrDefault());


                var str = model.User.Email + "Hello4Pizza" + roleName.RoleName;
                var encr = Crypto.EncryptStringAES(str, "HelloPizza");

                var url = "/account/registration/?id=" + encr;

                //It is necessary that the email and password credentials are set up correctly to whomever the administrator is
                //
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);
                using (var message = new System.Net.Mail.MailMessage("mmw5709@psu.edu", model.User.Email.ToString()))
                {
                    message.Subject = "Activation Account !";
                    message.Body = "<br/> Please click on the following link in order to register!" + "<br/><a href='" + link + "'> Account Registration ! </a>";
                    message.IsBodyHtml = true;

                    using (SmtpClient client = new SmtpClient
                    {
                        EnableSsl = true,
                        Host = "authsmtp.psu.edu",
                        Port = 587,
                        Credentials = new NetworkCredential("mmw5709@psu.edu", "Dodgerfan42")
                    })
                    {
                        client.Send(message);
                    }
                }
                   
                
                TempData["SM"] = "Email sent to " + model.User.Email + "!";
            }
            return RedirectToAction("AddUser", "Account");
        }

        //This method retrieves the registration view page with query string decrypted
        //
        [HttpGet]
        public ActionResult Registration(string id)
        {
            var decryption = Crypto.DecryptStringAES(id, "HelloPizza");

            if (decryption != null)
            {
                var index = decryption.IndexOf("Hello4Pizza");
                var email = decryption.Substring(0, index);
                var role = decryption.Substring(index + 11);
                var model = new RegistrationView
                {
                    Email = email,
                    Role = role
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //This is the post method for registration
        //
        [HttpPost]
        public ActionResult Registration(RegistrationView registrationView)
        {

            using (PennStateDB _context = new PennStateDB())
            {
                var role = Mapper.Map<Tbl_Roles, Role>(_context.Tbl_Roles.Where(x => x.RoleName == registrationView.Role).FirstOrDefault());
            
                bool statusRegistration = false;
                string messageRegistration = string.Empty;

                if (ModelState.IsValid)
                {
                        //Check if email is unique
                        if (_context.Tbl_Users.Any(x => x.Email == registrationView.Email))
                        {
                            ModelState.AddModelError("", "This Email Address is already being used.");
                            return View(registrationView);
                        }

                        //Check if username is unique
                        if (_context.Tbl_Users.Any(x => x.UserName == registrationView.Username))
                        {
                            ModelState.AddModelError("", "This Username is already being used.");
                            return View(registrationView);
                        }
                        // Email Verification  
                        string userName = Membership.GetUserNameByEmail(registrationView.Email);
                        if (!string.IsNullOrEmpty(userName))
                        {
                            ModelState.AddModelError("Warning Email", "Sorry: Email already Exists");
                            return View(registrationView);
                        }
                        //Save User Data 
                        string acode = "";
                        var user = new User()
                        {
                            UserName = registrationView.Username,
                            FirstName = registrationView.FirstName,
                            LastName = registrationView.LastName,
                            Email = registrationView.Email,
                            PasswordHashed = GetHashedPassword(registrationView.Password),
                            ActivationCode = Guid.NewGuid(),
                            RoleId = role.Id
                        };
                        acode = user.ActivationCode.ToString();
                        _context.Tbl_Users.Add(Mapper.Map<User, Tbl_Users>(user));
                        _context.SaveChanges();

                        //Verification Email  
                        VerificationEmail(registrationView.Email, acode);
                        messageRegistration = "Your account has been created successfully. ^_^";
                        statusRegistration = true;
                }
                else
                {
                    messageRegistration = "Something Wrong!";
                }
            
                ViewBag.Message = messageRegistration;
                ViewBag.Status = statusRegistration;
            }
            return View(registrationView);
        }

        [HttpGet]
        public ActionResult ActivationAccount(string id)
        {
            bool statusAccount = false;
            using (PennStateDB dbContext = new PennStateDB())
            {
                var userAccount = dbContext.Tbl_Users.Where(u => u.ActivationCode.ToString().Equals(id)).FirstOrDefault();

                if (userAccount != null)
                {
                    userAccount.IsActive = true;
                    dbContext.SaveChanges();
                    statusAccount = true;
                }
                else
                {
                    ViewBag.Message = "Something Wrong !!";
                }

            }
            ViewBag.Status = statusAccount;
            return View();
        }

        public ActionResult LogOut()
        {
            HttpCookie cookie = new HttpCookie("Cookie1", "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", null);
        }

        [NonAction]
        public void VerificationEmail(string email, string activationCode)
        {
            var url = string.Format("/Account/ActivationAccount/{0}", activationCode);
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);

            var fromEmail = new MailAddress("MarkWoodard2227@gmail.com", "Activation Account - Penn State Physics Lab Inventory");
            var toEmail = new MailAddress(email);

            var fromEmailPassword = "Dod&erf@n42";
            string subject = "Activation Account !";

            string body = "<br/> Please click on the following link in order to activate your account" + "<br/><a href='" + link + "'> Activation Account ! </a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true

            })

                smtp.Send(message);

        }
    }
}