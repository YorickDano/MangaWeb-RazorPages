// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MangaWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using MangaWeb.Managers;
using System.Text.Encodings.Web;

namespace MangaWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<MangaWebUser> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<MangaWebUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string EmailConfirmationUrl { get; set; }
        public bool IsMailValidationWait = false;
        [BindProperty]
        public string CodeForActivation { get; set; }
        public async Task MailValidation(string email, string code)
        {
            new MailManager().SendMailOnRegestration(email, code);
            IsMailValidationWait = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(int.Parse(TempData["CodeForActivation"].ToString()) == int.Parse(CodeForActivation))
            {
                var user = await _userManager.FindByEmailAsync(TempData["UserMail"].ToString());
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = TempData["ReturnUrl"].ToString() },
                    protocol: Request.Scheme);
                return Redirect(EmailConfirmationUrl);
            }

            return Page();          
        }
        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }
            Email = email;
            returnUrl = returnUrl ?? Url.Content("~/");

            TempData["UserMail"] = email;
            TempData["ReturnUrl"] = returnUrl;
            var code = new Random().Next(10000, 100000);
            TempData["CodeForActivation"] = code;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }
           
            await MailValidation(email, code.ToString());
            return Page();
        }
    }
}
