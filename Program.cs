using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

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

    //var selectedEpics = await db.Epics
    //.Where(w => w.StateId == 4)
    //.OrderBy(w => w.Priority)
    //.ToListAsync();

    //var authorsCommentCounts = await db.Comments
    //.GroupBy(c => c.AuthorId)
    //.Select(g => new { g.Key, Count = g.Count() })
    //.ToListAsync();

    //var topAuthor = authorsCommentCounts
    //.First(a => a.Count == authorsCommentCounts.Max(acc => acc.Count));

    //var userDetails = db.Users.First(u => u.Id == topAuthor.Key);

    //return new { userDetails, commentCount = topAuthor.Count };

    //var user = await db.Users.FirstAsync(u => u.Id == Guid.Parse("68366DBE-0809-490F-CC1D-08DA10AB0E61"));
    //var userComments = await db.Comments.Where(c => c.AuthorId == user.Id).ToListAsync();

    //return user;

    var user = await db.Users
    .Include(u => u.Comments).ThenInclude(c => c.WorkItem)
    .Include(u => u.Address)
    .FirstAsync(u => u.Id == Guid.Parse("68366DBE-0809-490F-CC1D-08DA10AB0E61"));

    return user;
});

app.MapPost("update", async (MyBoardsContext db) =>
{
    //var epic = await db.Epics.FirstAsync(epic => epic.Id == 1);

    //epic.Area = "Updated Area";
    //epic.Priority = 1;
    //epic.StartDate = DateTime.Now;

    //await db.SaveChangesAsync();
    //return epic;

    //var epic = await db.Epics.FirstAsync(epic => epic.Id == 1);
    //var onHoldState = await db.WorkItemStates.FirstAsync(a => a.Value == "On Hold");

    //epic.StateId = onHoldState.Id;

    //await db.SaveChangesAsync();
    //return epic;

    var epic = await db.Epics.FirstAsync(epic => epic.Id == 1);
    var rejectedState = await db.WorkItemStates.FirstAsync(a => a.Value == "Rejected");

    epic.State = rejectedState;
    await db.SaveChangesAsync();
    return epic;
});

app.MapPost("create", async (MyBoardsContext db) =>
    {
        //Tag tag = new Tag()
        //{
        //    Value = "EF"
        //};
        ////await db.AddAsync(tag);
        //await db.Tags.AddAsync(tag);
        //await db.SaveChangesAsync();
        //return tag;

        //Tag tag = new Tag()
        //{
        //    Value = "EF"
        //};
        //Tag tagAsp = new Tag()
        //{
        //    Value = "ASP"
        //};
        //var tags = new List<Tag>() { tag, tagAsp };
        //await db.Tags.AddRangeAsync(tags);
        //await db.SaveChangesAsync();

        //return tags;

        var address = new Address()
        {
            Id = Guid.NewGuid(),
            City = "Kraków",
            Country = "Poland",
            Street = "D³uga"
        };
        var user = new User()
        {
            Email = "luk1997r@o2.pl",
            Fullname = "Test User",
            Address = address,
        };
        await db.AddAsync(user);
        await db.SaveChangesAsync();

        return user;
    });
app.Run();