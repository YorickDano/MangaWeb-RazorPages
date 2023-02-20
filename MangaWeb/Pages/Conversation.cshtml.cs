using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages
{
    [Authorize]
    public class MessageModel : PageModel
    {
        private readonly UserManager<MangaWebUser> _userManager;
        private readonly MangaWebContext _context;

        public readonly IStringLocalizer<SharedResource> Localizer;
        public Conversation Conversation { get; set; }
        public string CurrentUserName;
        public string UserWriteToName;
        public string UserWriteToImageSrc;

        public MessageModel(IStringLocalizer<SharedResource> localizer,
            UserManager<MangaWebUser> userManager,
            MangaWebContext context)
        {
            Localizer = localizer;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string userNameWriteTo)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            CurrentUserName = currentUser.UserName;
            var userWriteTo = await _userManager.FindByNameAsync(userNameWriteTo);
            UserWriteToName = userWriteTo.UserName;
            UserWriteToImageSrc = userWriteTo.ProfileImage;
            var conversation = _context.Conversations.Include(x => x.Messages).FirstOrDefault(x =>
                (x.FirstUserName == currentUser.UserName &&
                x.SecondUserName == userNameWriteTo) || (x.FirstUserName == userNameWriteTo &&
                x.SecondUserName == currentUser.UserName));
            if (conversation == null)
            {
                Conversation = new Conversation()
                {
                    FirstUserName = currentUser.UserName,
                    FirstUserImageSrc = currentUser.ProfileImage,
                    SecondUserName = userNameWriteTo,
                    SecondUserImageSrc = userWriteTo.ProfileImage,
                    Created = DateTime.Now,
                    Messages = new List<Message>()
                };
                await _context.AddAsync(Conversation);
                await _context.SaveChangesAsync();
            }
            else
            {
                Conversation = conversation;
            }

            return Page();
        }
        public async Task<IActionResult> OnPostSendMessageAsync(string messageInput, string userNameWriteTo)
        {
            if(string.IsNullOrEmpty(messageInput))
            {
                return RedirectToPage("Conversation", new { userNameWriteTo });
            }
            var currentUser = await _userManager.GetUserAsync(User);
             Conversation = _context.Conversations.Include(x=>x.Messages).FirstOrDefault(x =>
                (x.FirstUserName == currentUser.UserName &&
                x.SecondUserName == userNameWriteTo) || (x.FirstUserName == userNameWriteTo &&
                x.SecondUserName == currentUser.UserName));

            var message = new Message()
            {
                Body = messageInput,
                DateTime = DateTime.Now,
                UserNameFrom = currentUser.UserName,
                UserNameTo = userNameWriteTo,
                Conversation = this.Conversation
            };
            if (Conversation.Messages == null) Conversation.Messages = new List<Message>();
            
            Conversation.Messages.Add(message);
            _context.Update(Conversation);
            await _context.SaveChangesAsync();
            return RedirectToPage("Conversation", new {userNameWriteTo});
        }
    }
}
