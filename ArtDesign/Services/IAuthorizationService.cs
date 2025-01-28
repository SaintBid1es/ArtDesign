using System.Threading.Tasks;
using ArtDesign.Model;

namespace ArtDesign.Services
{
    public interface IAuthorizationService
    {
        /// <summary>
        /// Проверяет логин и пароль, возвращает сотрудника (с ролью), если найден, иначе null.
        /// </summary>
        Task<Employee> AuthorizeAsync(string login, string password);
    }

}