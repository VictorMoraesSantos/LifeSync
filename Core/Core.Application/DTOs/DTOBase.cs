using System.Text.Json.Serialization;

namespace Core.Application.DTO
{
    public record DTOBase(
        [property: JsonPropertyOrder(-3)] int Id,
        [property: JsonPropertyOrder(-2)] DateTime CreatedAt,
        [property: JsonPropertyOrder(-1)] DateTime? UpdatedAt);
}
