namespace Backend.Models;

public class Tag
{
    [Key]
    public int ID { get; set; }
    [MaxLength(50)]
    public required string TagName { get; set; }
    public List<Dogadjaj>? Dogadjaji { get; set; }
}