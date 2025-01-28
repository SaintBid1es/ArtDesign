using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtDesign.Model
{
    /// <summary>
    /// Статус проекта
    /// </summary>
    [Table("Status")]
    public class Status : Base
    {
        [Required]
        [StringLength(30)]
        public string StatusName { get; set; }

        // Навигация
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}