using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.DAL.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDelete { get; set; } = false;
    }
}
