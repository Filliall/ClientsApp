using SQLite;
using System.ComponentModel.DataAnnotations;

namespace ClientsApp.Models
{
    public class Client
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O sobrenome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O sobrenome não pode ter mais de 100 caracteres.")]
        public string LastName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        [Range(0, 130, ErrorMessage = "A idade deve ser um valor válido.")]
        public int Age { get; set; }
    }
}
