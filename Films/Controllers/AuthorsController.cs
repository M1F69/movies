using Films.Contracts;
using Films.Data;
using Films.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Films.Controllers;

public class AuthorsController(MovieDbContext context) : ODataController
{
    [EnableQuery]
    public IActionResult Get(ODataQueryOptions<AuthorEntity> options)
    {
        var query = options.ApplyTo(context.Set<AuthorEntity>()) as IQueryable<AuthorEntity>;

        return Ok(query!.ToList());
    }

    [EnableQuery]
    public IActionResult Get(ODataQueryOptions<AuthorEntity> options, [FromODataUri] Guid key)
    {
        var query = options.ApplyTo(context.Set<AuthorEntity>()) as IQueryable<AuthorEntity>;

        return Ok(query!.FirstOrDefault(x => x.Id == key));
    }

    public IActionResult Post([FromODataUri] Guid key, [FromBody] CreateAuthorContract payload)
    {
        var entity = new AuthorEntity
        {
            Name = payload.Name,
            SurName = payload.SurName,
            LastName = payload.LastName,
            BornDate = payload.BornDate
        };
 
        context.Set<AuthorEntity>().Add(entity);

        context.SaveChanges();

        return Ok(entity);
    }
    

    public IActionResult Patch([FromODataUri] Guid key, [FromBody] Delta<AuthorEntity> delta)
    {
        var entity = context.Set<AuthorEntity>().FirstOrDefault(x => x.Id == key);

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
        var entity = context.Set<AuthorEntity>().FirstOrDefault(x => x.Id == key);

        if (entity is null)
        {
            return NotFound();
        }

        context.Remove(entity);
        context.SaveChanges();
        
        return Ok(entity);
    }
}