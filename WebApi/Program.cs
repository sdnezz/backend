using System.Text.Json;
using Dapper;
using FluentValidation;
using WebApi.BLL.Services;
using WebApi.DAL;
using WebApi.DAL.Interfaces;
using WebApi.DAL.Repositories;
using WebApi.Validators;
using WebApi.Config;
using WebApi.Jobs;

var builder = WebApplication.CreateBuilder(args);

DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.Services.AddScoped<UnitOfWork>();

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection(nameof(DbSettings)));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
// зависимость, которая автоматически подхватывает все контроллеры в проекте

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));
builder.Services.AddScoped<ValidatorFactory>();
builder.Services.AddScoped<RabbitMqService>();
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<IAuditLogOrderRepository, AuditLogOrderRepository>();

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options => 
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});
// добавляем swagger
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<OrderGenerator>();

// собираем билдер в приложение
builder.Services.AddOpenApi();

var app = builder.Build();

// добавляем 2 миддлвари для обработки запросов в сваггер
app.UseSwagger();
app.UseSwaggerUI();

// добавляем миддлварю для роутинга в нужный контроллер
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
Migrations.Program.Main([]); 
app.Run();

//docker-compose exec postgres psql -U user -d postgres -c "\dt" - просмотр таблиц