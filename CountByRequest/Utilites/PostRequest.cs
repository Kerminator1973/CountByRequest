using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace CountByRequest.Utilites
{
    public class PostRequest
    {
        public string Currency { get; set; } = "USD";
        public int Notes { get; set; }
        public int Denomination { get; set; }
    }

    public class PostSender
    {
        private readonly HttpClient _httpClient;

        public PostSender()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> SendPostAsync(PostRequest postRequest, string url)
        {
            string result = "Post request sent successfully.";
            try
            {
                // Сериализуем экземпляр класса PostRequest в текстовую строку, содержащую JSON
                string jsonString = JsonSerializer.Serialize(postRequest, new JsonSerializerOptions { WriteIndented = true });
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Посылаем post-запрос по указанному URL
                var response = await _httpClient.PostAsync(url, content);

                // Проверяем результат - удалось ли доставить запрос на сервер
                if (!response.IsSuccessStatusCode)
                {
                    result = $"Error sending post request: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                result = $"Exception occurred: {ex.Message}";
            }

            return result;
        }
    }
}


