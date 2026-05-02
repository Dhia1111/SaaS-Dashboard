using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models.Entites
{
    public interface IEntityWithTenantId
    {
        int TenantId { get; set; }
    }
    public interface IEntity { 
    
        int Id { get; set; }
    }


}
