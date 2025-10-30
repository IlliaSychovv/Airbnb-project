namespace Airbnb.Application.Interfaces.Mappers;

public interface IAuditMapper<TEntity, TEvent>
{
    TEvent Map(TEntity entity);
}