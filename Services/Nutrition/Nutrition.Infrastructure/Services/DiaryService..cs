using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.Results;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Services
{
    public class DiaryService : IDiaryService
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IPublisher _publisher;
        private readonly ILogger<DiaryService> _logger;

        public DiaryService(
            IDiaryRepository diaryRepository,
            IPublisher publisher,
            ILogger<DiaryService> logger)
        {
            _diaryRepository = diaryRepository;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task<Result<int>> CountAsync(Expression<Func<DiaryDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _diaryRepository.GetAll(cancellationToken);
                return Result.Success(all.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar diários");
                return Result.Failure<int>(Error.Problem("Diary", "CountError").Description);
            }
        }

        public async Task<Result<int>> CreateAsync(CreateDiaryDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.Failure("General", "NullData").Description);

                Diary? existingDiary = await _diaryRepository.GetByDate(dto.userId, dto.date, cancellationToken);
                if (existingDiary != null)
                    return Result.Failure<int>(Error.Conflict("Diary", "AlreadyExists").Description);

                Diary entity = DiaryMapper.ToEntity(dto);
                await _diaryRepository.Create(entity, cancellationToken);

                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar diário {@DiaryData}", dto);
                return Result.Failure<int>(Error.Problem("Diary", "CreateError").Description);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateDiaryDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null)
                    return Result.Failure<IEnumerable<int>>(Error.Failure("General", "NullData").Description);

                if (!dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("General", "EmptyList").Description);

                IEnumerable<Diary> entities = dtos.Select(DiaryMapper.ToEntity);
                await _diaryRepository.CreateRange(entities, cancellationToken);

                return Result.Success<IEnumerable<int>>(entities.Select(entity => entity.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos diários");
                return Result.Failure<IEnumerable<int>>(Error.Problem("Diary", "CreateRangeError").Description);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                Diary? entity = await _diaryRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("Diary", "NotFound").Description);

                await _diaryRepository.Delete(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir diário {DiaryId}", id);
                return Result.Failure<bool>(Error.Problem("Diary", "DeleteError").Description);
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("General", "EmptyList").Description);

                List<Diary> entities = new();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    Diary? entity = await _diaryRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                    return Result.Failure<bool>(Error.NotFound("Diary", "SomeNotFound").Description);

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Diary", "AllNotFound").Description);

                foreach (var entity in entities)
                    await _diaryRepository.Delete(entity, cancellationToken);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos diários {DiaryIds}", ids);
                return Result.Failure<bool>(Error.Problem("Diary", "DeleteRangeError").Description);
            }
        }

        public async Task<Result<IEnumerable<DiaryDTO>>> FindAsync(Expression<Func<DiaryDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<DiaryDTO>>(Error.Failure("General", "NullData").Description);

                // Para simplificar, não implementa filtro por predicate no momento
                IEnumerable<Diary?> entities = await _diaryRepository.GetAll(cancellationToken);
                IEnumerable<DiaryDTO> dtos = entities.Select(DiaryMapper.ToDTO);

                return Result.Success<IEnumerable<DiaryDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar diários com predicado");
                return Result.Failure<IEnumerable<DiaryDTO>>(Error.Problem("Diary", "FindError").Description);
            }
        }

        public async Task<Result<IEnumerable<DiaryDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Diary?> entities = await _diaryRepository.GetAll(cancellationToken);
                IEnumerable<DiaryDTO> dtos = entities.Select(DiaryMapper.ToDTO);

                return Result.Success<IEnumerable<DiaryDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os diários");
                return Result.Failure<IEnumerable<DiaryDTO>>(Error.Problem("Diary", "GetAllError").Description);
            }
        }

        public async Task<Result<IEnumerable<DiaryDTO>>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                if (userId <= 0)
                    return Result.Failure<IEnumerable<DiaryDTO>>(Error.Failure("General", "InvalidId").Description);

                IEnumerable<Diary?> entities = await _diaryRepository.GetAllByUserId(userId, cancellationToken);
                IEnumerable<DiaryDTO> dtos = entities.Select(DiaryMapper.ToDTO!);

                return Result.Success<IEnumerable<DiaryDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar diários do usuário {UserId}", userId);
                return Result.Failure<IEnumerable<DiaryDTO>>(Error.Problem("Diary", "GetByUserIdError").Description);
            }
        }

        public async Task<Result<DiaryDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return Result.Failure<DiaryDTO>(Error.Failure("General", "InvalidId").Description);

                Diary? entity = await _diaryRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<DiaryDTO>(Error.NotFound("Diary", "NotFound").Description);

                DiaryDTO dto = DiaryMapper.ToDTO(entity);
                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar diário {DiaryId}", id);
                return Result.Failure<DiaryDTO>(Error.Problem("Diary", "GetByIdError").Description);
            }
        }

        public async Task<Result<(IEnumerable<DiaryDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1)
                    return Result.Failure<(IEnumerable<DiaryDTO>, int)>(Error.Failure("Diary", "InvalidPage").Description);

                if (pageSize < 1)
                    return Result.Failure<(IEnumerable<DiaryDTO>, int)>(Error.Failure("Diary", "InvalidPageSize").Description);

                IEnumerable<Diary?> all = await _diaryRepository.GetAll(cancellationToken);
                int totalCount = all.Count();

                IEnumerable<DiaryDTO> items = all
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(DiaryMapper.ToDTO!);

                return Result.Success<(IEnumerable<DiaryDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de diários (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<DiaryDTO>, int)>(Error.Problem("Diary", "GetPagedError").Description);
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateDiaryDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.Failure("General", "NullData").Description);

                if (dto.Id <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                Diary? entity = await _diaryRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(Error.NotFound("Diary", "NotFound").Description);

                Diary? existingDiary = await _diaryRepository.GetByDate(entity.UserId, dto.Date, cancellationToken);
                if (existingDiary != null && existingDiary.Id != dto.Id)
                    return Result.Failure<bool>(Error.Conflict("Diary", "DateConflict").Description);

                entity.UpdateDate(dto.Date);
                entity.MarkAsUpdated();
                // Atualizar refeições e líquidos pode ser complexo e depende da regra de negócio
                // Aqui você pode implementar lógica para atualizar as coleções conforme necessário

                await _diaryRepository.Update(entity, cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar diário {@DiaryData}", dto);
                return Result.Failure<bool>(Error.Problem("Diary", "UpdateError").Description);
            }
        }

        public async Task<Result<bool>> AddMealToDiaryAsync(int diaryId, CreateMealDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (diaryId <= 0)
                    return Result.Failure<bool>(Error.Failure("General", "InvalidId").Description);

                if (dto == null)
                    return Result.Failure<bool>(Error.Failure("General", "NullData").Description);

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Failure<bool>(Error.Failure("Meal", "NameRequired").Description);

                Diary? diary = await _diaryRepository.GetById(diaryId, cancellationToken);
                if (diary == null)
                    return Result.Failure<bool>(Error.NotFound("Diary", "NotFound").Description);

                Meal meal = MealMapper.ToEntity(dto);
                meal.SetDiaryId(diaryId);
                diary.AddMeal(meal);

                await _diaryRepository.Update(diary, cancellationToken);

                // Publicar eventos de domínio
                foreach (var domainEvent in diary.DomainEvents)
                {
                    await _publisher.Publish(domainEvent, cancellationToken);
                }
                diary.ClearDomainEvents();

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar refeição ao diário {DiaryId}", diaryId);
                return Result.Failure<bool>(Error.Problem("Diary", "AddMealError").Description);
            }
        }
    }
}