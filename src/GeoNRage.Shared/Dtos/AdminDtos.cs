namespace GeoNRage.Shared.Dtos.Admin;

public record TableInfoDto(string Name, int RowCount);

public record LogEntryDto(string Message, string Level, DateTime Timestamp, string Exception);

public record AdminInfoDto(IEnumerable<TableInfoDto> Tables, IEnumerable<LogEntryDto> Logs);
