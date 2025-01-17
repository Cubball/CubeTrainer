namespace CubeTrainer.API.Common.Models;

internal sealed record PagedList<T>(
    int Page,
    int PageSize,
    int TotalPages,
    int TotalCount,
    List<T> Items);