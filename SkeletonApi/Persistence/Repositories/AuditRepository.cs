﻿using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Persistence.Repositories
{
    public class AuditRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuditRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddAuditActivity(ActivityUser activity)
        {
            _dbContext.ActivityUsers.Add(activity);
            _dbContext.SaveChanges();
        }
    }
}
