using Nutrition.Application.DTOs.Liquid;
using Nutrition.Application.DTOs.Meals;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Data;
using Nutrition.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class LiquidService : ILiquidService
    {
        private readonly ILiquidRepository _liquidRepository;

        public LiquidService(ILiquidRepository liquidRepository)
        {
            _liquidRepository = liquidRepository;
        }

        public async Task<int> CountAsync(Expression<Func<LiquidDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var all = await _liquidRepository.GetAll(cancellationToken);
            return all.Count();
        }

        public async Task<bool> CreateAsync(CreateLiquidDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            Liquid entity = LiquidMapper.ToEntity(dto);

            await _liquidRepository.Create(entity, cancellationToken);
            return true;
        }

        public async Task<bool> CreateRangeAsync(IEnumerable<CreateLiquidDTO> dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            IEnumerable<Liquid> entities = dto.Select(LiquidMapper.ToEntity);

            await _liquidRepository.CreateRange(entities, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int dto, CancellationToken cancellationToken = default)
        {
            Liquid? entity = await _liquidRepository.GetById(dto, cancellationToken);
            if (entity == null) return false;

            await _liquidRepository.Delete(entity, cancellationToken);

            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<int> dtos, CancellationToken cancellationToken = default)
        {
            if (dtos == null || !dtos.Any()) return false;
            List<Liquid> entities = new();
            foreach (var id in dtos)
            {
                Liquid? entity = await _liquidRepository.GetById(id, cancellationToken);
                if (entity != null)
                    entities.Add(entity);
            }
            if (!entities.Any()) return false;
            foreach (var entity in entities)
                await _liquidRepository.Delete(entity, cancellationToken);
            return true;
        }

        public async Task<IEnumerable<LiquidDTO>> FindAsync(Expression<Func<LiquidDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // Para simplificar, não implementa filtro por predicate no momento
            IEnumerable<Liquid?> entities = await _liquidRepository.GetAll(cancellationToken);
            IEnumerable<LiquidDTO?> dtos = entities.Select(LiquidMapper.ToDTO!);
            return dtos;
        }

        public async Task<IEnumerable<LiquidDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Liquid?> entities = await _liquidRepository.GetAll(cancellationToken);
            IEnumerable<LiquidDTO?> dtos = entities.Select(LiquidMapper.ToDTO!);
            return dtos;
        }

        public async Task<LiquidDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Liquid? entity = await _liquidRepository.GetById(id, cancellationToken);
            if (entity == null) return null;

            LiquidDTO dto = LiquidMapper.ToDTO(entity);
            return dto;
        }

        public async Task<(IEnumerable<LiquidDTO?> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            IEnumerable<Liquid?> all = await _liquidRepository.GetAll(cancellationToken);
            int totalCount = all.Count();

            IEnumerable<LiquidDTO?> items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(LiquidMapper.ToDTO!);

            return (items, totalCount);
        }

        public async Task<bool> UpdateAsync(UpdateLiquidDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            Liquid? entity = await _liquidRepository.GetById(dto.Id, cancellationToken);
            if (entity == null) return false;

            if (dto.Name != null)
                entity.SetName(dto.Name);
            if (dto.QuantityMl != null)
                entity.SetQuantityMl(dto.QuantityMl);
            if (dto.CaloriesPerMl != null)
                entity.SetCaloriesPerMl(dto.CaloriesPerMl);

            await _liquidRepository.Update(entity, cancellationToken);
            return true;
        }
    }
}
