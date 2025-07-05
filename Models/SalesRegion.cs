using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalytics.Models;

[Table("DimRegion")]
public class SalesRegion
{
    [Key]
    [Column("region_id")]
    public int RegionId { get; set; }

    [Column("region_name")]
    public string RegionName { get; set; } = null!;
}
