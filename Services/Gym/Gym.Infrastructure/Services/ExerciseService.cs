using BuildingBlocks.Results;
using Core.Domain.Exceptions;
using Gym.Application.Contracts;
using Gym.Application.DTOs.Exercise;
using Gym.Application.Mapping;
using Gym.Domain.Entities;
using Gym.Domain.Errors;
using Gym.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly ILogger<ExerciseService> _logger;

        public ExerciseService(IExerciseRepository exerciseRepository, ILogger<ExerciseService> logger)
        {
            _exerciseRepository = exerciseRepository;
            _logger = logger;
        }

        public async Task<Result<ExerciseDTO?>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _exerciseRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<ExerciseDTO?>(ExerciseErrors.NotFound(id));

                var dto = entity.ToDTO();

                return Result.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar exercício {Id}", id);
                return Result<ExerciseDTO?>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CountAsync(Expression<Func<ExerciseDTO, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<int>(Error.NullValue);

                var all = await _exerciseRepository.GetAll(cancellationToken);
                var count = all
                    .Where(e => e != null)
                    .Select(e => e.ToDTO())
                    .AsQueryable()
                    .Count(predicate);

                return Result.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar exercícios com filtro");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<int>> CreateAsync(CreateExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<int>(Error.NullValue);

                var entity = dto.ToEntity();

                await _exerciseRepository.Create(entity, cancellationToken);

                _logger.LogInformation("Exercício {Id} criado com sucesso", entity.Id);
                return Result.Success(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar exercício");
                return Result<int>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<int>>> CreateRangeAsync(IEnumerable<CreateExerciseDTO> dtos, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dtos == null || !dtos.Any())
                    return Result.Failure<IEnumerable<int>>(Error.Failure(Error.NullValue.Description));

                var entities = new List<Exercise>();
                var errors = new List<(string Title, string ErrorMessage)>();

                foreach (var dto in dtos)
                {
                    try
                    {
                        var entity = dto.ToEntity();
                        entities.Add(entity);
                    }
                    catch (DomainException ex)
                    {
                        errors.Add((dto.Description ?? "Sem descricao", ex.Message));
                    }
                }

                if (errors.Any())
                {
                    var errorDetails = string.Join("; ", errors.Select(e => $"'{e.Title}': {e.ErrorMessage}"));
                    return Result.Failure<IEnumerable<int>>(Error.Failure($"Algumas tarefas possuem dados inválidos: {errorDetails}"));
                }

                await _exerciseRepository.CreateRange(entities, cancellationToken);

                _logger.LogInformation("Criadas {Count} exercicios com sucesso", entities.Count);
                return Result.Success<IEnumerable<int>>(entities.Select(e => e.Id).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplos exercicios");
                return Result.Failure<IEnumerable<int>>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _exerciseRepository.GetById(id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(ExerciseErrors.NotFound(id));

                await _exerciseRepository.Delete(entity, cancellationToken);

                _logger.LogInformation("Exercício {Id} excluído com sucesso", id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir exercício {Id}", id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result.Failure<bool>(Error.Failure("Lista de IDs inválida ou vazia"));

                var entities = new List<Exercise>();
                var notFoundIds = new List<int>();

                foreach (var id in ids)
                {
                    var entity = await _exerciseRepository.GetById(id, cancellationToken);
                    if (entity != null)
                        entities.Add(entity);
                    else
                        notFoundIds.Add(id);
                }

                if (notFoundIds.Any())
                {
                    var idsText = string.Join(", ", notFoundIds);
                    return Result.Failure<bool>(Error.NotFound($"Os seguintes exercícios não foram encontradas: {idsText}"));
                }

                if (!entities.Any())
                    return Result.Failure<bool>(Error.NotFound("Nenhum dos exercícios foi encontrado"));

                foreach (var entity in entities)
                {
                    await _exerciseRepository.Delete(entity, cancellationToken);
                }

                _logger.LogInformation("Excluídas {Count} exercícios com sucesso", entities.Count);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplas exercícios {Ids}", ids);
                return Result.Failure<bool>(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<ExerciseDTO?>>> FindAsync(Expression<Func<ExerciseDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (predicate == null)
                    return Result.Failure<IEnumerable<ExerciseDTO?>>(Error.NullValue);

                var entities = await _exerciseRepository.GetAll(cancellationToken);

                var dtos = entities
                    .Where(e => e != null)
                    .Select(e => e.ToDTO())
                    .AsQueryable()
                    .Where(predicate)
                    .ToList();

                return Result.Success<IEnumerable<ExerciseDTO?>>(dtos);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar exercícios com filtro");
                return Result<IEnumerable<ExerciseDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<IEnumerable<ExerciseDTO?>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var entities = await _exerciseRepository.GetAll(cancellationToken);
                if (entities == null || !entities.Any())
                    return Result.Success<IEnumerable<ExerciseDTO?>>(new List<ExerciseDTO>());

                var dtos = entities.Select(e => e.ToDTO()!).ToList();

                return Result.Success<IEnumerable<ExerciseDTO?>>(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os exercícios");
                return Result<IEnumerable<ExerciseDTO?>>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<(IEnumerable<ExerciseDTO?> Items, int TotalCount)>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                if (page < 1 || pageSize < 1)
                    return Result.Failure<(IEnumerable<ExerciseDTO?>, int)>(Error.Failure("Parâmetros de paginação inválidos"));

                var entities = await _exerciseRepository.GetAll(cancellationToken);
                var totalCount = entities.Count();
                var items = entities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(e => e.ToDTO())
                    .ToList();

                return Result.Success<(IEnumerable<ExerciseDTO?> Items, int TotalCount)>((items, totalCount));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar exercícios com paginação");
                return Result<(IEnumerable<ExerciseDTO?>, int)>.Failure(Error.Failure(ex.Message));
            }
        }

        public async Task<Result<bool>> UpdateAsync(UpdateExerciseDTO dto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                    return Result.Failure<bool>(Error.NullValue);

                var entity = await _exerciseRepository.GetById(dto.Id, cancellationToken);
                if (entity == null)
                    return Result.Failure<bool>(ExerciseErrors.NotFound(dto.Id));

                entity.Update(dto.Name, dto.Description, dto.MuscleGroup, dto.Type);

                await _exerciseRepository.Update(entity, cancellationToken);

                _logger.LogInformation("Exercício {Id} atualizado com sucesso", dto.Id);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar exercício {Id}", dto.Id);
                return Result<bool>.Failure(Error.Failure(ex.Message));
            }
        }
    }
}
