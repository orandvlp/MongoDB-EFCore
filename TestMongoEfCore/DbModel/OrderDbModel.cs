using MongoDB.Bson;
using TestMongoEfCore.Entity;

namespace TestMongoEfCore.DbModel;

public class OrderDbModel
{
    public ObjectId Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderLineDbModel> OrderLines { get; set; } = [];

    public Order ToEntity() => Order.FromDatabase(this);
}