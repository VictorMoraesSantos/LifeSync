using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.DTOs.Meals;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class MealFoodService : IMealFoodService
    {
        private readonly IMealFoodRepository _mealFoodRepository;

        public MealFoodService(IMealFoodRepository mealFoodRepository)
        {
            _mealFoodRepository = mealFoodRepository;
        }

        public async Task<int> CountAsync(Expression<Func<MealFoodDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var all = await _mealFoodRepository.GetAll(cancellationToken);
            return all.Count();
        }

        public async Task<bool> CreateAsync(CreateMealFoodDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            MealFood entity = MealFoodMapper.ToEntity(dto);

            await _mealFoodRepository.Create(entity, cancellationToken);
            return true;
        }

        public async Task<bool> CreateRangeAsync(IEnumerable<CreateMealFoodDTO> dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            IEnumerable<MealFood> entities = dto.Select(MealFoodMapper.ToEntity);

            await _mealFoodRepository.CreateRange(entities, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(int dto, CancellationToken cancellationToken = default)
        {
            MealFood? entity = await _mealFoodRepository.GetById(dto, cancellationToken);
            if (entity == null) return false;

            await _mealFoodRepository.Delete(entity, cancellationToken);

            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || !ids.Any()) return false;

            List<MealFood> entities = new();
            foreach (var id in ids)
            {
                MealFood? entity = await _mealFoodRepository.GetById(id, cancellationToken);
                if (entity != null)
                    entities.Add(entity);
            }

            if (!entities.Any()) return false;

            foreach (var entity in entities)
                await _mealFoodRepository.Delete(entity, cancellationToken);

            return true;
        }

        public async Task<IEnumerable<MealFoodDTO>> FindAsync(Expression<Func<MealFoodDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // Para simplificar, não implementa filtro por predicate no momento
            IEnumerable<MealFood?> entities = await _mealFoodRepository.GetAll(cancellationToken);
            IEnumerable<MealFoodDTO?> dtos = entities.Select(MealFoodMapper.ToDTO!);
            return dtos;
        }

        public async Task<IEnumerable<MealFoodDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<MealFood?> entities = await _mealFoodRepository.GetAll(cancellationToken);
            IEnumerable<MealFoodDTO?> dtos = entities.Select(MealFoodMapper.ToDTO!);
            return dtos;
        }

        public async Task<MealFoodDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            MealFood? entity = await _mealFoodRepository.GetById(id, cancellationToken);
            if (entity == null) return null;

            MealFoodDTO dto = MealFoodMapper.ToDTO(entity);
            return dto;
        }

        public async Task<(IEnumerable<MealFoodDTO> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            IEnumerable<MealFood?> all = await _mealFoodRepository.GetAll(cancellationToken);
            int totalCount = all.Count();

            IEnumerable<MealFoodDTO?> items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MealFoodMapper.ToDTO!);

            return (items, totalCount);
        }

        public async Task<bool> UpdateAsync(UpdateMealFoodDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            MealFood? entity = await _mealFoodRepository.GetById(dto.Id, cancellationToken);
            if (entity == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                entity.SetName(dto.Name);

            if (dto.QuantityInGrams > 0)
                entity.SetQuantity(dto.QuantityInGrams);

            if (dto.CaloriesPerUnit > 0)
                entity.SetCaloriesPerUnit(dto.CaloriesPerUnit);

            // Atualizar refeições e líquidos pode ser complexo e depende da regra de negócio
            // Aqui você pode implementar lógica para atualizar as coleções conforme necessário

            await _mealFoodRepository.Update(entity, cancellationToken);
            return true;
        }
    }
}
