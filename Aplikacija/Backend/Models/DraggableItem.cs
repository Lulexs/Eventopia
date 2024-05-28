namespace Backend.Models;

public class DraggableItem
{
    [Key]
    public int ID { get; set; }
    public TipItema Tip { get; set; }
    public double Top { get; set; }
    public double Left { get; set; }
    public double? Height { get; set; }
    public double? HeightFactor { get; set; }
    public int? BrojMesta { get; set; }
    public required PlanProstora PlanProstora { get; set; }
    public Rezervacija? Rezervacija { get; set; }
    [InverseProperty("Corner1")]
    public Line? Line1 { get; set; }
    [InverseProperty("Corner2")]
    public Line? Line2 { get; set; }
}