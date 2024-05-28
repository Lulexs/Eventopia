namespace Backend.Models;

public class SurfaceDimension
{
    [Key]
    public int ID { get; set; }

    public double Height { get; set; }
    public double Width { get; set; }

    [ForeignKey("SurfaceDimension")]
    public required PlanProstora PlanProstora { get; set; }

}