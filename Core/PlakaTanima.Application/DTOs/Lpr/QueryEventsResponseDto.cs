using System.Collections.Generic;

namespace PlakaTanima.Application.DTOs.Lpr
{
    public class QueryEventsResponseDto
    {
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
        public List<AnprEventDto> Events { get; set; } = new();
    }
}
