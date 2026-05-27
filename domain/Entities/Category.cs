using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Categories")]
public class Category : IEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DifficultyLevel { get; set; }

    public int Points { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}