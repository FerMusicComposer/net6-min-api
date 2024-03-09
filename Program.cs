using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SixMinAPI.Data;
using SixMinAPI.Repositories;
using SixMinAPI.Dtos;
using SixMinAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlConnectionBuilder = new SqlConnectionStringBuilder
{
    ConnectionString = builder.Configuration.GetConnectionString("SQLDbConnection"),
    UserID = builder.Configuration["UserId"],
    Password = builder.Configuration["Password"],
    TrustServerCertificate = true
};

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(sqlConnectionBuilder.ConnectionString));
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Application Endpoints
app.MapGet("api/v1/commands", async (ICommandRepo repo, IMapper mapper) =>
{
    var commands = await repo.GetAllCommands();
    return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
});

app.MapGet("api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id) =>
{
    var command = await repo.GetCommandById(id);
    if (command != null)
    {
        return Results.Ok(mapper.Map<CommandReadDto>(command));
    }

    return Results.NotFound();
});

app.MapPost("api/v1/commands", async (ICommandRepo repo, IMapper mapper, CommandCreateDto commandCreateDto) =>
{
    var command = mapper.Map<Command>(commandCreateDto);
    await repo.CreateCommand(command);

    return Results.Created($"api/v1/commands/{command.Id}", mapper.Map<CommandReadDto>(command));
});

app.MapPut("api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id, CommandUpdateDto commandUpdateDto) =>
{
    var command = await repo.GetCommandById(id);
    if (command == null)
    {
        return Results.NotFound();
    }
    mapper.Map(commandUpdateDto, command);
    await repo.SaveChanges();

    return Results.Created($"api/v1/commands/{command.Id}", mapper.Map<CommandReadDto>(command));
});

app.MapDelete("api/v1/commands/{id}", async (ICommandRepo repo, int id) =>
{
    var command = await repo.GetCommandById(id);
    if (command == null)
    {
        return Results.NotFound();
    }
    repo.DeleteCommand(command);
    await repo.SaveChanges();
    return Results.Ok($"Command with id {id} deleted");
});

app.Run();

