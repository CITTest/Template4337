using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Template4337
{
    public class class1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string View { get; set; }
        public string Code { get; set; }
        public int Price { get; set; }
        public int Group { get; set; }

        public class1(string  name, string view, string code, int price)
        {
            Name = name;
            View = view;
            Code = code;
            Price = price;

            if (Price < 351) Group = 1;
            if (Price > 350 && Price < 800) Group = 2;
            if (Price > 800) Group = 3;
        }
    }
    public partial class Context : DbContext
    {
        

        public virtual DbSet<class1> Class1s { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=isrpo3;Trusted_Connection=True;");
            }
        }
        public Context() => Database.EnsureCreated();
    }
    }
