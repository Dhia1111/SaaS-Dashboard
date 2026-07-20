using Business;
using Business.Config;
using Business.Exceptions;
using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedDto_Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DtoAddPayment {

    [Required]
    public int  PlatFormPlanId { get; set; } 

    [Required]
    public int PricingOptionId { get; set; }

    [Required]

    public DtoPayment  Payment { get; set; } = null!;

    public int  TenantId { get; set; } 
}


public interface IPaymentService : IGenericService<DtoPayment> {

     public bool ValidateCurrencySupport(DtoTenantPricingOption PriceOption);

    public  Task<DtoPayment?> GetLastPendingPaymentAsync();

    public Task<IEnumerable<DtoPayment>?> GetAllByStatusWithIgnoreQueryFiltersAsync(enGeneralState state);

    public Task<DtoPayment?> GetByPaymentProviderIdAsync(string paymentProviderId);
    public Payment GetEntity(DtoPayment payment);

}


namespace Business
{
    public class clsPaymentService : GenericService<DtoPayment, Payment>, IPaymentService
    {
        private readonly IPaymentRepo _paymentRepo;
        private readonly PlatformInfo _platformInfo;

        public clsPaymentService(IPaymentRepo paymentRepo, ILogger<clsPaymentService> logger,
             IOptions<PlatformInfo>platformInfo
            ) : base(paymentRepo, logger)
        {
            _paymentRepo = paymentRepo;    
            _platformInfo = platformInfo.Value;
    
            
        }

        protected override DtoPayment ToDto(Payment payment)
        {
            return new DtoPayment { 
            
                Id = payment.Id,
                TenantId = payment.TenantId,
                PaymentStatus=(enGeneralState)payment.PaymentStatus,
                 Provider = (enPaymentProviders)payment.Provider,
                ProviderPaymentId=payment.ProviderPaymentId,
                CreatedAt=payment.CreatedAt.ToString(),
                CompletedAt=payment.CompletedAt.ToString(),
                Amount = payment.Amount,
                Currency = payment.Currency,
                 SubscriptionId=payment.SubscriptionId



            };


        }

        protected override Payment FromDto(DtoPayment payment)
        {
            return new Payment {

                Id = payment.Id,
                TenantId = payment.TenantId,
                PaymentStatus = (int)payment.PaymentStatus,
                Provider = (int)payment.Provider,
                ProviderPaymentId = payment.ProviderPaymentId,
                CreatedAt = DateTime.Parse(payment.CreatedAt).ToUniversalTime(),
                CompletedAt = DateTime.TryParse(payment.CompletedAt,out DateTime pd)?pd.ToUniversalTime():null,
                Amount=payment.Amount,
                Currency=payment.Currency,
                 SubscriptionId=payment.SubscriptionId
            };

        }

        public Payment GetEntity(DtoPayment payment)
        {
            return FromDto(payment);
        }

        public bool ValidateCurrencySupport(DtoTenantPricingOption PriceOption )
        {
            if (PriceOption.Currency.ToLower() != _platformInfo.DefaultCurrency.ToLower()) throw new ArgumentException("Currency is not Supported ");

          
            return true;
        }


        private  decimal CalcTaxes()
        {
            return 0;
        }
        
        public async Task<DtoPayment?> GetLastPendingPaymentAsync()
        {
            var res= await _paymentRepo.GetLastPenddingPaymentAsync();
            return res != null ? ToDto(res) : null;

        }

        public async Task<IEnumerable<DtoPayment>?> GetAllByStatusWithIgnoreQueryFiltersAsync(enGeneralState state)
        {
            var res= await _paymentRepo.GetAllByStatusWithIgnoreQueryFiltersAsync((int)state);
            return res != null? res.Select(e=>ToDto(e)):null;


        }


        public async Task<DtoPayment?> GetByPaymentProviderIdAsync(string paymentProviderId)
        {
            var res = await _paymentRepo.GetByPaymentProviderId(paymentProviderId);
            return res!=null? ToDto(res) : null;

        }





    }

}
