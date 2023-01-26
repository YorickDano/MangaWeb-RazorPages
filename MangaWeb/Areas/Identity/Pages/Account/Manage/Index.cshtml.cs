﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using MangaWeb.APIClients;
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;

namespace MangaWeb.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<MangaWebUser> _userManager;
        private readonly SignInManager<MangaWebUser> _signInManager;
        private readonly AnimeAndHentaiImageClient _animeAndHentaiClient;
        public readonly IStringLocalizer<SharedResource> Localizer;

        public IndexModel(
            UserManager<MangaWebUser> userManager,
            SignInManager<MangaWebUser> signInManager,
            AnimeAndHentaiImageClient animeAndHentaiImageClient,
            IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _animeAndHentaiClient = animeAndHentaiImageClient;
            Localizer = localizer;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }
        public string Image { get; set; }
        public MangaWebUser MangaWebUser { get; set; }

        [Required]
        [BindProperty]
        public IFormFile FormFile { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(MangaWebUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Image = _userManager.Users.FirstOrDefault(x => x.Id == user.Id).ProfileImage;
            MangaWebUser = user;
            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            LanguageManager.IsEnglish = Request.Cookies.Where(x => x.Key.Contains("Culture"))
                .Any(x => x.Value.Contains("en"));

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddImageAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (FormFile == null || user == null)
            {
                StatusMessage = "Something went wrong";
                return Page();
            }
            using (var ms = new MemoryStream())
            {
                FormFile.CopyTo(ms);
                var base64 = Convert.ToBase64String(ms.ToArray());
                var imgSrc = String.Format("data:image/jpg;base64,{0}", base64);
                user.ProfileImage = imgSrc;
            }

            await _userManager.UpdateAsync(user);
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostSetRandomHentaiProfileImageAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            //   user.ProfileImage = await _animeAndHentaiClient.GetRandomImageAsByteArray(AnimeType.Hentai);

            await _userManager.UpdateAsync(user);
            await LoadAsync(user);

            return Page();
        }
        public async Task<IActionResult> OnPostSetRandomAnimeProfileImageAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            user.ProfileImage = await _animeAndHentaiClient.GetRandomImageAsString();

            await _userManager.UpdateAsync(user);
            await LoadAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostChangeLanguageAsync(string language)
        {
            if (language == "en")
            {
                LanguageManager.Set(Response, language);
            }
            else
            {
                LanguageManager.Set(Response, language);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            return Redirect(Request.Path);
        }
    }
}
