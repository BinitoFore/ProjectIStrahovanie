using CurseProject.Models;
using Microsoft.Extensions.Primitives;

namespace CurseProject.ViewModelsl
{
    public class AddPropertyViewModel
    {
        public static string clientName { get; set; }

        public static int amenitId { get; set; }
        public static int riskId { get; set; }
        public static Contract contract { get; set; }

        public static List<Property> properties { get; set; }
        public static List<Property> addingProperty { get; set; }
        public static List<int> indexes { get; set; }
    }
}
