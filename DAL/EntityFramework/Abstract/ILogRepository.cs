using DAL.EntityFramework.GenericRepository;
using DAL.EntityFramework.Utility;
using DTO;
using ENTITIES.Entities;

namespace DAL.EntityFramework.Abstract;

public interface ILogRepository : IGenericRepository<Log>
{
}