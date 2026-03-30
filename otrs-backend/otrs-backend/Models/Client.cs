using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // np. "Hustletrack ITSM"

        public string? Description { get; set; }

        public string? City { get; set; }
        private string? _postalCode;
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Kod pocztowy musi być w formacie XX-XXX")]
        public string? PostalCode
        {
            get => _postalCode;
            set => _postalCode = string.IsNullOrWhiteSpace(value) ? null : value;
        }
        public string? Street { get; set; }
        public string? StreetNumber { get; set; }
        public string? ApartmentNumber { get; set; }

        private string? _phone;
        [Phone(ErrorMessage = "Niepoprawny format telefonu")]
        public string? Phone
        {
            get => _phone;
            set => _phone = string.IsNullOrWhiteSpace(value) ? null : value;
        }


        // Relacja: Jeden klient może mieć wiele kategorii
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}