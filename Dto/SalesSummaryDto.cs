using Microsoft.EntityFrameworkCore;

namespace SalesAnalytics.Dto;

[Keyless]
public class SalesSummaryDto
{
    public int TotalRecords { get; set; }
}
