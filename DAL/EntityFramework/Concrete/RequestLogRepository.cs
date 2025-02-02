using DAL.EntityFramework.Abstract;
using DAL.EntityFramework.Context;
using DAL.EntityFramework.GenericRepository;
using ENTITIES.Entities;

namespace DAL.EntityFramework.Concrete;

public class RequestLogRepository(DataContext dataContext)
    : GenericRepository<RequestLog>(dataContext), IRequestLogRepository
{
}