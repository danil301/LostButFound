# Lost but Found

LostButFound - это веб-платформа, созданная для помощи пользователям в поиске утерянных вещей. 
Пользователи, потерявшие предмет, могут создавать подробные запросы, предоставляя всю необходимую информацию о потерянном предмете.

## Ключевые функции

### 1. Создание Запросов

- Пользователи могут создавать подробные запросы о потерянных вещах, включая описания и место утери предмета.

### 2. Поиск и Отклик

- Пользователи, нашедшие утерянный предмет, могут просматривать запросы, оставленные другими пользователями.
- Возможность откликнуться на запрос и уведомить владельца, что предмет найден.

# Требования для Backend

## Зависимости

- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [EntityFrameworkCore.SqlServer](https://docs.microsoft.com/en-us/ef/core/providers/sql-server/)
- [AspNetCore.Cors](https://docs.microsoft.com/en-us/aspnet/core/security/cors)
- [AspNetCore.Authentication.JwtBearer](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/jwtbearer)
- [MailKit](https://www.nuget.org/packages/MailKit/)
- [Dadata](https://dadata.ru/api/)

## Установка
1. Клонируйте репозиторий:
```bash
git clone https://github.com/danil301/LostButFound.git
```
2. Перейдите в директорию проекта:
```bash
cd your_directory
```
3. Установите [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0).

4. Добавьте необходимые NuGet пакеты:
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.AspNetCore.Cors
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package MailKit
dotnet add package Dadata
```

# Настройка
## 1. Настройте подключение к базе данных в файле appsettings.json:
Замените your_connection_string на свою строку подключения для соединения с MSSQL Server
```json
{
    "ConnectionStrings": {
        "LostButFoundConnectionString": "your_connection_string"
    }
}
```
## 2. Настройте CORS и аутентификацию JWT:

В файле Program.cs убедитесь, что CORS и аутентификация JWT настроены следующим образом:
```csharp
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    var secret = builder.Configuration.GetValue<string>("Secret");
    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = "https://localhost:7000/",
        ValidIssuer = "https://localhost:7000/",
        IssuerSigningKey = key,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
```
## 3. Настройте MailKit в соответствии с документацией данного пакета.

## 4. Настройте Dadata в файле Controller/ThingController.cs 
Перейдите в файл ThingController.cs по указанному пути и измените в методе EditData токен(token) и секретный ключ(secret), поставив свои значения.
```csharp
        [HttpPost]
        public async Task<IActionResult> EditData(string data)
        {
            var token = "your_dadata_token";
            var secret = "your_dadata_secret_key";
            var api = new CleanClientAsync(token, secret);
            var result = await api.Clean<Address>(data);

            List<string> metroList = new();
            foreach (var item in result.metro.ToList())
            {
                if (!metroList.Contains(item.name)) metroList.Add(item.name);
            }

            Data resultData = new Data()
            {
                Region = result.region,
                District = result.city_district,
                Street = result.street,
                Metro = metroList
            };

            return Ok(resultData);
        }
```
# Запуск Backend:
Убедитесь, что все зависимости установлены и настройки базы данных, CORS, аутентификации JWT, MailKit и Dadata выполнены корректно. 
После чего введите для запуска:
```bash
dotnet run
```
