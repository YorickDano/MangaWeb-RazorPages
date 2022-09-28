namespace MangaWeb.Models
{
    public class MangaReadPage
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public string ImageUrl { get; set; }
        public MangaRead MangaRead { get; set; }

        public MangaReadPage(int pageNumber, string imageUrl)
        {
            PageNumber = pageNumber;
            ImageUrl = imageUrl;
        }
    }
}
