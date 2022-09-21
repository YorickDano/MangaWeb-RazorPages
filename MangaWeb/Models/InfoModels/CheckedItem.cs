using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MangaWeb.Models.InfoModels
{
    public class CheckedItem
    {
        [BindRequired]
        public string Gener { get; set; }
    }
}
