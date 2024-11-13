using MongoDB.Bson;

namespace TestMongoEfCore.DbModel;

public class Post
{
    public ObjectId Id { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PostedOn { get; set; }

    public Blog Blog { get; set; }
}