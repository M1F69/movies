using Films.Types;

namespace Films.Contracts;

public record CreateMovieWithFileContract
{
    
    public string Name { get; set; }
    
    public string Description { get; set; }

    public ushort Year { get; set; }

    public MovieType Type { get; set; }
    
    public bool Viewed { get; set; }
    
    public MovieGenreType[] Genre { get; set; }
    
    public string TrailerHref { get; set; }

    public IEnumerable<IFormFile> Files { get; set; } = new List<IFormFile>();
}