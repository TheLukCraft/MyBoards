using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyBoardsContext>(
    option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnectionString"))
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();

var pendingMigrations = dbContext.Database.GetPendingMigrations();
if (pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}

var users = dbContext.Users.ToList();
if (!users.Any())
{
    var user1 = new User()
    {
        Email = "user1@test.com",
        Fullname = "user one",
        Address = new Address()
        {
            City = "Warszawa",
            Street = "Szeroka"
        }
    };

    var user2 = new User()
    {
        Email = "user2@test.com",
        Fullname = "user two",
        Address = new Address()
        {
            City = "Kraków",
            Street = "Jakaœ"
        }
    };
    dbContext.Users.AddRange(user1, user2);
    dbContext.SaveChanges();
}
app.MapGet("data", async (MyBoardsContext db) =>
{
    //var tags = db.Tags.ToList();

    //var epic = db.Epics.First();

    //var user = db.Users.First(u => u.Fullname == "User One");

    //var toDoWorkItems = db.WorkItems.Where(x => x.StateId == 1).ToList();

    //var newComments = await db.Comments.Where(c => c.CreatedDate > new DateTime(2022, 7, 23))
    //.ToListAsync();

    //var top5NewestComments = await db.Comments
    //.OrderByDescending(c => c.CreatedDate)
    //.Take(5).
    //ToListAsync();

    //var statesCount = await db.WorkItems
    //.GroupBy(x => x.StateId)
    //.Select(g => new { stateId = g.Key, count = g.Count() })
    //.ToListAsync();

    return new { statesCount };
});
app.Run();