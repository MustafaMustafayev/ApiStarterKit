﻿using DAL.EntityFramework.Abstract;
using DAL.EntityFramework.Context;
using DAL.EntityFramework.GenericRepository;
using ENTITIES.Entities;

namespace DAL.EntityFramework.Concrete;

public class ResponseLogRepository(DataContext dataContext)
    : GenericRepository<ResponseLog>(dataContext), IResponseLogRepository
{
}