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

public class UsersController(MovieDbContext context) : ODataController
{
    [EnableQuery]
    public IActionResult Get(ODataQueryOptions<UserEntity> options)
    {
        var query = options.ApplyTo(context.Set<UserEntity>()) as IQueryable<UserEntity>;

        return Ok(query!.ToList());
    }
    
    
    public IActionResult Post([FromBody] CreateUserContract payload)
    {
        var entity = new UserEntity
        {
            NickName = payload.NickName,
            Mail = payload.Mail,
            FullName = payload.FullName,
            Password = payload.Password

            
        };
 
        context.Set<UserEntity>().Add(entity);
        context.SaveChanges();

        return Ok(entity);
    }
    
    //TODO: не уверена на счет того что в (↓скобках↓)  удалять по никнейму
    public IActionResult Delete([FromODataUri] string key)
    {
        var entity = context.Set<UserEntity>().FirstOrDefault(x => x.NickName == key);

        if (entity is null)
        {
            return NotFound();
        }

        context.Remove(entity);
        context.SaveChanges();
        
        return Ok(entity);
    }
}