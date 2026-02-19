using Connection.Data;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models
{
    public class DtoPlatformPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public PlatformBillingInterval BillingInterval { get; set; }
    }
    public interface IPlatformPlanRepository:IGenericRepo<PlatformPlan>
    {
        Task<IReadOnlyList<PlatformPlan>> GetAllAsync();
    }
    public class clsPlatformPlanRepo : GenericRepo<PlatformPayment>
    {


        public clsPlatformPlanRepo(
            SaasDashboardContext context,
            ILogger<clsPlatformPlanRepo> logger)
        :base(context,logger){
        
        }

        public async Task<IReadOnlyList<PlatformPlan>> GetAllAsync()
        {
            try
            {
                return await _context.PlatformPlans.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PlatformPlans");
                throw;
            }
        }


        public async Task<bool> AddAsync(PlatformPlan plan)
        {
            try
            {
                _context.PlatformPlans.Add(plan);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding PlatformPlan {Name}", plan.Name);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(PlatformPlan plan)
        {
            try
            {
                _context.PlatformPlans.Update(plan);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating PlatformPlan {Id}", plan.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(PlatformPlan plan)
        {
            try
            {
                _context.PlatformPlans.Remove(plan);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PlatformPlan {Id}", plan.Id);
                return false;
            }
        }
    }


}
