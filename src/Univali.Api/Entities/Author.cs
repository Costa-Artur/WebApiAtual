using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univali.Api.Entities;

public class Author 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AuthorId {get; set;}
    [Required]
    [MaxLength(50)]
    public string FirstName {get; set;} = string.Empty;
    [Required]
    [MaxLength(50)]
   	public string LastName {get; set;} = string.Empty;
    [MaxLength(11)]
    public List<Course> Courses {get;set;} = new();

    public Author (string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}