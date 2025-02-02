using DAL.EntityFramework.Abstract;
using DAL.EntityFramework.Context;
using DAL.EntityFramework.GenericRepository;
using DAL.EntityFramework.Utility;
using DTO;
using ENTITIES.Entities;
using ENTITIES.Enums;

namespace DAL.EntityFramework.Concrete;

public class LogRepository(DataContext dataContext)
    : GenericRepository<Log>(dataContext), ILogRepository
{
}