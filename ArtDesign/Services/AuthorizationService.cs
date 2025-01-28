using System.Data.Entity;
using System.Threading.Tasks;
using ArtDesign.Database;
using ArtDesign.Model;

namespace ArtDesign.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly DataManager _dataManager;

        public AuthorizationService(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public async Task<Employee> AuthorizeAsync(string login, string password)
        {
            // Реальный запрос в БД
            var employee = await _dataManager.Employees.GetQueryable()
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Login == login && e.Password == password);

            return employee; // вернёт null, если не найден
        }
    }
}