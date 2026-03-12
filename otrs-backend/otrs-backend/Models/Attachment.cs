using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Models
{
    public class Attachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } // np. "projekt.pdf"

        [Required]
        public string FilePath { get; set; } // np. "Uploads/guid_projekt.pdf"

        public string ContentType { get; set; } // np. "application/pdf"

        public long FileSize { get; set; } // w bajtach

        // Relacja: Załącznik należy do jednego komentarza
        public int CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}