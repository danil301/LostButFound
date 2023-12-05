using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LostButFound.API.Services.Helpers
{
    class UploadInfo
    {
        public string Href { get; set; }
        public string Method { get; set; }
        public bool Templated { get; set; }
    }

    class UploadPhoto
    {
        static async Task<string> Main()
        {
            // Генерация уникального кода
            string uniqueCode = Guid.NewGuid().ToString("N").Substring(0, 6);

            // Путь к файлу на вашем компьютере
            string filePath = @"D:\videos\19.jpg";

            // Путь, по которому следует загрузить файл на Яндекс.Диск
            string uploadPath = "app:/" + uniqueCode + ".jpg";

            // Токен Яндекс.Диска
            string yandexDiskToken = "y0_AgAAAAAvPfWkAArxEQAAAADz55gnmJi9eOPCQjqX4NzzwEp64vonKb4";

            // Формирование URL для запроса на загрузку файла
            var uploadUrl = $"https://cloud-api.yandex.net/v1/disk/resources/upload?path={Uri.EscapeDataString(uploadPath)}&overwrite=true";

            // Добавление токена в заголовок запроса
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + yandexDiskToken);

                // Отправка запроса на получение URL для загрузки
                var response = await client.GetAsync(uploadUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Чтение URL из ответа
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var uploadInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UploadInfo>(jsonString);

                    // Загрузка файла на Яндекс.Диск
                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentRange = new ContentRangeHeaderValue(0, fileStream.Length - 1, fileStream.Length);

                        var fileUploadResponse = await client.PutAsync(uploadInfo.Href, fileContent);

                        if (fileUploadResponse.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Файл успешно загружен на Яндекс.Диск");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка при загрузке файла на Яндекс.Диск. Код: " + fileUploadResponse.StatusCode);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка при получении URL для загрузки. Код: " + response.StatusCode);
                }

            }
            return uniqueCode + ".jpg";
        }
    }

}
