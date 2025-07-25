

namespace SchoolPilot.Print.Model
{
    public class PrintSchema<T>
    {
        public BasicPrintHeader HeaderInfo { get; set; }

        public T Model { get; set; }

        public FooterInfo Footer { get; set; }
    }
}
