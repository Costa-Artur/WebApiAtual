namespace Univali.Api.Entities;

//Classe que representa o user no database
public class User
{
    public int Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string UserName {get;set;} = string.Empty;
    public string Password {get;set;} = string.Empty;
}