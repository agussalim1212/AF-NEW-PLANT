﻿using SkeletonApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface ISubjectRepository
    {
        Task<bool> ValidateData(Subject subject);
    }
}