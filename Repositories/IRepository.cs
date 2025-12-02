namespace NominaCaribe.Repositories
{
    public interface IRepository<T> where T : class
    {
        void Agregar(T entidad);
        T ObtenerPorId(int id);
        List<T> ObtenerTodos();
        void Actualizar(T entidad);
        void Eliminar(int id);
    }
}