
namespace SchoolPilot.Print.Attributes
{
    public class PrintTitleAttribute : Attribute
    {
        public string Title { get; set; }

        public PrintTitleAttribute(string title)
        {
            Title = title;
        }
    }
}
