using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univali.Api.Entities;

public class Course
{   
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CourseId { get; set;}
    [Required]
    [MaxLength(60)]
    public string Title { get; set;} = string.Empty;
    [Required]
    [MaxLength(190)]
    public string Description { get; set;} = string.Empty;
    public decimal Price { get; set;}
    public List<Author> Authors { get; set;} = new();

    public Course(string title, decimal price ,string description)
    {
        Title = title;
        Price = price;
        Description = description;
    }


}