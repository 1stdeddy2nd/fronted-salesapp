using SalesAnalytics.Models;

namespace SalesAnalytics.Dto;

public class SalesResponseDto
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public List<Sales> Data { get; set; } = new();
}
