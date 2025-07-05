using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalytics.Models;

[Table("Sales")]
public class Sales
{
    [Key]
    [Column("order_id")]
    public string? OrderId { get; set; }

    [Column("customer_id")]
    public string? Customer { get; set; }

    [Column("product")]
    public string? Product { get; set; }

    [Column("region")]
    public string? Region { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("timestamp")]
    public DateTime Timestamp { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
