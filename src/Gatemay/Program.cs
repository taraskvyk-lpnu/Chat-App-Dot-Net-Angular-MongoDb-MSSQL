using Gatemay;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration
    .AddJsonFile("ocelot.chats.json", optional: false, reloadOnChange: true)
    .AddJsonFile("ocelot.chathub.json", optional: false, reloadOnChange: true)
    .AddJsonFile("ocelot.user.management.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);
builder.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseWebSockets();
app.UseHttpsRedirection();
app.UseOcelot().GetAwaiter().GetResult();

app.Run();