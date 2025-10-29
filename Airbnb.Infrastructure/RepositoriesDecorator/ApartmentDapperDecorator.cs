using Airbnb.Application.DTO.Dappers;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Repositories;
using Contracts.MonolithEvents;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Airbnb.Infrastructure.RepositoriesDecorator;

public class ApartmentDapperDecorator : IApartmentDapperRepository
{
    private readonly IApartmentDapperRepository _repository;
    private readonly IEventSender _eventSender;

    public ApartmentDapperDecorator(IApartmentDapperRepository repository, IEventSender eventSender)
    {
        _repository = repository;
        _eventSender = eventSender;
    }

    public async Task UpsertAsync(ApartmentUpsertDto dto)
    {
        await _repository.UpsertAsync(dto);

        var eventDto = new ApartmentUpdatedEvent
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Location = dto.Location,
            ExternalId = dto.ExternalId, 
            Metadata = JsonSerializer.Serialize(dto.Metadata) 
        };
        
        await _eventSender.SendEvent(dto.Id.ToString(), eventDto);
    }
    
    public async Task<IEnumerable<GroupByResultDto>> GetGroupByResultAsync() => await _repository.GetGroupByResultAsync();
    public async Task<IEnumerable<HavingResultDto>> GetHavingResultsAsync() => await _repository.GetHavingResultsAsync();
    public async Task<AggregateStatsDto> GetStatisticsAsync() => await _repository.GetStatisticsAsync();
    public async Task<QuantilesDto> GetPriceQuantilesAsync() => await _repository.GetPriceQuantilesAsync();
}