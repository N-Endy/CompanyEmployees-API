using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects
{
    public abstract record CompanyForManipulationDto
    {
        [Required(ErrorMessage = "Company name is a required field")]
        [MaxLength(40, ErrorMessage = "Maximum length for company name is 40 characters")]
        public string? Name { get; init; }

        [Required(ErrorMessage = "Company address is a required field")]
        [MaxLength(60, ErrorMessage = "Maximum length for company address is 60 characters")]
        public string? Address { get; init; }

        [Required(ErrorMessage = "Country is a required field")]
        [MaxLength(30, ErrorMessage = "Maximum length for company country is 30 characters")]
        public string? Country { get; init; }
    }
}