using System.ComponentModel.DataAnnotations;

namespace TraineeCamp.Models
{
    // Represents data for email and an uploaded file
    public class FileEmailModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [Display(Name = "Enter your email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please choose a file with the .docx extension")]
        [Display(Name = "Choose a file")]
        public IFormFile UploadedFile { get; set; }
    }
}
