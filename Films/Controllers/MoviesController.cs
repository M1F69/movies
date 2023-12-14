using System.Data;
using System.Net;
using System.Net.Mime;
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

    [EnableQuery]
    public IQueryable<MovieEntity> Get(ODataQueryOptions<MovieEntity> options)
    {
        var query = options.ApplyTo(context.Set<MovieEntity>()) as IQueryable<MovieEntity>;

        return context.Set<MovieEntity>();
    }

    [EnableQuery]
    public IActionResult Get(ODataQueryOptions<MovieEntity> options, [FromODataUri] Guid key)
    {
        var query = options.ApplyTo(context.Set<MovieEntity>()) as IQueryable<MovieEntity>;

        return Ok(query!.FirstOrDefault(x => x.Id == key));
    }

    public async Task<IActionResult> Post([FromForm] CreateMovieWithFileContract payload)
    {
        var entity = new MovieEntity
        {
            Name = payload.Name,
            Description = payload.Description,
            Year = payload.Year,
            Type = payload.Type,
            Genre = payload.Genre,
            Viewed = payload.Viewed
        };

        context.Set<MovieEntity>().Add(entity);
        await context.SaveChangesAsync();

        if (!payload.Files.Any()) return Ok(entity);

        var blob = payload.Files.First();

        await using var stream = new MemoryStream();
        await blob.CopyToAsync(stream);

        await context.Database.BeginTransactionAsync();
        
        var createCommand = context.Database.GetDbConnection().CreateCommand();
        createCommand.CommandText = "SELECT lo_creat(-1) AS blob_id";
        
        var blobId = Convert.ToInt64(await createCommand.ExecuteScalarAsync());
        
        var putCommand = context.Database.GetDbConnection().CreateCommand();
        putCommand.CommandText = "SELECT lo_put(@blobId, @offset, @buffer)";
        putCommand.Parameters.Add(new NpgsqlParameter("@blobId", DbType.Int64) { Value = blobId });
        putCommand.Parameters.Add(new NpgsqlParameter("@offset", DbType.Int64) { Value = 0 });
        putCommand.Parameters.Add(new NpgsqlParameter("@buffer", DbType.Binary) { Value = stream.ToArray() });

        await putCommand.ExecuteNonQueryAsync();
        
        var blobEntity = new BlobEntity
        {
            LoId = blobId,
            Name = blob.FileName,
            Size = blob.Length,
            MimeType = blob.ContentType,
            CreatedAt = DateTime.Now,
        };
        
        context.Set<BlobEntity>().Add(blobEntity);
        await context.SaveChangesAsync();

        entity.ImageId = blobEntity.Id;
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
        
        await context.Database.CommitTransactionAsync();

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
    
    [HttpGet("Movies/{key}/download")]
    [HttpGet("Movies({key})/download")]
    public async Task<IActionResult> Download([FromODataUri] Guid key)
    {
        var entity = await context.Set<MovieEntity>().Include(p => p.Image).FirstOrDefaultAsync(p => p.Id.Equals(key));
        if (entity is null)
        {
            return NotFound();
        }

        var blob = entity.Image;
        
        if (blob is null)
        {
            return NotFound();
        }
        
        const int size = 65536;
        
        var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT lo_get(@blobId, @offset, @buffer)";

        await command.Connection.OpenAsync();
        
        var cd = new ContentDisposition
        {
            FileName = blob.Name,
            Size = blob.Size,
            Inline = false
        };

        Response.Headers.Add("Content-Disposition", cd.ToString());
        Response.Headers.Add("Content-Description", "File Transfer");
        Response.Headers.Add("Content-Type", blob.MimeType);

        var stream = Response.BodyWriter.AsStream(true);
        var offset = 0;

        while (offset < blob.Size)
        {
            command.Parameters.Clear();
            command.Parameters.Add(new NpgsqlParameter("@blobId", DbType.Int64) { Value = blob.LoId });
            command.Parameters.Add(new NpgsqlParameter("@offset", DbType.Int64) { Value = offset });
            command.Parameters.Add(new NpgsqlParameter("@buffer", DbType.Int32) { Value = size });

            var buffer = (await command.ExecuteScalarAsync()) as byte[];

            await stream.WriteAsync(buffer, 0, buffer.Length);
            offset += buffer.Length;
        }
        
        stream.Close();

        return new EmptyResult();
    }

}