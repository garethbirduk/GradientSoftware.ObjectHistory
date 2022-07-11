namespace Gradient.ObjectHistoryTest
{
    public class MyClass
    {
        public MyClass()
        {

        }

        public MyClass(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public int Value { get; set; }

        public string NameValue
        {
            get
            {
                return $"{Name}_{Value}";
            }
        }
    }
}
