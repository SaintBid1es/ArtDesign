using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtDesign.Model
{
    /// <summary>
    /// Этап проекта
    /// </summary>
    [Table("WorkStages")]
    public class WorkStage : Base
    {
        [Required]
        [StringLength(30)]
        public string StageName { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
        [Column("Project_ID")]
        public int ProjectId { get; set; }
        [Column("Employee_ID")]
        public int EmployeeId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }
    }
}