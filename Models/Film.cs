using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filmothèque.Models
{
    public class Film
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du film est requis.")]
        [StringLength(100, ErrorMessage = "Le titre ne doit pas dépasser 100 caractères.")]
        [Display(Name = "Titre")]
        public string Title { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Date de sortie")]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "Le genre est requis.")]
        [Display(Name = "Genre")]
        public GenreFilm Genre { get; set; }

        [Range(0, 5, ErrorMessage = "La note doit être comprise entre 0 et 5.")]
        [Display(Name = "Note (/5)")]
        public double Rating { get; set; }

        [Range(0.01, 1000.00, ErrorMessage = "Le prix doit être positif.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Prix (€)")]
        public decimal Price { get; set; }

        [Range(0, 10000, ErrorMessage = "La quantité doit être positive ou nulle.")]
        [Display(Name = "Quantité en stock")]
        public int StockQuantity { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; } = "/images/default-movie.jpg";

        [StringLength(1000, ErrorMessage = "La description ne doit pas dépasser 1000 caractères.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;
    }
}