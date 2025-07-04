using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.Guids;
using YayZent.Framework.Auth.Domain.Entities;
using YayZent.Framework.Auth.Domain.Shared.Etos;
using YayZent.Framework.SqlSugarCore.Abstractions;

namespace YayZent.Framework.Auth.Application.EventHandlers;

public class RefreshTokenCreatedEventHandler: ILocalEventHandler<RefreshTokenCreatedEventArgs>, ITransientDependency
{
    private IGuidGenerator _guidGenerator;
    private readonly ISqlSugarRepository<RefreshTokenAggregateRoot, Guid> _refreshTokenRepository;

    public RefreshTokenCreatedEventHandler(IGuidGenerator guidGenerator,ISqlSugarRepository<RefreshTokenAggregateRoot, Guid> refreshTokenRepository)
    {
        _guidGenerator = guidGenerator;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task HandleEventAsync(RefreshTokenCreatedEventArgs eventData)
    {
        var entity = new RefreshTokenAggregateRoot(_guidGenerator.Create(), eventData.UserId, eventData.Token,
            eventData.ExpireTime);

        await _refreshTokenRepository.InsertAsync(entity);
    }
}