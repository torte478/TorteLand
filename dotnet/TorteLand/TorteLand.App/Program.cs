using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TorteLand.Core;
using TorteLand.Firebase;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddCors(
           _ => _.AddPolicy(
               "AllowOrigin",
               options => options
                          .AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()))
       .AddFirebase(builder.Configuration)
       .AddCoreLogic();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowOrigin");

app.MapControllers();

app.Run();