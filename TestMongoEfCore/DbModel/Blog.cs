using MongoDB.Bson;

namespace TestMongoEfCore.DbModel;

public class Blog
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }
    public string Author { get; set; }

    public ICollection<Post> Posts { get; } = new List<Post>();
}