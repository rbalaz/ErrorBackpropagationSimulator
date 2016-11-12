using System.ComponentModel.DataAnnotations;

namespace ErrorBackpropagationMVC.Models
{
    public class LearningFormModel
    {
        [Required(ErrorMessage = "Set learning parameter with values from (0.01,10)")]
        [Display(Name = "Learning parameter of neural network")]
        [Range(0.01, 10)]
        public double learningParameter { get; set; }

        [Required(ErrorMessage = "Example: 5 15")]
        [Display(Name = "Enter number of neurons in input and hidden layers, separated by blank space")]
        public string layers { get; set; }

        [Required(ErrorMessage = "Enter folder created when uploading data to server.")]
        [Display(Name = "Name of folder where training data is located")]
        public string folderName { get; set; }

        [Required(ErrorMessage = "Enter filename of the training data.")]
        [Display(Name = "Name of the training data file")]
        public string fileName { get; set; }

        [Required(ErrorMessage = "Enter error tolerance for classification.")]
        [Display(Name = "Tolerance of error used in learning")]
        [Range(0.001, 0.1)]
        public double errorTolerance { get; set; }

        [Required(ErrorMessage = "Training goal needs to be set.")]
        [Display(Name = "Required success rate for learning in %")]
        [Range(1, 99)]
        public double successRate { get; set; }
    }
}