namespace Films.Contracts;

public record CreateAuthorContract
{
    public string Name { get; set; } = "";
    public string SurName { get; set; } = "";
    public string LastName { get; set; } = "";
    public ushort BornDate { get; set; } = 0;
}