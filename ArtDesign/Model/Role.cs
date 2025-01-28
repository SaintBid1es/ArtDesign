using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtDesign.Model
{
    /// <summary>
    /// Роль
    /// </summary>
    [Table("Roles")]
    public class Role : Base
    {
        [Required]
        [StringLength(30)]
        public string RoleName { get; set; }

        // Навигационное свойство
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}