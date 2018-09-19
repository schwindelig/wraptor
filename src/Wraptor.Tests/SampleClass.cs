namespace Wraptor.Tests
{
    public class SampleClass : ISampleClass
    {
        public int IntProperty { get; set; }

        public string StringProperty { get; set; }

        public int GetInt(int input) => input;

        public string GetString(string input) => input;

        public TInput GetT<TInput>(TInput input) => input;
    }
}
