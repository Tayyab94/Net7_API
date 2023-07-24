using System.ComponentModel.DataAnnotations;

namespace LearnAPI_Net7.Models.ViewModels
{
    public class CreateCustomreVM
    {
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }

    }
}
