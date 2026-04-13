using System.Reflection;
using Microsoft.OpenApi.Models;
using Tidy_BackendTest_MidLevel.Application.Services;
using Tidy_BackendTest_MidLevel.Domain.Services;
using Tidy_BackendTest_MidLevel.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure (DbContext + Repository)
builder.Services.AddInfrastructure(builder.Configuration);

// Application Services
builder.Services.AddScoped<MyOfficeAcpdService>();

// Domain Services
builder.Services.AddSingleton<SidGeneratorService>();

// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyOffice ACPD API",
        Version = "v1",
        Description = """
            MyOffice 人員帳號資料 CRUD Web API

            **端點說明：**
            - `GET /api/myofficeacpd` — 查詢所有人員帳號
            - `GET /api/myofficeacpd/{id}` — 依 SID 查詢單筆
            - `POST /api/myofficeacpd` — 新增人員帳號（SID 自動產生）
            - `PUT /api/myofficeacpd/{id}` — 依 SID 更新人員帳號
            - `DELETE /api/myofficeacpd/{id}` — 依 SID 刪除人員帳號
            """,
        Contact = new OpenApiContact
        {
            Name = "Tidy"
        }
    });

    // Include XML comments for Swagger descriptions
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Swagger is always enabled (not just in Development) for test visibility
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyOffice ACPD API v1");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Default redirect: root → swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();
