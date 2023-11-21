using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;


namespace SkeletonApi.Persistence.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly IGenericRepository<Subject> _repository;

        public async Task<bool> ValidateData(Subject subject)
        {
            var x = await _repository.Entities.Where(o => o.Vid == subject.Vid && o.Subjects.ToLower() == subject.Subjects.ToLower()).CountAsync();

            if (x > 0)
            {
                return false;
            }
            return true;
        }
    }
}