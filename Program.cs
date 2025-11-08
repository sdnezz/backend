using System.Text.Json;
using Dapper;
using FluentValidation;
using SolutionLab1.BLL.Services;
using SolutionLab1.DAL;
using SolutionLab1.DAL.Interfaces;
using SolutionLab1.DAL.Repositories;
using SolutionLab1.Validators;
using SolutionLab1.Config;

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

builder.Services.AddControllers();
// добавляем swagger
builder.Services.AddSwaggerGen();

// собираем билдер в приложение
var app = builder.Build();

// добавляем 2 миддлвари для обработки запросов в сваггер
app.UseSwagger();
app.UseSwaggerUI();

// добавляем миддлварю для роутинга в нужный контроллер
app.MapControllers();

// вместо *** должен быть путь к проекту Migrations
// по сути в этот момент будет происходить накатка миграций на базу
Migrations.Program.Main([]); 

// запускам приложение
app.Run();



//docker-compose exec postgres psql -U user -d postgres -c "\dt" - просмотр таблиц