using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtDesign.Model
{
    /// <summary>
    /// Сотрудник
    /// </summary>
    [Table("Employees")]
    public class Employee : Base
    {
        [Required]
        [StringLength(30)]
        public string Surname { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(30)]
        public string Patronymic { get; set; }

        [Required]
        [StringLength(30)]
        public string Login { get; set; }

        [Required]
        [StringLength(30)]
        public string Password { get; set; }
        [Column("Role_ID")]
        public int RoleId { get; set; }

        // Навигационное свойство
        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }

        // Навигационное свойство
        public virtual ICollection<WorkStage> WorkStages { get; set; } = new List<WorkStage>();
    }
}