
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MangaWeb.Managers
{
    public class JsonManager
    {
        public async static Task<string> Serialize<T>(T obj) 
        {
            var stream = new MemoryStream();

            await JsonSerializer.SerializeAsync<T>(stream, obj);

            return Convert.ToBase64String(stream.ToArray());
        }


        public static byte[] SerializeToByteArray<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }
    }
}
