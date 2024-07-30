namespace Common.Models;

public record PagedResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<T> Data { get; set; }

    public PagedResponse(int pageNumber, int pageSize, int totalRecords, IEnumerable<T> data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling(totalRecords / (decimal)pageSize);
        Data = data;
    }
}
