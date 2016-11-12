using System.ComponentModel.DataAnnotations;

namespace ErrorBackpropagationMVC.Models
{
    public class TestingFormModel
    {
        [Required(ErrorMessage = "Enter folder created when uploading data to server.")]
        [Display(Name = "Name of folder where training data is located")]
        public string folderName { get; set; }

        [Required(ErrorMessage = "Enter filename of the training data.")]
        [Display(Name = "Name of the training data file")]
        public string fileName { get; set; }
    }
}