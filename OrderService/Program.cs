using Common.State;
using Messaging;
using OrderService;

var builder = WebApplication.CreateBuilder(args);


var rdOpt = builder.Configuration.GetSection("Redis").Get<RedisOptions>()!;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new RedisStateStore(rdOpt));

builder.Services.SetUpRabbitMq(builder.Configuration);

builder.Services.AddSingleton<RabbitSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
