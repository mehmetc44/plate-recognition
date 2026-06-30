using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlakaTanima.Application.DTOs.Lpr;

namespace PlakaTanima.Application.Services
{
    public interface ILprService
    {
        void EnqueueLprEventProcessing(LprEventWebhookDto dto);
        Task<QueryEventsResponseDto> QueryEventsAsync(
            string? search,
            string? categories,
            string? direction,
            DateTime? startDate,
            DateTime? endDate,
            int page,
            int pageSize);
        Task<List<AnprEventHistoryDto>> GetPlateHistoryAsync(string plate);
    }
}
