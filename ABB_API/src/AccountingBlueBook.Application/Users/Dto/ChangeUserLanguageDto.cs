using System.ComponentModel.DataAnnotations;

namespace AccountingBlueBook.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}