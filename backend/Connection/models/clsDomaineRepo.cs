using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DtoDomain
{

    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public int TenantId { get; set; }


}
public interface IDomainRepo :IGenericRepo<Domain> {




  
    Task<Domain?> FindAsync(int id);

    Task<Domain?> FindAsync(string Name);




}

namespace Connection.models
{

    public class clsDomaineRepo: GenericRepo<Domain>, IDomainRepo

    {
      
        public clsDomaineRepo(SaasDashboardContext context,ILogger<clsDomaineRepo> logger):base(context,logger) { 
     

        
        }

        public new async Task<IReadOnlyList<Domain>> GetAllAsync()
        {

            try
            {  
                var res = await _context.Domains.IgnoreQueryFilters().AsNoTracking().ToListAsync();
                 return res;
            } 
            catch(Exception ex) 
            {

                _logger.LogError(ex, "Could not fetch domain names  ");
                throw;
            }

        }

        public new async Task<int>AddAsync(Domain entity)
        {

            try
            {

                await _context.Domains.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity.Id;
            }
            catch(Exception ex) 
            {

                _logger.LogError(ex,"Could not add entity with the name  {name} ",entity.Name);
                throw;
            }


        }

        public new async Task<bool> UpdateAsync(Domain entity)
        {
            try{  
                
                _context.Domains.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        } catch(Exception ex) 
            {

                _logger.LogError(ex, "Could not update entity with a name  of {name} ", entity.Name);
                throw;
            }
        }

        public new  async Task<bool> DeleteAsync(Domain entity)
        {
            try{   _context.Domains.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }    catch(Exception ex) 
            {

                _logger.LogError(ex, "Could not delete entity with the name {name} ", entity.Name);
                throw;
            }
        }

        public async Task<Domain?>FindAsync(int id)
        {
            try
            {
                var res = await _context.Domains.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(e=>e.Id==id);
                return res;
            }
              catch(Exception ex) 
            {

                _logger.LogError(ex, "Could not find entity with an id of {id} ", id);
                throw;
            }
        }

        public async Task<Domain?> FindAsync(string Name)
        {
            try
            {
                var res = await _context.Domains.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(e => e.Name == Name);
                return res;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Could not find entity with a Name of {name} ", Name);
                throw;
            }
        }


    }
}
