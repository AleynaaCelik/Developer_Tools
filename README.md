
📦 Projeyi Çalıştırma Adımları
1. Proje Kurulumu
bash
Kopyala
Düzenle
# Yeni klasör oluştur ve içine gir
mkdir DevTools.API
cd DevTools.API

# .NET 8 Web API projesi oluştur
dotnet new webapi -n DevTools.API
cd DevTools.API

# Gerekli NuGet paketlerini ekle
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation.AspNetCore
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package System.IdentityModel.Tokens.Jwt
2. Dosya Yapısı
pgsql
Kopyala
Düzenle
DevTools.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── CodeAnalysisController.cs
│   ├── UserController.cs
│   └── HealthController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── Entities/
│   │   ├── User.cs
│   │   └── CodeAnalysisSession.cs
│   └── DTOs/
│       ├── CodeAnalysisRequest.cs
│       ├── CodeAnalysisResponse.cs
│       └── AuthDTOs.cs
├── Services/
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── IOpenAIService.cs
│   ├── OpenAIService.cs
│   ├── ICodeAnalysisService.cs
│   └── CodeAnalysisService.cs
├── Program.cs
└── appsettings.json
3. appsettings.json Ayarları
json
Kopyala
Düzenle
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DevToolsDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "ExpirationInDays": 7
  },
  "OpenAI": {
    "ApiKey": "sk-your-openai-api-key-here",
    "Model": "gpt-4",
    "MaxTokens": 2000,
    "Temperature": 0.3
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
4. Veritabanı Migrasyonu
bash
Kopyala
Düzenle
dotnet ef migrations add InitialCreate
dotnet ef database update
5. OpenAI API Anahtarını Ayarla
OpenAI platformuna git.

Yeni bir API key oluştur.

appsettings.json içindeki OpenAI:ApiKey kısmına yapıştır.

6. Projeyi Çalıştır
bash
Kopyala
Düzenle
# Normal başlatma
dotnet run

# Watch mode (otomatik yeniden başlatma)
dotnet watch run
🔧 Test Etme
Swagger UI
https://localhost:7xxx/swagger (port değişebilir)

Health Check
bash
Kopyala
Düzenle
curl https://localhost:7xxx/health
Kullanıcı Kaydı
bash
Kopyala
Düzenle
curl -X POST https://localhost:7xxx/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "firstName": "Test",
    "lastName": "User"
  }'
Giriş (Login)
bash
Kopyala
Düzenle
curl -X POST https://localhost:7xxx/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!"
  }'
Kod Analizi (Token ile)
bash
Kopyala
Düzenle
curl -X POST https://localhost:7xxx/api/codeanalysis/review \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE" \
  -d '{
    "code": "function hello() { console.log(\"Hello World\"); }",
    "language": "javascript",
    "fileName": "test.js",
    "includePerformance": true,
    "includeSecurity": true,
    "includeCodeStyle": true,
    "includeOptimization": true
  }'
📊 API Endpointleri
🔐 Authentication
POST /api/auth/register – Yeni kullanıcı kaydı

POST /api/auth/login – Giriş işlemi

POST /api/auth/refresh – JWT yenileme

🧠 Code Analysis (JWT gerekli)
POST /api/codeanalysis/analyze – Genel analiz

POST /api/codeanalysis/review – Kod review

POST /api/codeanalysis/documentation – Dokümantasyon üretimi

POST /api/codeanalysis/bugs – Bug tespiti

POST /api/codeanalysis/tests – Test case üretimi

GET /api/codeanalysis/usage – Kullanım istatistikleri

👤 User Management
GET /api/user/profile – Profil bilgisi

⚙️ System
GET /health – Health check

GET /api/health/version – Versiyon bilgisi

🐛 Sık Karşılaşılan Sorunlar
1. OpenAI API Error 401
API key doğru mu?

OpenAI kullanım hakkın var mı?

2. Database Connection Error
SQL Server LocalDB kurulu mu?

appsettings.json bağlantı bilgileri doğru mu?

3. JWT Token Error
Secret key en az 32 karakter mi?

Token "Bearer " prefix’i ile mi gönderildi?

📄 Loglar
bash
Kopyala
Düzenle
# Konsol logları
dotnet run

# Dosya logları
tail -f logs/devtools-*.txt
🔄 Sonraki Adımlar
⏭️ React frontend geliştirme

⏭️ Monaco Editor entegrasyonu

⏭️ Real-time özellikler (SignalR)

⏭️ Dosya yükleme ve işleme

⏭️ GitHub entegrasyonu

⏭️ Docker containerization

⏭️ Azure/AWS deployment

💡 Geliştirme Önerileri
🔁 Caching: Redis ile yanıtları cache’le

🧱 Rate Limiting: AspNetCoreRateLimit paketini kullan

✔️ Validation: FluentValidation kurallarını detaylandır

📈 Monitoring: Application Insights entegre et

🔐 Security: HTTPS zorunlu kıl, CORS yapılandır
