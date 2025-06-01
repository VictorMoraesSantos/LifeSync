using BuildingBlocks.Exceptions;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using System.Linq.Expressions;
using BuildingBlocks.CQRS.Publisher;

namespace Nutrition.Infrastructure.Services
{
    public class DiaryService : IDiaryService
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IPublisher _publisher;

        public DiaryService(IDiaryRepository diaryRepository, IPublisher publisher)
        {
            _diaryRepository = diaryRepository;
            _publisher = publisher;
        }

        public async Task<int> CountAsync(Expression<Func<DiaryDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            // Para simplificar, não implementa filtro por predicate no momento
            var all = await _diaryRepository.GetAll(cancellationToken);
            return all.Count();
        }

        public async Task<int> CreateAsync(CreateDiaryDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            Diary? existingDiary = await _diaryRepository.GetByDate(dto.userId, dto.date, cancellationToken);
            if (existingDiary != null) throw new ArgumentNullException(nameof(existingDiary));

            Diary entity = DiaryMapper.ToEntity(dto);

            await _diaryRepository.Create(entity, cancellationToken);
            return entity.Id;
        }

        public async Task<IEnumerable<int>> CreateRangeAsync(IEnumerable<CreateDiaryDTO> dtos, CancellationToken cancellationToken = default)
        {
            if (dtos == null || !dtos.Any()) throw new ArgumentNullException(nameof(dtos));

            IEnumerable<Diary> entities = dtos.Select(DiaryMapper.ToEntity);

            await _diaryRepository.CreateRange(entities, cancellationToken);
            return entities.Select(entity => entity.Id).ToList();
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Diary? entity = await _diaryRepository.GetById(id, cancellationToken);
            if (entity == null) return false;

            await _diaryRepository.Delete(entity, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || !ids.Any()) return false;

            List<Diary> entities = new();
            foreach (var id in ids)
            {
                Diary? entity = await _diaryRepository.GetById(id, cancellationToken);
                if (entity != null)
                    entities.Add(entity);
            }

            if (!entities.Any()) return false;

            foreach (var entity in entities)
                await _diaryRepository.Delete(entity, cancellationToken);

            return true;
        }

        public async Task<IEnumerable<DiaryDTO>> FindAsync(Expression<Func<DiaryDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            // Para simplificar, não implementa filtro por predicate no momento
            IEnumerable<Diary?> entities = await _diaryRepository.GetAll(cancellationToken);
            IEnumerable<DiaryDTO> dtos = entities.Select(DiaryMapper.ToDTO);
            return dtos;
        }

        public async Task<IEnumerable<DiaryDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<Diary?> entities = await _diaryRepository.GetAll(cancellationToken);
            IEnumerable<DiaryDTO> dtos = entities.Select(DiaryMapper.ToDTO);
            return dtos;
        }

        public async Task<IEnumerable<DiaryDTO>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            if (userId <= 0) throw new BadRequestException($"{nameof(userId)} must be positive.");

            IEnumerable<Diary?> entities = await _diaryRepository.GetAllByUserId(userId, cancellationToken);
            IEnumerable<DiaryDTO> dtos = entities.Select(DiaryMapper.ToDTO!);
            return dtos;
        }

        public async Task<DiaryDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Diary? entity = await _diaryRepository.GetById(id, cancellationToken);
            if (entity == null) return null;

            DiaryDTO dto = DiaryMapper.ToDTO(entity);
            return dto;
        }

        public async Task<(IEnumerable<DiaryDTO?> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            IEnumerable<Diary?> all = await _diaryRepository.GetAll(cancellationToken);
            int totalCount = all.Count();

            IEnumerable<DiaryDTO?> items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(DiaryMapper.ToDTO!);

            return (items, totalCount);
        }

        public async Task<bool> UpdateAsync(UpdateDiaryDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) return false;

            Diary? entity = await _diaryRepository.GetById(dto.Id, cancellationToken);
            if (entity == null) return false;

            Diary? existingDiary = await _diaryRepository.GetByDate(entity.UserId, dto.Date, cancellationToken);
            if (existingDiary != null) return false;

            entity.UpdateDate(dto.Date);
            entity.MarkAsUpdated();
            // Atualizar refeições e líquidos pode ser complexo e depende da regra de negócio
            // Aqui você pode implementar lógica para atualizar as coleções conforme necessário

            await _diaryRepository.Update(entity, cancellationToken);
            return true;
        }

        public async Task<bool> AddMealToDiaryAsync(int diaryId, CreateMealDTO dto, CancellationToken cancellationToken = default)
        {
            Diary? diary = await _diaryRepository.GetById(diaryId, cancellationToken);
            if (diary == null) return false;

            Meal meal = MealMapper.ToEntity(dto);
            meal.SetDiaryId(diaryId);
            diary.AddMeal(meal);

            await _diaryRepository.Update(diary, cancellationToken);

            foreach (var domainEvent in diary.DomainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
            diary.ClearDomainEvents();

            return true;
        }
    }
}