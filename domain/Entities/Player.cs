using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Player")]
public class Player : IEntity
{
    [Key]
    public int id { get; set; }
    public string alias { get; set; } = string.Empty;
    public int totalPointsAchieved { get; set; } = 0;

    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}