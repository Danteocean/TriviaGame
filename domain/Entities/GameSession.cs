using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("GameSessions")]
public class GameSession : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public int PlayerId { get; set; }
    public int CurrentRound { get; set; } = 1;
    public decimal AccumulatedPrize { get; set; } = 0;
    public int IdStatus { get; set; } = 1; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Player Player { get; set; } = null!;
}