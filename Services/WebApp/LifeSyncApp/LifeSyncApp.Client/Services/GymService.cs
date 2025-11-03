using LifeSyncApp.Client.Models.Common;
using LifeSyncApp.Client.Models.Gym;
using System.Net.Http.Json;
using System.Text.Json;

namespace LifeSyncApp.Client.Services
{
    public class GymService : IGymService
    {
        private readonly HttpClient _httpClient;

        public GymService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResult<List<ExerciseDto>>> GetExercisesAsync()
        {
            var response = await _httpClient.GetAsync("/gym-service/api/exercises");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<List<ExerciseDto>>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult<List<ExerciseDto>> { Success = true, StatusCode = (int)response.StatusCode, Data = new List<ExerciseDto>() };
            }
            var result = JsonSerializer.Deserialize<HttpResult<List<ExerciseDto>>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult<List<ExerciseDto>> { Success = false };
        }

        public async Task<HttpResult<ExerciseDto>> GetExerciseByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/gym-service/api/exercises/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<ExerciseDto>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult<ExerciseDto> { Success = false, StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
            }
            var result = JsonSerializer.Deserialize<HttpResult<ExerciseDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult<ExerciseDto> { Success = false };
        }

        public async Task<HttpResult<ExerciseDto>> CreateExerciseAsync(CreateExerciseRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/gym-service/api/exercises", request);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<ExerciseDto>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult<ExerciseDto> { Success = false, StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
            }
            var result = JsonSerializer.Deserialize<HttpResult<ExerciseDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult<ExerciseDto> { Success = false };
        }

        public async Task<HttpResult> UpdateExerciseAsync(int id, UpdateExerciseRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/gym-service/api/exercises/{id}", request);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult { Success = true, StatusCode = (int)response.StatusCode };
            }
            var result = JsonSerializer.Deserialize<HttpResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteExerciseAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/gym-service/api/exercises/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult { Success = true, StatusCode = (int)response.StatusCode };
            }
            var result = JsonSerializer.Deserialize<HttpResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult<List<RoutineDto>>> GetRoutinesAsync()
        {
            var response = await _httpClient.GetAsync("/gym-service/api/routines");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<List<RoutineDto>>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult<List<RoutineDto>> { Success = true, StatusCode = (int)response.StatusCode, Data = new List<RoutineDto>() };
            }
            var result = JsonSerializer.Deserialize<HttpResult<List<RoutineDto>>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult<List<RoutineDto>> { Success = false };
        }

        public async Task<HttpResult<RoutineDto>> GetRoutineByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/gym-service/api/routines/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<RoutineDto>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult<RoutineDto> { Success = false, StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
            }
            var result = JsonSerializer.Deserialize<HttpResult<RoutineDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult<RoutineDto> { Success = false };
        }

        public async Task<HttpResult<RoutineDto>> CreateRoutineAsync(CreateRoutineRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/gym-service/api/routines", request);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<RoutineDto>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult<RoutineDto> { Success = false, StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
            }
            var result = JsonSerializer.Deserialize<HttpResult<RoutineDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult<RoutineDto> { Success = false };
        }

        public async Task<HttpResult> UpdateRoutineAsync(int id, UpdateRoutineRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/gym-service/api/routines/{id}", request);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult { Success = true, StatusCode = (int)response.StatusCode };
            }
            var result = JsonSerializer.Deserialize<HttpResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteRoutineAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/gym-service/api/routines/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult { Success = true, StatusCode = (int)response.StatusCode };
            }
            var result = JsonSerializer.Deserialize<HttpResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult<List<TrainingSessionDto>>> GetTrainingSessionsAsync()
        {
            var response = await _httpClient.GetAsync("/gym-service/api/training-sessions");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<List<TrainingSessionDto>>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                // Trate204/empty body como lista vazia para não quebrar o render
                return new HttpResult<List<TrainingSessionDto>> { Success = true, StatusCode = (int)response.StatusCode, Data = new List<TrainingSessionDto>() };
            }
            var result = JsonSerializer.Deserialize<HttpResult<List<TrainingSessionDto>>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult<List<TrainingSessionDto>> { Success = false };
        }

        public async Task<HttpResult<TrainingSessionDto>> GetTrainingSessionByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/gym-service/api/training-sessions/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<TrainingSessionDto>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult<TrainingSessionDto> { Success = false, StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
            }
            var result = JsonSerializer.Deserialize<HttpResult<TrainingSessionDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult<TrainingSessionDto> { Success = false };
        }

        public async Task<HttpResult<TrainingSessionDto>> CreateTrainingSessionAsync(CreateTrainingSessionRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("/gym-service/api/training-sessions", request);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<TrainingSessionDto>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult<TrainingSessionDto> { Success = false, StatusCode = (int)response.StatusCode, Errors = new[] { "Empty response" } };
            }

            // Gym API Create retorna Created com Id (int). Tente desserializar como HttpResult<int>
            try
            {
                var created = JsonSerializer.Deserialize<HttpResult<int>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (created?.Success == true && created.Data >0)
                {
                    // Constrói DTO básico com dados enviados
                    return new HttpResult<TrainingSessionDto>
                    {
                        Success = true,
                        StatusCode =201,
                        Data = new TrainingSessionDto
                        {
                            Id = created.Data,
                            StartTime = request.StartTime,
                            EndTime = request.EndTime,
                            Notes = request.Notes
                        }
                    };
                }
            }
            catch { /* fallback abaixo */ }

            // Fallback para shape direto
            var result = JsonSerializer.Deserialize<HttpResult<TrainingSessionDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult<TrainingSessionDto> { Success = false };
        }

        public async Task<HttpResult> UpdateTrainingSessionAsync(int id, UpdateTrainingSessionRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"/gym-service/api/training-sessions/{id}", request);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult { Success = true, StatusCode = (int)response.StatusCode };
            }
            var result = JsonSerializer.Deserialize<HttpResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult { Success = false };
        }

        public async Task<HttpResult> DeleteTrainingSessionAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/gym-service/api/training-sessions/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Errors = await ReadErrorsAsync(response)
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return new HttpResult { Success = true, StatusCode = (int)response.StatusCode };
            }
            var result = JsonSerializer.Deserialize<HttpResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new HttpResult { Success = false };
        }

        private static async Task<string[]> ReadErrorsAsync(HttpResponseMessage response)
        {
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json)) return new[] { response.ReasonPhrase ?? "Erro" };

                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("errors", out var errorsProp))
                {
                    if (errorsProp.ValueKind == JsonValueKind.Array)
                    {
                        return errorsProp.EnumerateArray().Select(e => e.GetString() ?? string.Empty).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                    }
                    if (errorsProp.ValueKind == JsonValueKind.Object)
                    {
                        var list = new List<string>();
                        foreach (var prop in errorsProp.EnumerateObject())
                        {
                            if (prop.Value.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var item in prop.Value.EnumerateArray())
                                {
                                    var msg = item.GetString();
                                    if (!string.IsNullOrWhiteSpace(msg)) list.Add(msg!);
                                }
                            }
                        }
                        return list.Count >0 ? list.ToArray() : new[] { "Requisição inválida" };
                    }
                }
                var title = doc.RootElement.TryGetProperty("title", out var t) ? t.GetString() : null;
                var detail = doc.RootElement.TryGetProperty("detail", out var d) ? d.GetString() : null;
                return new[] { detail ?? title ?? response.ReasonPhrase ?? "Erro" };
            }
            catch
            {
                return new[] { response.ReasonPhrase ?? "Erro" };
            }
        }
    }
}
