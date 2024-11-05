using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using TestMongoEfCore.DbModel;

namespace TestMongoEfCore.Entity;

public class Order : IValidatableObject
{
    private Order(ObjectId id, DateTime createdAt, IEnumerable<OrderLine> orderLines)
    {
        Id = id;
        CreatedAt = createdAt;
        OrderLines = orderLines.ToList();
    }

    public ObjectId Id { get; set; }
    public DateTime CreatedAt { get; }
    public List<OrderLine> OrderLines { get; }

    // This is a Create operation so we generate a new Id,
    // set the date to now and create an empty list of order lines
    public static Order Create()
    {
        var order = new Order(
            id: ObjectId.GenerateNewId(),
            createdAt: DateTime.Now,
            orderLines: new List<OrderLine>()
        );

        var validationResults = order.Validate(new ValidationContext(order)).ToArray();
        if (validationResults.Length > 0)
        {
            throw new ValidationException(string.Join("; ", validationResults.Select(vr => vr.ErrorMessage)));
        }

        return order;
    }

    // This is a Read operation so we populate all the properties with the values from the database
    public static Order FromDatabase(OrderDbModel orderDbModel) =>
        new(
            id: orderDbModel.Id,
            createdAt: orderDbModel.CreatedAt,
            orderLines: orderDbModel.OrderLines.Select(OrderLine.FromDatabase)
                .ToList()
        );

    // Add a new order line to the order
    // Validate the order line before adding it to the order
    public void AddOrderLine(OrderLine orderLine)
    {
        var validationResults = orderLine.Validate(new ValidationContext(orderLine)).ToArray();
        if (validationResults.Length == 0)
        {
            OrderLines.Add(orderLine);
        }
        else
        {
            throw new ValidationException(string.Join("; ", validationResults.Select(vr => vr.ErrorMessage)));
        }
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResults = new List<ValidationResult>();
        if (CreatedAt.Date != DateTime.Now.Date)
        {
            validationResults.Add(new ValidationResult("Order date must be today"));
        }

        return validationResults;
    }

    public OrderDbModel ToDbModel()
    {
        return new OrderDbModel
        {
            Id = Id,
            CreatedAt = CreatedAt,
            OrderLines = OrderLines.Select(ol => new OrderLineDbModel
            {
                Id = ol.Id,
                Product = ol.Product,
                Quantity = ol.Quantity
            }).ToList()
        };
    }
}