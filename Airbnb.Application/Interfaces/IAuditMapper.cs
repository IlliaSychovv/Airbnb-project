namespace Airbnb.Application.Interfaces;

public interface IAuditMapper<TEntity, TEvent>
{
    TEvent Map(TEntity entity);
}