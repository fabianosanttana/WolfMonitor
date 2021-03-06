﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Totten.Solutions.WolfMonitor.Domain.Base;
using Totten.Solutions.WolfMonitor.Infra.CrossCutting.Structs;

namespace Totten.Solutions.WolfMonitor.Domain.Features.UsersAggregation
{
    public interface IUserRepository : IRepository<User>
    {
        Task<Result<Exception, User>> GetByCredentials(Guid companyId, string login, string password);
        Result<Exception, IQueryable<User>> GetAll(Guid companyId);
        Result<Exception, IQueryable<User>> GetAllByCompanyId(Guid companyId);
    }
}
