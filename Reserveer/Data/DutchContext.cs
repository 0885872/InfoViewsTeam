using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reserveer.Models;

namespace Reserveer.Data
{
    public class DutchContext : DbContext
    {
        public DutchContext(DbContextOptions<DutchContext> options) : base(options)
        {

        }

        public DbSet<User> user { get; set; }
    }
}
