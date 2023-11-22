using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Features.Users;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataRepository<User> _repository;


        public async Task<bool> ValidateData(User user)
        {
            var x = await _repository.Entities.Where(o => o.UserName.ToLower() == user.UserName.ToLower()).CountAsync();

            if (x > 0)
            {
                return false;
            }
            return true;
        }
       
    }
}

