using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SalesAnalytics.Data;
using SalesAnalytics.Dto;
using SalesAnalytics.Models;

namespace SalesAnalytics.Controllers;

[ApiController]
[Route("api/sales")]
public class SalesController : ControllerBase
{
    private readonly SalesDbContext _context;

    public SalesController(SalesDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Sales.Admin")]
    [HttpGet]
    public async Task<ActionResult<SalesResponseDto>> GetSales(
        [FromQuery] string? customerId,
        [FromQuery] string? product,
        [FromQuery] string? region,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var parameters = new[]
        {
            new SqlParameter("@CustomerId", customerId ?? (object)DBNull.Value),
            new SqlParameter("@Product", product ?? (object)DBNull.Value),
            new SqlParameter("@Region", region ?? (object)DBNull.Value),
            new SqlParameter("@StartDate", startDate ?? (object)DBNull.Value),
            new SqlParameter("@EndDate", endDate ?? (object)DBNull.Value),
            new SqlParameter("@PageNumber", pageNumber),
            new SqlParameter("@PageSize", pageSize)
        };

        // Execute usp_GetSalesData
        var data = await _context.Sales
            .FromSqlRaw("EXEC dbo.usp_GetSalesData @CustomerId, @Product, @Region, @StartDate, @EndDate, @PageNumber, @PageSize", parameters)
            .ToListAsync();

        // Execute usp_GetSalesSummary (reuse params, skip page)
        var summary = _context.SalesSummaries
            .FromSqlRaw("EXEC dbo.usp_GetSalesSummary @CustomerId, @Product, @Region, @StartDate, @EndDate", parameters.Take(5).ToArray())
            .AsEnumerable()
            .FirstOrDefault();

        var response = new SalesResponseDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = summary?.TotalRecords ?? 0,
            TotalPages = (int)Math.Ceiling((summary?.TotalRecords ?? 0) / (double)pageSize),
            Data = data
        };

        return Ok(response);
    }

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<SalesProduct>>> GetProducts()
    {
        var products = await _context.DimProducts.ToListAsync();
        return Ok(products);
    }

    [HttpGet("regions")]
    public async Task<ActionResult<IEnumerable<SalesRegion>>> GetRegions()
    {
        var regions = await _context.DimRegions.ToListAsync();
        return Ok(regions);
    }

    [HttpGet("customers")]
    public async Task<ActionResult<IEnumerable<SalesCustomer>>> GetCustomers()
    {
        var customers = await _context.DimCustomers.ToListAsync();
        return Ok(customers);
    }

}
