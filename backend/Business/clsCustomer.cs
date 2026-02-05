

public interface ICustomerService {


    public List<DTOCustomer> GetAllCustomers();


}

namespace Business
{
    public class clsCustomer:ICustomerService
    {

        private readonly ICustomerRepo _customer ;
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Adress { get; set; }
        public string? phone { get; set; }
        
        public clsCustomer(ICustomerRepo _customer)
        {
            this._customer = _customer;
        }

        public List<DTOCustomer> GetAllCustomers()
        {
            return _customer.GetAllCustomers();
        }

    }
}
