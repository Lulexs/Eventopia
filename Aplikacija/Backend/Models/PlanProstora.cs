namespace Backend.Models;

public class PlanProstora
{
    [Key]
    public int ID { get; set; }

    public List<DraggableItem>? DraggableItems { get; set; }

    public List<Line>? Lines { get; set; }

    public required Prostor Prostor { get; set; }
    public required SurfaceDimension SurfaceDimension { get; set; }
    public Dogadjaj? Dogadjaj { get; set; }

}