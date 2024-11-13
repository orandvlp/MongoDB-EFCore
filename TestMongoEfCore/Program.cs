using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using TestMongoEfCore;
using TestMongoEfCore.DbModel;

#region Start MongoDb TestContainer
var mongoDbContainer = new Testcontainers.MongoDb.MongoDbBuilder()
    .WithImage("mongo:latest")
    .WithPortBinding(27017)
    .Build();
await mongoDbContainer.StartAsync();

var connectionString = mongoDbContainer.GetConnectionString();
var mongoDatabase = new MongoClient(connectionString).GetDatabase("test");
var dbContextOptions =
    new DbContextOptionsBuilder<MyDbContext>().UseMongoDB(mongoDatabase.Client,
        mongoDatabase.DatabaseNamespace.DatabaseName);
#endregion

// Create a new blog
var blogId = await CreateBlogAsync(dbContextOptions.Options);

// Let's add a new post to the blog
await AddPostToBlogAsync(blogId, dbContextOptions.Options);

// Let's check if the post was updated
var wereChangesSaved = await WasThePostSaved(dbContextOptions.Options, blogId);
Console.WriteLine($"Changes {(wereChangesSaved ? "were" : "were not")} saved");
return;

async Task<ObjectId> CreateBlogAsync(DbContextOptions<MyDbContext> contextOptions)
{
    await using var dbContext = new MyDbContext(contextOptions);

    var blog = new Blog
    {
        Name = "MongoDB Blog",
        Author = "Oran"
    };

    dbContext.Blogs.Add(blog);
    await dbContext.SaveChangesAsync();
    return blog.Id;
}

async Task AddPostToBlogAsync(ObjectId blogId, DbContextOptions<MyDbContext> contextOptions)
{
    await using var dbContext = new MyDbContext(contextOptions);

    // Load the blog from the database
    var blog = await dbContext.Blogs
        .FindAsync(blogId);

    // Add a new post to the blog
    var post = new Post
    {
        Id = ObjectId.GenerateNewId(),
        Title = "MongoDB Post",
        Content = "This is a post about MongoDB EF Core provider",
        PostedOn = DateTime.Now
    };
    blog!.Posts.Add(post);

    var changed = await dbContext.SaveChangesAsync();
    Console.WriteLine($"Changed: {changed}");
}

async Task<bool> WasThePostSaved(DbContextOptions<MyDbContext> contextOptions, ObjectId blogId)
{
    await using var dbContext = new MyDbContext(contextOptions);
    // Let's check if the post was updated
    var updatedOrNotUpdatedThatIsTheQuestion = await dbContext.Blogs
        .FindAsync(blogId);

    return updatedOrNotUpdatedThatIsTheQuestion is not null &&
           updatedOrNotUpdatedThatIsTheQuestion.Posts.Count == 1;
}