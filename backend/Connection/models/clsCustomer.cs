using Connection.Data;


public interface ICustomerRepo {

   

    public List<DTOCustomer> GetAllCustomers();


}

public class DTOCustomer {


    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Adress { get; set; }
    public string? phone { get; set; }


}


namespace Connection.models
{
    public partial class clsCustomerRepository :ICustomerRepo
    {
        private readonly SaasDashboardContext _context;
        public clsCustomerRepository(SaasDashboardContext context)
        {
            _context = context;

        }

       public List<DTOCustomer> GetAllCustomers()
        {
            if  (_context!=null&& _context.Customers != null)
            {
                return _context.Customers
                    .Select(c => new DTOCustomer
                    {
                        Id = c.Id,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Adress = c.Adress,
                        phone = c.phone
                    })
                    .ToList();
            }

            return [];
        }
        
    }
}
