using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;


[Table("Questions")]
public class Question : IEntity
{
    [Key]
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Text { get; set; } = string.Empty;

    public Category Category { get; set; } = null!;
    public ICollection<Option> Options { get; set; } = new List<Option>();
}