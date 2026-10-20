using DAL_Celebrity;
// Связывает generic-интерфейс с конкретными типами
namespace DAL_Celebrity_MSSQL
{
    public interface IRepository : DAL_Celebrity.IRepository<Celebrity, Lifeevent> { }
}