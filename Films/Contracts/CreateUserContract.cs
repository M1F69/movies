namespace Films.Contracts;

public class CreateUserContract
{
    
    public string NickName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Password { get; set; } = "";
    public string Mail { get; set; } = "";
}