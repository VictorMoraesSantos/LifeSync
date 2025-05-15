using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class MealService : IMealService
    {
        private readonly IMealRepository _mealRepository;

        public MealService(IMealRepository mealRepository)
        {
            _mealRepository = mealRepository;
        }

        public async Task<int> CountAsync(Expression<Func<MealDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var all = await _mealRepository.GetAll(cancellationToken);
            return all.Count();
        }

        public async Task<bool> CreateAsync(CreateMealDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            Meal entity = MealMapper.ToEntity(dto);

            await _mealRepository.Create(entity, cancellationToken);
            return true;
        }

        public async Task<bool> CreateRangeAsync(IEnumerable<CreateMealDTO> dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            IEnumerable<Meal> entities = dto.Select(MealMapper.ToEntity);

            await _mealRepository.CreateRange(entities, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int dto, CancellationToken cancellationToken = default)
        {
            Meal? entity = await _mealRepository.GetById(dto, cancellationToken);
            if (entity == null) return false;

            await _mealRepository.Delete(entity, cancellationToken);

            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || !ids.Any()) return false;

            List<Meal> entities = new();
            foreach (var id in ids)
            {
                Meal? entity = await _mealRepository.GetById(id, cancellationToken);
                if (entity != null)
                    entities.Add(entity);
            }

            if (!entities.Any()) return false;

            foreach (var entity in entities)
                await _mealRepository.Delete(entity, cancellationToken);

            return true;
        }

        public async Task<IEnumerable<MealDTO?>> FindAsync(Expression<Func<MealDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // Para simplificar, não implementa filtro por predicate no momento
            IEnumerable<Meal?> entities = await _mealRepository.GetAll(cancellationToken);
            IEnumerable<MealDTO?> dtos = entities.Select(MealMapper.ToDTO!);
            return dtos;
        }

        public async Task<IEnumerable<MealDTO?>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Meal?> entities = await _mealRepository.GetAll(cancellationToken);
            IEnumerable<MealDTO?> dtos = entities.Select(MealMapper.ToDTO!);
            return dtos;
        }

        public async Task<MealDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Meal? entity = await _mealRepository.GetById(id, cancellationToken);
            if (entity == null) return null;

            MealDTO dto = MealMapper.ToDTO(entity);
            return dto;
        }

        public async Task<(IEnumerable<MealDTO?> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            IEnumerable<Meal?> all = await _mealRepository.GetAll(cancellationToken);
            int totalCount = all.Count();

            IEnumerable<MealDTO?> items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MealMapper.ToDTO!);

            return (items, totalCount);
        }

        public async Task<bool> UpdateAsync(UpdateMealDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            Meal? entity = await _mealRepository.GetById(dto.Id, cancellationToken);
            if (entity == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                entity.UpdateName(dto.Name);

            if (!string.IsNullOrWhiteSpace(dto.Description))
                entity.UpdateDescription(dto.Description);

            // Atualizar refeições e líquidos pode ser complexo e depende da regra de negócio
            // Aqui você pode implementar lógica para atualizar as coleções conforme necessário

            await _mealRepository.Update(entity, cancellationToken);
            return true;
        }
    }
}
