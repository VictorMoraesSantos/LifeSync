﻿using Core.Domain.Repositories;
using TaskManager.Domain.Entities;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Domain.Repositories
{
    public interface ITaskLabelRepository : IRepository<TaskLabel, int>
    {
        public Task<IEnumerable<TaskLabel?>> FindByFilter(TaskLabelFilter filter, CancellationToken cancellationToken = default);
    }
}
