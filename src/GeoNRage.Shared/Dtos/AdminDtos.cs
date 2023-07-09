namespace GeoNRage.Shared.Dtos.Admin;

public record TableInfoDto(string Name, int RowCount);

public record AdminInfoDto(IEnumerable<TableInfoDto> Tables);

public record UserAminViewDto(string UserName, string? PlayerId, string? PlayerName, IEnumerable<string> Roles);
