using System.ComponentModel.DataAnnotations;

public class ReplaceTaskTagDto
{
    [Required]
    public Guid NewTagId { get; set; }
}
