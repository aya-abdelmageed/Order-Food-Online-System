using OrderFood.DAL.Entities.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.BLL.Interfaces
{
    public interface IPaymentService
    {
        public Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string UserId);
    }
}
