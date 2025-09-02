namespace C_Sharp_Web_API.Shared;

public class PaginationMetadata(int totalItemCount, int pageSize, int currentPage)
{
    public int TotalItemCount { get; set; } = totalItemCount;
    public int PageSize { get; set; } = pageSize;
    public int TotalPageCount { get; set; } = (int)Math.Ceiling(totalItemCount / (double)pageSize);
    public int CurrentPage { get; set; } = currentPage;
}