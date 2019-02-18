using System.Collections.Generic;

namespace ConsoleAppBoilerPlate.Models
{
    public class GenericResults
    {
        public int SuccessCount { get; set; }

        public int ErrorCount { get; set; }

        public List<string> SuccessMessages { get; set; } = new List<string>();

        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
