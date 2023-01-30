namespace Domain.DTOs;

public record PaginatedResultDto(int TotalPages, int PageIndex, int PageSize, long Count, dynamic Itens)
{
    public bool HasPrevious => PageIndex > 0;

    public bool HasNext => PageIndex < TotalPages;
}