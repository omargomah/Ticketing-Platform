namespace Domain.Interfaces
{
    public interface IDataSeeding<TEntity>
    {
        List<TEntity> DataSeeding();
    }
}
