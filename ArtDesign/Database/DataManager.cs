using ArtDesign.Model;

namespace ArtDesign.Database
{
    /// <summary>
    /// Фасад (шлюз) для работы с репозиториями всех сущностей.
    /// </summary>
    public class DataManager
    {
        private readonly ArtDesignDbContext _context;
        
        public IRepository<Client> Clients { get; }
        public IRepository<Employee> Employees { get; }
        public IRepository<Role> Roles { get; }
        public IRepository<Status> Statuses { get; }
        public IRepository<Project> Projects { get; }
        public IRepository<WorkStage> WorkStages { get; }
        public IRepository<FileEntity> Files { get; }

        /// <summary>
        /// В конструкторе инициализируем EF-контекст и репозитории.
        /// </summary>
        public DataManager(ArtDesignDbContext context)
        {
            _context = context;

            // Под капотом создаём BaseRepository с TEntity
            Clients = new BaseRepository<Client>(_context);
            Employees = new BaseRepository<Employee>(_context);
            Roles = new BaseRepository<Role>(_context);
            Statuses = new BaseRepository<Status>(_context);
            Projects = new BaseRepository<Project>(_context);
            WorkStages = new BaseRepository<WorkStage>(_context);
            Files = new BaseRepository<FileEntity>(_context);
        }
    }
}