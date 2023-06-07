using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace DovizKur.Models
{
    public class DovizKurContext : DbContext
    {
        public DbSet<DovizKuru> Kurlar { get; set; }
    }
}
