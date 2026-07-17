using Microsoft.AspNetCore.Builder;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var httpClient = new HttpClient();

// Đích đến của bạn (Cloudflare)
var targetBaseUrl = "https://alpr.duahau.net"; 

app.MapPost("/lpr", async (HttpContext context) =>
{
    // Tạo request mới để forward
    var targetUrl = $"{targetBaseUrl}/lpr";
    var request = new HttpRequestMessage(HttpMethod.Post, targetUrl);

    // QUAN TRỌNG: Ghi đè Host Header
    request.Headers.Host = "alpr.duahau.net"; 

    // Copy toàn bộ body từ camera gửi sang
    request.Content = new StreamContent(context.Request.Body);
    request.Content.Headers.ContentType = context.Request.ContentType != null 
        ? System.Net.Http.Headers.MediaTypeHeaderValue.Parse(context.Request.ContentType) 
        : null;

    // Gửi đi
    var response = await httpClient.SendAsync(request);

    // Trả kết quả về cho Camera
    context.Response.StatusCode = (int)response.StatusCode;
    await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
});

app.Run();
