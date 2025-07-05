using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalytics.Models;

[Table("DimCustomer")]
public class SalesCustomer
{
    [Key]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Column("customer_code")]
    public string CustomerCode { get; set; } = null!;

    [Column("customer_name")]
    public string? CustomerName { get; set; }
}
