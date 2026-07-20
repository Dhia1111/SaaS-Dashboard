using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DtoPayment {
    public int Id { get; set; }
    public int TenantId { get; set; }

    public enPaymentProviders Provider { get; set; }

    public int SubscriptionId { get; set; }
    public string? ProviderPaymentId { get; set; } 

    public enGeneralState PaymentStatus { get; set; }

    public string? CreatedAt { get; set; }

    public string? CompletedAt { get; set; }


    public decimal Amount {  get; set; }

    public string Currency { get; set; } = null!;

    



}

public interface IPaymentRepo : IGenericRepo<Payment> {
    Task<Payment?> GetLastPenddingPaymentAsync();
    Task<bool> HasPenddingPaymentAsync();
    Task<int> AddAsync(Payment Payment,PlatformSubscription PlatformSubscription);
    Task<IEnumerable< Payment>?> GetAllByStatusWithIgnoreQueryFiltersAsync(int status);
     Task<Payment?> GetByPaymentProviderId(string paymentProviderId);



}


namespace Connection.models
{
    public class clsPaymentRepo:GenericRepo<Payment>,IPaymentRepo
    {
        public clsPaymentRepo(ILogger<clsPaymentRepo>logger,SaasDashboardContext context) : base(context,logger)
        {


        }

        public async Task<Payment?> GetLastPenddingPaymentAsync()
        {

            return await _context.PlatformPayments.FirstOrDefaultAsync(i => i.PaymentStatus == (int)enGeneralState.Pending);

        }
        public async Task<bool> HasPenddingPaymentAsync()
        {

            return (await GetLastPenddingPaymentAsync()) != null;


        }

        public async Task<int> AddAsync(Payment Payment, PlatformSubscription PlatformSubscription)
        {

            using var Transaction=await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.PlatformSubscriptions.AddAsync(PlatformSubscription);
                await _context.SaveChangesAsync();
                await _context.PlatformPayments.AddAsync(Payment);
                await _context.SaveChangesAsync();

                await Transaction.CommitAsync();


            }
            
            catch
            {
                Transaction.Rollback();

            }

            return Payment.Id;



        }

        public async Task<IEnumerable<Payment>?> GetAllByStatusWithIgnoreQueryFiltersAsync(int status)
        {
           return   await  _context.PlatformPayments.AsNoTracking().IgnoreQueryFilters().
                           Where(p=>p.PaymentStatus==status).
                           ToListAsync();
         }

        public async Task<Payment?> GetByPaymentProviderId(string providerPaymentId)
        {
          return  await _context.PlatformPayments.SingleOrDefaultAsync(e => e.ProviderPaymentId == providerPaymentId);
        }



    }
}
