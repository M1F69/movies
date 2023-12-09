using System.Data;
using System.Net;
using System.Text;
using Films.Contracts;
using Films.Data;
using Films.Data.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Npgsql;

namespace Films.Controllers;

public class MoviesController(MovieDbContext context) : ODataController
{
    private const int FileSizeLimit = 10485760;

    [EnableQuery]
    public IActionResult Get(ODataQueryOptions<MovieEntity> options)
    {
        var query = options.ApplyTo(context.Set<MovieEntity>()) as IQueryable<MovieEntity>;

        return Ok(query!.ToList());
    }

    [EnableQuery]
    public IActionResult Get(ODataQueryOptions<MovieEntity> options, [FromODataUri] Guid key)
    {
        var query = options.ApplyTo(context.Set<MovieEntity>()) as IQueryable<MovieEntity>;

        return Ok(query!.FirstOrDefault(x => x.Id == key));
    }

    public IActionResult Post([FromBody] CreateMovieContract payload, [FromForm] IEnumerable<IFormFile> Files)
    {
        var entity = new MovieEntity
        {
            Name = payload.Name,
            Description = payload.Description,
            Year = payload.Year,
            Type = payload.Type
        };

        // context.Set<MovieEntity>().Add(entity);

        // context.SaveChanges();

        return Ok(entity);
    }   
    
    public IActionResult Post([FromForm] CreateMovieWithFileContract payload)
    {
        var entity = new MovieEntity
        {
            Name = payload.Name,
            Description = payload.Description,
            Year = payload.Year,
            Type = payload.Type
        };

        // context.Set<MovieEntity>().Add(entity);

        // context.SaveChanges();

        return Ok(entity);
    }   

    public IActionResult Patch([FromODataUri] Guid key, [FromBody] Delta<MovieEntity> delta)
    {
        var entity = context.Set<MovieEntity>().FirstOrDefault(p => p.Id.Equals(key));

        if (entity is null)
        {
            return NotFound();
        }

        delta.Patch(entity);

        context.Entry(entity).State = EntityState.Modified;
        context.SaveChanges();

        return Ok(entity);
    }

    public IActionResult Delete([FromODataUri] Guid key)
    {
        var entity = context.Set<MovieEntity>().FirstOrDefault(p => p.Id.Equals(key));

        if (entity is null)
        {
            return NotFound();
        }

        context.Remove(entity);
        context.SaveChanges();

        return Ok(entity);
    }

    [HttpPost("Movies/{key}/upload")]
    [HttpPost("Movies({key})/upload")]
    public async Task<IActionResult> Upload([FromODataUri] Guid key)
    {
        var entity = await context.Set<MovieEntity>().FirstOrDefaultAsync(p => p.Id.Equals(key));
        if (entity is null)
        {
            return NotFound();
        }

       
        return BadRequest();
    }

    private static Encoding GetEncoding(MultipartSection section)
    {
        var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);
        if (!hasMediaTypeHeader || Encoding.UTF8.Equals(mediaType.Encoding))
        {
            return Encoding.UTF8;
        }

        return mediaType.Encoding;
    }

    private async Task<long> ProcessStreamedFile(MultipartSection section,
        ContentDispositionHeaderValue contentDisposition)
    {
        long blobId = 0;

        try
        {
            var createCommand = context.Database.GetDbConnection().CreateCommand();
            createCommand.CommandText = "SELECT lo_creat(-1) AS blob_id";
            //createCommand.Transaction = m_context.Database.CurrentTransaction.GetDbTransaction();

            blobId = Convert.ToInt64(await createCommand.ExecuteScalarAsync());


            await using var memoryStream = new MemoryStream();
            await section.Body.CopyToAsync(memoryStream);

            // Check if the file is empty or exceeds the size limit.
            if (memoryStream.Length == 0)
            {
                ModelState.AddModelError("File", "The file is empty.");
            }
            else if (memoryStream.Length > FileSizeLimit)
            {
                var megabyteSizeLimit = FileSizeLimit / 1048576;

                ModelState.AddModelError("File", $"The file exceeds {megabyteSizeLimit:N1} MB.");
            }
            else if (!FileHelper.FileHelpers.IsValidFileExtensionAndSignature(contentDisposition.FileName.Value,
                         memoryStream, new[] {".gif", ".png", ".jpeg", ".jpg"}))
            {
                ModelState.AddModelError("File",
                    "The file type isn't permitted or the file's signature doesn't match the file's extension.");
            }

            var putCommand = context.Database.GetDbConnection().CreateCommand();
            putCommand.CommandText = "SELECT lo_put(@blobId, @offset, @buffer)";
            putCommand.Parameters.Add(new NpgsqlParameter("@blobId", DbType.Int64) {Value = blobId});
            putCommand.Parameters.Add(new NpgsqlParameter("@offset", DbType.Int64) {Value = 0});
            putCommand.Parameters.Add(new NpgsqlParameter("@buffer", DbType.Binary) {Value = memoryStream.ToArray()});
            //putCommand.Transaction = m_context.Database.CurrentTransaction.GetDbTransaction();

            await putCommand.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("File",
                $"The upload failed. Please contact the Help Desk for support. Error: {ex.HResult}");
            // Log the exception
        }

        return blobId;
    }
}