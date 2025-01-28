using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtDesign.Model
{
    /// <summary>
    /// Клиент
    /// </summary>
    [Table("Clients")]
    public class Client : Base
    {
        [Required]
        [StringLength(30)]
        public string Surname { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [StringLength(30)]
        public string Patronymic { get; set; }

        [Required]
        [StringLength(45)]
        public string Email { get; set; }

        public int Phone { get; set; }

        [StringLength(50)]
        public string HistoryOfProjects { get; set; }

        [Required]
        [StringLength(30)]
        public string NameCompany { get; set; }

        // Навигационное свойство – например, 
        // список заказанных проектов:
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}