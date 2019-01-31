using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SmartSchool.Models;

namespace SmartSchool.Controllers
{
    public class ParentsController : Controller
    {
        // GET: Parents/Register
        [HttpGet]
        public ActionResult Register()
        {
            if (Request.Cookies.Get("parents") != null)
            {
                return RedirectToAction("Index", "Parents");
            }
            else if (Request.Cookies.Get("admin") != null)
            {
                return RedirectToAction("Home", "ERP");
            }
            else
            {
                return View();
            }
        }

        // POST: Parents/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude = "IsEmailVerified,ActivationCode")] ParentsLogin parentsLogin)
        {
            bool Status = false;
            string message = "";

            if (ModelState.IsValid)
            {
                // Email is already Exist
                var isExist = IsEmailExist(parentsLogin.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(parentsLogin);
                }

                // Generate Activation Code
                parentsLogin.ActivationCode = Guid.NewGuid();

                // Password Hashing
                parentsLogin.Password = Crypto.Hash(parentsLogin.Password);
                parentsLogin.ConfirmPassword = Crypto.Hash(parentsLogin.ConfirmPassword);

                parentsLogin.IsEmailVerified = false;

                // Save to Database
                using (ParentsRegsEntities parentsReg = new ParentsRegsEntities())
                {
                    Parents parents = new Parents();
                    parents.Id = parentsLogin.Id;
                    if (parents.checkId())
                    {
                        parentsReg.ParentsLogins.Add(parentsLogin);
                        parentsReg.SaveChanges();

                        // Send Email to User
                        SendVerificationLinkEmail(parentsLogin.Email, parentsLogin.ActivationCode.ToString());
                        message = "Registration successfully done. Account activation link " +
                            " has been sent to your email id:" + parentsLogin.Email;
                        Status = true;
                    }
                    else
                    {
                        message = "Invalid id";
                    }
                }
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (ParentsRegsEntities parentsReg = new ParentsRegsEntities())
            {
                parentsReg.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                                        // Confirm password does not match issue on save changes
                var v = parentsReg.ParentsLogins.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    parentsReg.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        [NonAction]
        public bool IsEmailExist(string email)
        {
            using (ParentsRegsEntities parentsReg = new ParentsRegsEntities())
            {
                var v = parentsReg.ParentsLogins.Where(a => a.Email == email).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            try
            {
                var verifyUrl = "/Parents/" + emailFor + "/" + activationCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                var fromEmail = new MailAddress("emran35-1609@diu.edu.bd", "SmartSchool");
                var toEmail = new MailAddress(email);
                var fromEmailPassword = "diu123456"; // Replace with actual password

                string subject = "";
                string body = "";
                if (emailFor == "VerifyAccount")
                {
                    subject = "Your account is successfully created!";
                    body = "<br/><br/>Your Smart School account is" +
                        " successfully created. Please click on the below link to verify your account" +
                        " <br/><br/><a href='" + link + "'>" + link + "</a> ";
                }
                else if (emailFor == "ResetPassword")
                {
                    subject = "Reset Password";
                    body = "Hi!<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                        "<br/><br/><a href=" + link + ">Reset Password link</a>";
                }

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
            catch
            {

            }
        }

        // GET: Parents/Login
        [HttpGet]
        public ActionResult Login()
        {
            if (Request.Cookies.Get("parents") != null)
            {
                return RedirectToAction("Index", "Parents");
            }
            else if (Request.Cookies.Get("admin") != null)
            {
                return RedirectToAction("Home", "ERP");
            }
            else
            {
                return View();
            }
        }

        // POST: Parents/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Parents parents, string ReturnUrl = "")
        {
            string message = "";
            using (ParentsRegsEntities parentsReg = new ParentsRegsEntities())
            {
                var v = parentsReg.ParentsLogins.Where(a => a.Id == parents.Id).FirstOrDefault();
                if (v != null)
                {
                    if (!v.IsEmailVerified)
                    {
                        ViewBag.ErrorMessage = "Please verify your email first";
                        return View();
                    }
                    if (string.Compare(Crypto.Hash(parents.Password), v.Password) == 0)
                    {
                        int timeout = parents.RememberMe ? 1440 : 720; // 1440 min = 1 day && 720 min= 12 hour
                        var ticket = new FormsAuthenticationTicket(parents.Id, parents.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        HttpCookie cookie = new HttpCookie("parents", encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        Response.Cookies.Add(cookie);
                        ViewBag.User = Request.Cookies.Get("parents").Value;
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Parents");
                        }
                    }
                    else
                    {
                        message = "Invalid Id or password";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.ErrorMessage = message;
            return View();
        }

        // POST: Parents/_LogoutPartial
        //[Authorize]
        //[HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Response.Cookies["parents"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Login", "Parents");
        }

        // GET: Parents/ForgotPassword // for parents
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            if (Request.Cookies.Get("parents") != null)
            {
                return RedirectToAction("Index", "Parents");
            }
            else if (Request.Cookies.Get("admin") != null)
            {
                return RedirectToAction("Home", "ERP");
            }
            else
            {
                return View();
            }
        }

        // POST: Parents/ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            bool status = false;

            using (ParentsRegsEntities parentsReg = new ParentsRegsEntities())
            {
                var account = parentsReg.ParentsLogins.Where(a => a.Email == Email).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;

                    //Avoid confirm password not match issue, as we had added a confirm password property
                    parentsReg.Configuration.ValidateOnSaveEnabled = false;
                    parentsReg.SaveChanges();
                    ViewBag.Message = "Reset password link has been sent to your email id";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.ErrorMessage = "Sorry! account not found";
                }
            }
            return View();
        }

        // GET: Parents/ResetPassword // for parents
        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (ParentsRegsEntities parentsReg = new ParentsRegsEntities())
            {
                var user = parentsReg.ParentsLogins.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPassword reset = new ResetPassword();
                    reset.ResetCode = id;
                    return View(reset);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        // POST: Parents/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword reset)
        {
            if (ModelState.IsValid)
            {
                using (ParentsRegsEntities parentsReg = new ParentsRegsEntities())
                {
                    var user = parentsReg.ParentsLogins.Where(a => a.ResetPasswordCode == reset.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = Crypto.Hash(reset.NewPassword);
                        user.ResetPasswordCode = "";
                        parentsReg.Configuration.ValidateOnSaveEnabled = false;
                        parentsReg.SaveChanges();
                        ViewBag.message = "New password updated successfully";
                    }
                }
                ModelState.Clear();
            }
            else
            {
                ViewBag.Errormessage = "Something invalid!";
            }
            return View(reset);
        }

        //[Authorize]
        // GET: Parents/Index
        public ActionResult Index()
        {
            if (Request.Cookies.Get("parents") != null)
            {
                Parents parents = new Parents();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Parents");
            }
        }

          
    }
}