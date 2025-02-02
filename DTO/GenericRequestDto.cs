using ENTITIES.Enums;

namespace DTO;

public record GenericRequestDto(
    PaginationDto? Pagination,
    SortDto? Sort,
    IList<FilterDto>? Filters
);

public record FilterDto(
    string FieldName,
    bool IsInRange,
    bool IsOnlyEquals,
    string? Value,
    string? StartValue,
    string? EndValue
);

public record SortDto(
    string FieldName,
    ESortType Type
);

public record PaginationDto(
    int PageSize,
    int PageIndex
);