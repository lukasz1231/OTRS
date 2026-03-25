using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Models
{
    public class Attachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        public string ContentType { get; set; }

        public long FileSize { get; set; }

        public int CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}