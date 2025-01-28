using ArtDesign.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtDesign.Model
{
    /// <summary>
    /// Проект
    /// </summary>
    [Table("Project")]
    public class Project : Base
    {
        [Column("Client_ID")]
        public int ClientId { get; set; }

        [Required]
        [StringLength(30)]
        [Searchable]
        public string ProjectName { get; set; }

        [Required]
        [StringLength(150)]
        public string Description { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
        [Column("Status_ID")]
        public int StatusId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public virtual Client Client { get; set; }

        [ForeignKey(nameof(StatusId))]
        public virtual Status Status { get; set; }

        // Навигация
        public virtual ICollection<WorkStage> WorkStages { get; set; } = new List<WorkStage>();
        public virtual ICollection<FileEntity> Files { get; set; } = new List<FileEntity>();
    }
}