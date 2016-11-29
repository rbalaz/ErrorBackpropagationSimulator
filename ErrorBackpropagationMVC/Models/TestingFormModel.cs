using System.ComponentModel.DataAnnotations;

namespace ErrorBackpropagationMVC.Models
{
    public class TestingFormModel
    {
        [Required(ErrorMessage = "Enter folder created when uploading data to server.")]
        [Display(Name = "Name of folder where training data is located")]
        public string folderName { get; set; }

        [Required(ErrorMessage = "Enter filename of the network file with suffix.")]
        [Display(Name = "Name of the network file")]
        public string fileName { get; set; }
    }
}