using BuildingBlocks.CQRS.Publisher;
using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.DTOs.Meal;
using Nutrition.Application.Interfaces;
using Nutrition.Application.Mapping;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Errors;
using Nutrition.Domain.Filters;
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
            _diaryRepository = diaryRepository ?? throw new ArgumentNullException(nameof(diaryRepository));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<DiaryDTO>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var diary = await _diaryRepository.GetById(id, cancellationToken);
                if (diary == null)
                    return Result.Failure<DiaryDTO>(DiaryErrors.NotFound(id));

                var diaryDTO = diary.ToDTO();

                return Result.Success(diaryDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar diário {DiaryId}", id);
                return Result.Failure<DiaryDTO>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<DiaryDTO> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<DiaryDTO>, int)>(Error.Failure("Parâmetros de paginação inválidos"));

                var entities = await _diaryRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Where(e => e != null)
                    .Select(DiaryMapper.ToDTO)
                    .ToList();

                return Result.Success<(IEnumerable<DiaryDTO> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter página de diários (Página: {Page}, Tamanho: {PageSize})", page, pageSize);
                return Result.Failure<(IEnumerable<DiaryDTO>, int)>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<DiaryDTO>>> FindAsync(Expression<Func<DiaryDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<DiaryDTO>>(Error.NullValue);

                var entities = await _diaryRepository.GetAll(cancellationToken);
                var dtos = entities
                    .Where(e => e != null)
                    .Select(DiaryMapper.ToDTO)
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<DiaryDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar diários com predicado");
                return Result.Failure<IEnumerable<DiaryDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<DiaryDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var all = await _diaryRepository.GetAll(cancellationToken);
                var dtos = all
                    .Where(e => e != null)
                    .Select(DiaryMapper.ToDTO)
                    .AsQueryable();
                int count = predicate != null ? dtos.Count(predicate) : dtos.Count();
                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar diários");
                return Result.Failure<int>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<DiaryDTO>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _diaryRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<DiaryDTO>>(new List<DiaryDTO>());

                var dtos = entities.Where(e => e != null).Select(DiaryMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<DiaryDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os diários");
                return Result.Failure<IEnumerable<DiaryDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<DiaryDTO>>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                    return Result.Failure<IEnumerable<DiaryDTO>>(DiaryErrors.InvalidUserId);

                var entities = await _diaryRepository.GetAllByUserId(userId, cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<DiaryDTO>>(new List<DiaryDTO>());

                var dtos = entities.Select(DiaryMapper.ToDTO).ToList();

                return Result.Success<IEnumerable<DiaryDTO>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar diários do usuário {UserId}", userId);
                return Result.Failure<IEnumerable<DiaryDTO>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateDiaryDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var existingDiary = await _diaryRepository.GetByDate(dto.userId, dto.date, cancellationToken);
                if (existingDiary != null)
                    return Result.Failure<int>(Error.Conflict("Já existe um diário para esta data"));

                var entity = dto.ToEntity();
                await _diaryRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Diário criado com sucesso: {DiaryId}", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar diário {@DiaryData}", dto);
                return Result.Failure<int>(DiaryErrors.CreateError);
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateDiaryDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure("Lista de diários inválida ou vazia"));

                var entities = new List<Diary>();
                var errors = new List<(int UserId, DateOnly Date, string ErrorMessage)>();

                foreach (var dto in dtos)
                {
                    try
                    {
                        var entity = dto.ToEntity();
                        entities.Add(entity);
                    }
                    catch (DomainException ex)
                    {
                        errors.Add((dto.userId, dto.date, ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"Usuário {e.UserId} - {e.Date:yyyy-MM-dd}: {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Alguns diários possuem dados inválidos: {errorDetails}"));
                }

                await _diaryRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Criados {Count} diários com sucesso", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos diários");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateDiaryDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _diaryRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(DiaryErrors.NotFound(dto.Id));

                var existingDiary = await _diaryRepository.GetByDate(entity.UserId, dto.Date, cancellationToken);
                if (existingDiary != null && existingDiary.Id != dto.Id)
                    return Result.Failure<bool>(Error.Conflict("Já existe um diário para esta data"));

                entity.UpdateDate(dto.Date);
                await _diaryRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Diário atualizado com sucesso: {DiaryId}", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar diário {DiaryId}", dto.Id);
                return Result.Failure<bool>(DiaryErrors.UpdateError);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _diaryRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(DiaryErrors.NotFound(id));

                await _diaryRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Diário excluído com sucesso: {DiaryId}", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir diário {DiaryId}", id);
                return Result.Failure<bool>(DiaryErrors.DeleteError);
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Lista de IDs inválida ou vazia"));

                var entities = new List<Diary>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _diaryRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"Os seguintes diários não foram encontrados: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Nenhum dos diários foi encontrado"));

                foreach (var entity in entities)
                {
                    await _diaryRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Excluídos {Count} diários com sucesso", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplos diários {DiaryIds}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> AddMealToDiaryAsync(int diaryId, CreateMealDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(DiaryErrors.NullMeal);

                var diary = await _diaryRepository.GetById(diaryId, cancellationToken);
                if (diary == null)
                    return Result.Failure<bool>(DiaryErrors.NotFound(diaryId));

                var meal = dto.ToEntity();

                diary.AddMeal(meal);

                await _diaryRepository.Update(diary, cancellationToken);

                foreach (var domainEvent in diary.DomainEvents)
                {
                    await _publisher.Publish(domainEvent, cancellationToken);
                }

                diary.ClearDomainEvents();

                _logger.LogInformation("Refeição adicionada ao diário {DiaryId} com sucesso", diaryId);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar refeição ao diário {DiaryId}", diaryId);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<DiaryDTO> Items, PaginationData Pagination)>> GetByFilterAsync(
    DiaryFilterQueryDTO filter,
    CancellationToken cancellationToken)
        {
            try
            {
                var domainFilter = new DiaryQueryFilter(
                    filter.Id,
                    filter.UserId,
                    filter.TotalCaloriesEqual,
                    filter.TotalCaloriesGreaterThen,
                    filter.TotalCaloriesLessThen,
                    filter.TotalLiquidsMlEqual,
                    filter.TotalLiquidsMlGreaterThen,
                    filter.TotalLiquidsMlLessThen,
                    filter.MealId,
                    filter.LiquidId,
                    filter.CreatedAt,
                    filter.UpdatedAt,
                    filter.IsDeleted,
                    filter.SortBy,
                    filter.SortDesc,
                    filter.Page,
                    filter.PageSize);

                var (entities, totalItems) = await _diaryRepository.FindByFilter(domainFilter, cancellationToken);
                if (!entities.Any())
                    return Result.Success<(IEnumerable<DiaryDTO> Items, PaginationData Pagination)>(
                        (new List<DiaryDTO>(), new PaginationData(filter.Page, filter.PageSize)));

                var dtos = entities
                    .Where(e => e != null)
                    .Select(DiaryMapper.ToDTO)
                    .ToList();

                var pageSize = filter.PageSize ?? 50;
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var pagination = new PaginationData(filter.Page, pageSize, totalItems, totalPages);

                return Result.Success<(IEnumerable<DiaryDTO> Items, PaginationData Pagination)>((dtos, pagination));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar diários com filtro {@Filter}", filter);
                return Result.Failure<(IEnumerable<DiaryDTO>, PaginationData)>(Error.Failure(ex.Message));
            }
        }

    }
}