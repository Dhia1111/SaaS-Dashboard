using Connection.Data;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDomainRepo {



    Task<IReadOnlyList<Domain>> AllAsync();

    Task<int> AddAsync(Domain entity);

    Task<bool> UpdateAsync(Domain entity);

    Task<bool> DeleteAsync(Domain entity);
    Task<Domain?> FindAsync(int id);

    Task<Domain?> FindAsync(string Name);




}

namespace Connection.models
{

    public class clsDomaineRepo:IDomainRepo

    {
        private readonly SaasDashboardContext _context;
        private readonly ILogger<clsDomaineRepo> _logger;
        public clsDomaineRepo(SaasDashboardContext cotext,ILogger<clsDomaineRepo> logger) { 
        
            _context = cotext;
            _logger= logger;

        
        }

        public async Task<IReadOnlyList<Domain>> AllAsync()
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

        public async Task<int>AddAsync(Domain entity)
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

        public async Task<bool> UpdateAsync(Domain entity)
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

        public async Task<bool> DeleteAsync(Domain entity)
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
