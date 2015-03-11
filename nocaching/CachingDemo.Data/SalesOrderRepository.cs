﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CachingDemo.Data.Models;

namespace CachingDemo.Data
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        public async Task<ICollection<SalesOrderHeader>> GetTopTenSalesOrdersAsync()
        {
            using (var context = new AdventureWorksContext())
            {
                return await context.SalesOrders
                    .Include(soh => soh.Customer.Person)
                    .Include(soh => soh.SalesPerson)
                    .OrderByDescending(soh => soh.TotalDue)
                    .Take(10)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
        }
    }
}