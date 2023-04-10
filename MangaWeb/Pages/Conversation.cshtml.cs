using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace MangaWeb.Pages
{
    [Authorize]
    public class ConversationModel : PageModel
    {
        private readonly UserManager<MangaWebUser> _userManager;
        private readonly MangaWebContext _context;
        private readonly IServiceProvider _serviceProvider;

        public readonly IStringLocalizer<SharedResource> Localizer;
        public Conversation Conversation { get; set; }
        public string CurrentUserName;
        public string UserWriteToName;
        public string UserWriteToImageSrc;
        private bool IsFirtLoad = true;

        public ConversationModel(IStringLocalizer<SharedResource> localizer,
            UserManager<MangaWebUser> userManager,
            MangaWebContext context,
            IServiceProvider serviceProvider)
        {
            Localizer = localizer;
            _userManager = userManager;
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public async Task<IActionResult> OnGetAsync(string userNameWriteTo)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            CurrentUserName = currentUser.UserName;
            var userWriteTo = await _userManager.FindByNameAsync(userNameWriteTo);
            UserWriteToName = userWriteTo.UserName;
            UserWriteToImageSrc = userWriteTo.ProfileImage;
            var conversation = _context.Conversations.Include(x => x.Messages).Include(x => x.FirstUser)
                .Include(x => x.SecondUser).FirstOrDefault(x =>
               (x.FirstUser.UserName == currentUser.UserName &&
               x.SecondUser.UserName == userNameWriteTo) || (x.FirstUser.UserName == userNameWriteTo &&
               x.SecondUser.UserName == currentUser.UserName));
            if (conversation == null)
            {
                Conversation = new Conversation()
                {
                    FirstUser = currentUser,
                    FirstUserId = currentUser.Id,
                    SecondUser = userWriteTo,
                    SecondUserId = userWriteTo.Id,
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
            if (string.IsNullOrEmpty(messageInput))
            {
                return RedirectToPage("Conversation", new { userNameWriteTo });
            }
            var currentUser = await _userManager.GetUserAsync(User);
            Conversation = _context.Conversations.Include(x => x.Messages).Include(x => x.FirstUser)
                .Include(x => x.SecondUser).FirstOrDefault(x =>
               (x.FirstUser.UserName == currentUser.UserName &&
               x.SecondUser.UserName == userNameWriteTo) || (x.FirstUser.UserName == userNameWriteTo &&
               x.SecondUser.UserName == currentUser.UserName));

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
            return RedirectToPage("Conversation", new { userNameWriteTo });
        }

        public async Task<IActionResult> OnGetMarkMessageAsViewedAsync(int messageId, string userNameWriteTo)
        {
            using IServiceScope scope =
                _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var currentUser = await _userManager.GetUserAsync(User);
            var context = scope.ServiceProvider.GetService<MangaWebContext>();

            var message = await context.Messages.FirstOrDefaultAsync(m => m.Id == messageId);

            message.IsViewed = message.UserNameFrom != currentUser.UserName;
            context.Messages.Update(message);

            await context.SaveChangesAsync();


            return new JsonResult(new { success = true });

        }
    }
}
