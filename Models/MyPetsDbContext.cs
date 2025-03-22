using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyPets.Models
{
    public class MyPetsDbContext: DbContext
    {
        public MyPetsDbContext(DbContextOptions<MyPetsDbContext> options): base(options) {}

        public DbSet<MyPetsViewModel> MyPets {get; set;}
    }
}