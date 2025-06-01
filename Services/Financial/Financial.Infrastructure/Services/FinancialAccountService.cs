using Financial.Application.Contracts;
using Financial.Application.DTOs.FinancialAccount;
using Financial.Application.Mappings;
using Financial.Domain.Entities;
using Financial.Domain.Repositories;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Services
{
    public class FinancialAccountService : IFinancialAccountService
    {
        private readonly IFinancialAccountRepository _financialAccountRepository;

        public FinancialAccountService(IFinancialAccountRepository financialAccountRepository)
        {
            _financialAccountRepository = financialAccountRepository;
        }

        public async Task<int> CountAsync(Expression<Func<FinancialAccountDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var all = await _financialAccountRepository.GetAll(cancellationToken);
            return all.Count();
        }

        public async Task<int> CreateAsync(CreateFinancialAccountDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = FinancialMapper.ToEntity(dto);

            await _financialAccountRepository.Create(entity, cancellationToken);

            return entity.Id;
        }

        public async Task<IEnumerable<int>> CreateRangeAsync(IEnumerable<CreateFinancialAccountDTO> dto, CancellationToken cancellationToken = default)
        {
            if (dto == null || !dto.Any()) throw new ArgumentNullException(nameof(dto));
            var entities = dto.Select(FinancialMapper.ToEntity).ToList();
            await _financialAccountRepository.CreateRange(entities, cancellationToken);
            return entities.Select(e => e.Id);
        }

        public async Task<bool> DeleteAsync(int dto, CancellationToken cancellationToken = default)
        {
            if (dto <= 0) throw new ArgumentOutOfRangeException(nameof(dto), "ID must be greater than zero.");
            var entity = await _financialAccountRepository.GetById(dto, cancellationToken);
            if (entity == null) return false;
            await _financialAccountRepository.Delete(entity, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<int> dtos, CancellationToken cancellationToken = default)
        {
            if (dtos == null || !dtos.Any()) return false;
            List<FinancialAccount> entities = new();
            foreach (var id in dtos)
            {
                FinancialAccount? entity = await _financialAccountRepository.GetById(id, cancellationToken);
                if (entity != null)
                    entities.Add(entity);
            }
            if (!entities.Any()) return false;
            foreach (var entity in entities)
                await _financialAccountRepository.Delete(entity, cancellationToken);
            return true;
        }

        public async Task<IEnumerable<FinancialAccountDTO?>> FindAsync(Expression<Func<FinancialAccountDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var all = await _financialAccountRepository.GetAll(cancellationToken);

            var allDtos = all.Select(FinancialMapper.ToDTO).ToList();

            var filtered = allDtos.AsQueryable().Where(predicate).ToList();

            return filtered;
        }

        public async Task<IEnumerable<FinancialAccountDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var all = await _financialAccountRepository.GetAll(cancellationToken);
            var allDtos = all.Select(FinancialMapper.ToDTO).ToList();
            return allDtos;
        }

        public async Task<FinancialAccountDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");

            var entity = await _financialAccountRepository.GetById(id, cancellationToken);
            if (entity == null) return null;

            var dto = FinancialMapper.ToDTO(entity);
            return dto;
        }

        public async Task<(IEnumerable<FinancialAccountDTO> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than zero.");
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            var all = await _financialAccountRepository.GetAll(cancellationToken);
            var allDtos = all.Select(FinancialMapper.ToDTO).ToList();
            int totalCount = allDtos.Count;
            var pagedItems = allDtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return (pagedItems, totalCount);
        }

        public async Task<bool> UpdateAsync(UpdateFinancialAccountDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Id <= 0) throw new ArgumentOutOfRangeException(nameof(dto.Id), "ID must be greater than zero.");

            var entity = await _financialAccountRepository.GetById(dto.Id, cancellationToken);
            if (entity == null) return false;

            entity.UpdateDetails(dto.Name, dto.AccountType);

            await _financialAccountRepository.Update(entity, cancellationToken);

            return true;
        }
    }
}
