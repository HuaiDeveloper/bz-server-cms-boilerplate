namespace Shared;

public class BasePaginationResponse<T>
{
    public T? Data { get; set; }
    public int TotalCount { get; set; } = default!;
}