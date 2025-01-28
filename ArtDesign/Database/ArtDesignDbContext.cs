using System.Data.Entity;
using ArtDesign.Model;

namespace ArtDesign.Database
{
    public class ArtDesignDbContext : DbContext
    {
        // Название connectionString в App.config
        public ArtDesignDbContext() : base("name=ARTPROJECTEntities")
        {
        }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<WorkStage> WorkStages { get; set; }
        public virtual DbSet<FileEntity> Files { get; set; }
    }
}