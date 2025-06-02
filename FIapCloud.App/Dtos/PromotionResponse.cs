namespace FIapCloud.App.Dtos;

public class PromotionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime InitialDate { get; set; }
    public DateTime FinalDate { get; set; }
    public decimal Percentage { get; set; }
    public bool Enable { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Guid> GameIds { get; set; } = new();
}
