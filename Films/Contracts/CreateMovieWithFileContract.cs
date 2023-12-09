using Films.Types;

namespace Films.Contracts;

public record CreateMovieWithFileContract: CreateMovieContract
{
    public string Name { get; set; } = "";
    
    public string Description { get; set; } = "";

    public ushort Year { get; set; } = 0;

    public MovieType Type { get; set; } = MovieType.Default;

    public IEnumerable<IFormFile> Files { get; set; }
}