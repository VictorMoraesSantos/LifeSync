using BuildingBlocks.Exceptions;
using Core.Domain.Exceptions;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class DailyProgressService : IDailyProgressService
    {
        private readonly IDailyProgressRepository _dailyProgressRepository;

        public DailyProgressService(IDailyProgressRepository dailyProgressRepository)
        {
            _dailyProgressRepository = dailyProgressRepository;
        }

        public async Task<int> CountAsync(Expression<Func<DailyProgressDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var all = await _dailyProgressRepository.GetAll(cancellationToken);
            return all.Count();
        }

        public async Task<bool> CreateAsync(CreateDailyProgressDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            var entity = DailyProgressMapper.ToEntity(dto);
            await _dailyProgressRepository.Create(entity, cancellationToken);
            return true;
        }

        public async Task<bool> CreateRangeAsync(IEnumerable<CreateDailyProgressDTO> dtos, CancellationToken cancellationToken = default)
        {
            if (dtos == null || !dtos.Any()) return false;

            var entities = dtos.Select(DailyProgressMapper.ToEntity);
            await _dailyProgressRepository.CreateRange(entities, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
            if (entity == null) return false;

            await _dailyProgressRepository.Delete(entity, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || !ids.Any()) return false;

            var entities = new List<DailyProgress>();
            foreach (var id in ids)
            {
                var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
                if (entity != null)
                    entities.Add(entity);
            }

            if (!entities.Any()) return false;

            foreach (var entity in entities)
                await _dailyProgressRepository.Delete(entity, cancellationToken);

            return true;
        }

        public async Task<IEnumerable<DailyProgressDTO>> FindAsync(Expression<Func<DailyProgressDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // Para simplificar, não implementa filtro por predicate no momento
            var entities = await _dailyProgressRepository.GetAll(cancellationToken);
            var dtos = entities.Select(DailyProgressMapper.ToDTO);
            return dtos;
        }

        public async Task<IEnumerable<DailyProgressDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _dailyProgressRepository.GetAll(cancellationToken);
            var dtos = entities.Select(DailyProgressMapper.ToDTO);
            return dtos;
        }

        public async Task<DailyProgressDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
            if (entity == null) return null;

            var dto = DailyProgressMapper.ToDTO(entity);
            return dto;
        }

        public async Task<IEnumerable<DailyProgressDTO>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var entities = await _dailyProgressRepository.GetAllByUserId(userId, cancellationToken);
            var dtos = entities.Select(DailyProgressMapper.ToDTO);
            return dtos;
        }

        public async Task<(IEnumerable<DailyProgressDTO> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var all = await _dailyProgressRepository.GetAll(cancellationToken);
            int totalCount = all.Count();

            var items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(DailyProgressMapper.ToDTO);

            return (items, totalCount);
        }

        public async Task SetConsumedAsync(int id, int calories, int liquidsMl, CancellationToken cancellationToken)
        {
            var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"DailyProgress with id {id} not found.");

            entity.SetConsumed(calories, liquidsMl);
            await _dailyProgressRepository.Update(entity, cancellationToken);
        }

        public async Task SetGoalAsync(int id, DailyGoalDTO goalDto, CancellationToken cancellationToken)
        {
            var entity = await _dailyProgressRepository.GetById(id, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"DailyProgress with id {id} not found.");

            var goal = DailyGoalMapper.ToEntity(goalDto);
            entity.SetGoal(goal);
            await _dailyProgressRepository.Update(entity, cancellationToken);
        }

        public async Task<bool> UpdateAsync(UpdateDailyProgressDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            var entity = await _dailyProgressRepository.GetById(dto.Id, cancellationToken);
            if (entity == null) return false;

            // Atualize os campos necessários
            entity.SetConsumed(dto.CaloriesConsumed, dto.LiquidsConsumedMl);

            await _dailyProgressRepository.Update(entity, cancellationToken);
            return true;
        }
    }
}