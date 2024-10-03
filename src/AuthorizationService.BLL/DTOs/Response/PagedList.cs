using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.BLL.DTOs.Response;

public class PagedList<T>
{
    private PagedList(List<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public List<T> Items { get; }

    public int Page { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public bool HasNextPage => Page * PageSize < TotalCount;

    public bool HasPreviousPage => Page > 1;

    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 0)
        {
            throw new ArgumentException("Cannot be negative", nameof(page));
        }

        if (pageSize < 0)
        {
            throw new ArgumentException("Cannot be negative", nameof(pageSize));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedList<T>(items, page, pageSize, totalCount);
    }

    public static async Task<PagedList<T>> CreateAsync(IEnumerable<T> enumerable, int page, int pageSize)
    {
        if (page < 0)
        {
            throw new ArgumentException("Cannot be negative", nameof(page));
        }

        if (pageSize < 0)
        {
            throw new ArgumentException("Cannot be negative", nameof(pageSize));
        }

        var list = enumerable.ToList();
        var totalCount = list.Count;
        var items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return await Task.FromResult(new PagedList<T>(items, page, pageSize, totalCount));
    }
}
