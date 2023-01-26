// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using MangaWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<MangaWebUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public readonly IStringLocalizer<SharedResource> Localizer;

        public PersonalDataModel(
            UserManager<MangaWebUser> userManager,
            ILogger<PersonalDataModel> logger,
            IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _logger = logger;
            Localizer = localizer;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}
