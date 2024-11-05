using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using TestMongoEfCore.Entity;

namespace TestMongoEfCore.DbModel;

public class OrderLineDbModel
{
    public required ObjectId Id { get; set; }

    [MaxLength(OrderLine.ProductMaxLength)]
    public required string Product { get; set; }

    public required int Quantity { get; set; }
}