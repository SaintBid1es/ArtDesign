using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtDesign.Model
{
    /// <summary>
    /// Файл, прикреплённый к проекту
    /// </summary>
    [Table("Files")]
    public class FileEntity : Base
    {
        [Required]
        [StringLength(30)]
        public string FileName { get; set; }

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; }
       
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }
    }
}