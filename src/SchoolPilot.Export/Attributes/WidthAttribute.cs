

namespace SchoolPilot.Export.Attributes
{
    public class WidthAttribute : Attribute
    {
        public double Width { get; private set; }

        public WidthAttribute(double width)
        {
            Width = width;
        }
    }
}
