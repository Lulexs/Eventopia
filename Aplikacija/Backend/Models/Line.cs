namespace Backend.Models;

public class Line
{
    [Key]
    public int ID { get; set; }

    public double X1 { get; set; }

    public double X2 { get; set; }

    public double Y1 { get; set; }

    public double Y2 { get; set; }

    public required PlanProstora PlanProstora { get; set; }
    [ForeignKey("Corner1FK")]
    public DraggableItem? Corner1 { get; set; }
    [ForeignKey("Corner2FK")]
    public DraggableItem? Corner2 { get; set; }
}