using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace StockMaster.Models
{
    [Index(nameof(SKU), IsUnique = true)]
    public class Producto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El SKU es obligatorio")]
        public string SKU { get; set; }
        [Required(ErrorMessage = "El precio es obligatorio")]
        public decimal Precio { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser un número positivo")]
        public int Stock { get; set; }
        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
    }
}
