using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using TestMongoEfCore.DbModel;

namespace TestMongoEfCore.Entity;

public class OrderLine : IValidatableObject
{
    private OrderLine(ObjectId id, string product, int quantity)
    {
        Id = id;
        Product = product;
        Quantity = quantity;
    }

    public const int ProductMaxLength = 50;

    public ObjectId Id { get; set; }
    public string Product { get; set; }
    public int Quantity { get; set; }

    // This is a Create operation so we generate a new Id
    public static OrderLine Create(string product, int quantity) =>
        new(
            id: ObjectId.GenerateNewId(),
            product: product,
            quantity: quantity
        );

    // This is a Read operation so we populate all the properties with the values from the database
    public static OrderLine FromDatabase(OrderLineDbModel orderLineDbModel) =>
        new(
            id: orderLineDbModel.Id,
            product: orderLineDbModel.Product,
            quantity: orderLineDbModel.Quantity
        );

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResults = new List<ValidationResult>();
        if (Quantity < 1)
        {
            validationResults.Add(new ValidationResult("Quantity must be greater than 0"));
        }

        if (Product.Length > ProductMaxLength)
        {
            validationResults.Add(
                new ValidationResult($"Product name must be less than {ProductMaxLength} characters"));
        }

        return validationResults;
    }
}