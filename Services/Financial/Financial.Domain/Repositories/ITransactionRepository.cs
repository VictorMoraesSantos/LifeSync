﻿using Core.Domain.Repositories;
using Financial.Domain.Entities;

namespace Financial.Domain.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction, int>
    {
        Task<IEnumerable<Transaction?>> GetByUserIdAsync(int userId, DateTime startDate, DateTime endDate, int? categoryId, Enums.TransactionType? type);
    }
}
