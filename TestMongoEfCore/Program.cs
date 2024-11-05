using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using TestMongoEfCore;
using TestMongoEfCore.Entity;

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

// Create a new order
var orderId = await CreateOrderAsync(dbContextOptions.Options);

// Let's add a new order line to the order
await AddOrderLineToOrderAsync(orderId, dbContextOptions.Options);

// Let's check if the order was updated
var wereChangesSaved = await WasTheOrderLineSaved(dbContextOptions.Options, orderId);
Console.WriteLine($"Changes {(wereChangesSaved ? "were" : "were not")} saved");
return;

async Task<ObjectId> CreateOrderAsync(DbContextOptions<MyDbContext> contextOptions)
{
    await using var dbContext = new MyDbContext(contextOptions);

    // Create a new order and check that it's valid
    var orderEntity = Order.Create();

    // The order is valid so we can save it
    dbContext.Orders.Add(orderEntity.ToDbModel());
    await dbContext.SaveChangesAsync();
    return orderEntity.Id;
}

// I was hoping that this would work despite adding all the manual lines to detect changes and attach the entity
async Task AddOrderLineToOrderAsync(ObjectId objectId, DbContextOptions<MyDbContext> contextOptions)
{
    await using var dbContext = new MyDbContext(contextOptions);
    dbContext.ChangeTracker.DetectChanges(); // <<<<<<<<<<< Doesn't work despite this line

    // Load the order from the database and add a new order line
    var orderDbModel = await dbContext.Orders
        .AsNoTracking()
        .FirstOrDefaultAsync(o => o.Id == objectId); // <<<<<<<<<<< Doesn't work despite adding AsNoTracking

    // Map to an order entity to add a new order line and apply all the "business rules"
    var orderEntity = orderDbModel!.ToEntity();

    // Add a new orderline to the order.
    // This will validate the order line and add it to the order if it's valid
    // or throw an exception if it's not
    var orderLineEntity = OrderLine.Create("MongoDB T-shirt", 2);
    orderEntity.AddOrderLine(orderLineEntity);

    // Map the order entity back to a db model to save it
    var updatedDbModel = orderEntity.ToDbModel();
    dbContext.Attach(updatedDbModel); // <<<<<<<<<<< Doesn't work despite this line

    var changed = await dbContext.SaveChangesAsync();
    Console.WriteLine($"Changed: {changed}");
}

async Task<bool> WasTheOrderLineSaved(DbContextOptions<MyDbContext> contextOptions, ObjectId orderId1)
{
    await using var dbContext = new MyDbContext(contextOptions);
    // Let's check if the order was updated
    var updatedOrNotUpdatedThatIsTheQuestion = await dbContext.Orders
        .FindAsync(orderId1);

    return updatedOrNotUpdatedThatIsTheQuestion is not null &&
           updatedOrNotUpdatedThatIsTheQuestion.OrderLines.Count == 1;
}