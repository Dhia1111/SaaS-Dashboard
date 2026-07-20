using Connection.Data;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;
 

namespace Connection.models
{
    public class DtoPlatformSubscription
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int PricingOptionId { get; set; }
        public string StartedAt { get; set; } = null!;
        public string? EndsAt { get; set; }
        public DtoTenantPricingOption? PricingOption { get; set; }
        public bool IsActive { get; set; } = false;
         public bool IsItFree { get; set; } = false;
        public bool IsRegisterdToClientSubscription { get; set; }=false;

    }

    public interface IPlatformSubscriptionRepo : IGenericRepo<PlatformSubscription>
    {
        Task<PlatformSubscription?> GetByTenantIdAsync(int tenantId);

        Task<bool> UpdateAsync(Payment Payment, PlatformSubscription PlatformSubscription);
        Task<PlatformSubscription?> GetActiveByTenantIdAsync(int tenantId);
        Task<int> AddAsync(Payment Payment, PlatformSubscription PlatformSubscription);
        Task<bool> ActivateSubscriptionWithQueryFiltersIgnoreAsync(
    int paymentId);
        Task<bool> DisActivateSubscriptionWithQueryFiltersIgnoreAsync(int paymentId, enGeneralState status);
        Task<bool> ConfirmUpGradeWithQueryFiltersIgnoreAsync(int paymentId);
        Task<List<PlatformSubscription>> GetAllWhereIsNotRegisterdAsync();

    }


    public class clsPlatformSubscriptionRepo
        : GenericRepo<PlatformSubscription>, IPlatformSubscriptionRepo
    {
        public clsPlatformSubscriptionRepo(
            SaasDashboardContext context,
            ILogger<GenericRepo<PlatformSubscription>> logger)
            : base(context, logger)
        {
        }

        public async Task<PlatformSubscription?> GetByTenantIdAsync(int tenantId)
        {
            try
            {
                return await _context.PlatformSubscriptions
                    .Include(s => s.TenantPlanPricingOption)
                    .Include(s => s.Tenant)
                    .SingleOrDefaultAsync(s => s.TenantId == tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching PlatformSubscription by TenantId {TenantId}",
                    tenantId);

                throw;
            }
        }

        public async Task<PlatformSubscription?> GetActiveByTenantIdAsync(int tenantId)
        {
            try
            {
                return await _context.PlatformSubscriptions.IgnoreQueryFilters().Include(e => e.TenantPlanPricingOption)
                    .SingleOrDefaultAsync(s =>
                        s.TenantId == tenantId &&
                        s.IsActive == true
                       );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching ACTIVE PlatformSubscription for TenantId {TenantId}",
                    tenantId);

                throw;
            }
        }


        public async Task<int> AddAsync(Payment Payment, PlatformSubscription PlatformSubscription)
        {

            using var Transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.PlatformSubscriptions.AddAsync(PlatformSubscription);
                await _context.SaveChangesAsync();
                Payment.SubscriptionId = PlatformSubscription.Id;
                await _context.PlatformPayments.AddAsync(Payment);
                await _context.SaveChangesAsync();

                await Transaction.CommitAsync();


            }

            catch
            {
                Transaction.Rollback();

            }

            return PlatformSubscription.Id;



        }

        public async Task<bool> ActivateSubscriptionWithQueryFiltersIgnoreAsync(
    int paymentId)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {

                Payment? payment =
                    await _context.PlatformPayments.IgnoreQueryFilters()
                        .SingleOrDefaultAsync(
                            p => p.Id == paymentId);

                if (payment == null)
                {
                    _logger.LogWarning("Payment with and Id : {paymentId} does not exist   ", paymentId);

                    return false;

                }

                var Tenant = await _context.Tenants.SingleOrDefaultAsync(t => t.TenantId == payment.TenantId);
                if (Tenant == null)
                {
                    _logger.LogWarning("Payment with and Id : {paymentId} does not exist   ", paymentId);

                    return false;

                }

                Tenant.IsActive = true;


                if (payment.PaymentStatus ==
                    (int)enGeneralState.Success)
                    return true; // Idempotency

                var subscription =
                    await _context.PlatformSubscriptions.IgnoreQueryFilters()
                        .SingleOrDefaultAsync(
                            s => s.Id ==
                            payment.SubscriptionId);

                if (subscription == null)
                {
                    _logger.LogWarning("Payment with and Id : {paymentId} does not relate to any Subscription", paymentId);

                    return false;
                }

                payment.PaymentStatus =
                   (int)enGeneralState.Success;

                payment.CompletedAt =
                    DateTime.UtcNow;

                subscription.IsActive = true;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Faild to commit transaction ");

                return false;
            }
        }


        public async Task<bool> DisActivateSubscriptionWithQueryFiltersIgnoreAsync(
    int paymentId, enGeneralState status)
        {
            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                Payment? payment =
                    await _context.PlatformPayments.IgnoreQueryFilters()
                        .SingleOrDefaultAsync(
                            p => p.Id == paymentId);

                if (payment == null)
                {
                    _logger.LogWarning("Payment with and Id : {paymentId} does not exist   ", paymentId);

                    return false;

                }

                if (status ==
                      enGeneralState.Success)
                {
                    _logger.LogError("Invalid status for deactivation. Status cannot be 'Success' when deactivating a subscription.");
                    throw new Exception("Invalid status for deactivation. Status cannot be 'Success' when deactivating a subscription.");
                }
                else if (status ==
                   enGeneralState.Pending)
                {
                    _logger.LogError("Invalid status for deactivation. Status cannot be 'Pending' when deactivating a subscription.");
                    throw new Exception("Invalid status for deactivation. Status cannot be 'Pending' when deactivating a subscription.");
                }
                else if (status ==
                   enGeneralState.Active)
                {
                    _logger.LogError("Invalid status for deactivation. Status cannot be 'Active' when deactivating a subscription.");
                    throw new Exception("Invalid status for deactivation. Status cannot be 'Active' when deactivating a subscription.");
                }

                var subscription =
                    await _context.PlatformSubscriptions.IgnoreQueryFilters()
                        .SingleOrDefaultAsync(
                            s => s.Id ==
                            payment.SubscriptionId);

                if (subscription == null)
                {
                    _logger.LogWarning("Payment with and Id : {paymentId} does not relate to any Subscription", paymentId);

                    return false;
                }

                payment.PaymentStatus =
                   (int)status;

                payment.CompletedAt =
                    DateTime.UtcNow;

                subscription.IsActive = false;

                // tenant

                var tenant = await _context.Tenants.SingleOrDefaultAsync(t => t.TenantId == payment.TenantId);

                if (tenant == null) throw new Exception("no tenant is associated with this payment");

                tenant.IsActive = false;


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Faild to commit transaction ");

                return false;
            }
        }
        public async Task<bool> UpdateAsync(Payment Payment, PlatformSubscription PlatformSubscription)
        {

            using var Transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.PlatformSubscriptions.Update(PlatformSubscription);
                _context.PlatformPayments.Update(Payment);
                await _context.SaveChangesAsync();
                await Transaction.CommitAsync();


            }

            catch
            {
                Transaction.Rollback();

            }

            return true;



        }


        public override async Task<int> AddAsync(PlatformSubscription PlatformSubscription)
        {
            using var Transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Tenant tenant = await _context.Tenants.SingleOrDefaultAsync(t => t.TenantId == PlatformSubscription.TenantId);
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant with Id {TenantId} does not exist.", PlatformSubscription.TenantId);
                    throw new Exception($"Tenant with Id {PlatformSubscription.TenantId} does not exist.");
                }

                tenant.IsActive = true;
                tenant.HaveUsedTheFreeTry = true;
                _context.Tenants.Update(tenant);
                await _context.PlatformSubscriptions.AddAsync(PlatformSubscription);
                await _context.SaveChangesAsync();
                await Transaction.CommitAsync();
                return PlatformSubscription.Id;



            }
            catch
            {
                await Transaction.RollbackAsync();
                throw;
            }


        }


        public async Task<bool> ConfirmUpGradeWithQueryFiltersIgnoreAsync(int paymentId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // fetch and set payment info to success 

                Payment? payment = await _context.PlatformPayments.IgnoreQueryFilters()
                    .SingleOrDefaultAsync(p => p.Id == paymentId);
                if (payment == null)
                {
                    _logger.LogWarning("Payment with Id {paymentId} does not exist.", paymentId);
                    return false;
                }

                payment.PaymentStatus = (int)enGeneralState.Success;

                // fetch and disActivate the old subscription for the tenant if it exists
                var oldSubscription = await this.GetActiveByTenantIdAsync(payment.TenantId);
               
                // Deactivate the old subscription if it exists 
                if (oldSubscription != null)
                {
                    oldSubscription.IsActive = false;
                    _context.PlatformSubscriptions.Update(oldSubscription);
                }
                

                // fetch the new subscription related to the payment and activate it
                var subscription = await _context.PlatformSubscriptions.IgnoreQueryFilters()
                    .SingleOrDefaultAsync(s => s.Id == payment.SubscriptionId);
                if (subscription == null)
                {
                    _logger.LogWarning("Payment with Id {paymentId} does not relate to any Subscription.", paymentId);
                    return false;
                }
                subscription.IsActive = true;
                Tenant? tenant=await _context.Tenants.SingleOrDefaultAsync(t=>t.TenantId==payment.TenantId);
                if (tenant != null)
                {
                    tenant.IsActive = true;

                }

                

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to commit transaction for confirming upgrade.");
                return false;
            }
        }
      public async  Task<List<PlatformSubscription>> GetAllWhereIsNotRegisterdAsync()
        {
            try
            {
               return await _context.PlatformSubscriptions.Where(e => e.IsRegisterdToClientSubscription == false).ToListAsync();
                    


            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "faild to fetch Subscription list ");
                throw ex;
            }

           

        }

    }
}