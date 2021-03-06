using BMBOT.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace BMBOT
{
    public class ApplicationContext: DbContext
    {
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=BMBOTConfiguration;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        public DbSet<BOTProfile> BOTProfile { get; set; }
        public DbSet<BOTProfileTemplateMapping> BOTProfileTemplateMapping { get; set; }
        public DbSet<BOTTemplate> BOTTemplate { get; set; }
        public DbSet<BOTTemplateColumns> BOTTemplateColumns { get; set; }
        public DbSet<BOTTemplateValidationMapping> BOTTemplateValidationMapping { get; set; }

        public DbSet<BOTFileLocationConfiguration> BOTFileLocationConfiguration { get; set; }
    }
}
