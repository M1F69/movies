namespace Films.Contracts;

public record CreateMovieWithFileContract: CreateMovieContract
{
    public IEnumerable<IFormFile> Files { get; set; }
}