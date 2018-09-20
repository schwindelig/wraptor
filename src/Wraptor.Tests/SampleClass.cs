namespace Wraptor.Tests
{
    public class SampleClass : ISampleClass
    {
        public int IntProperty { get; set; }

        public string StringProperty { get; set; }

        public int GetInt(int input) => input;

        public string GetString(string input) => input;

        public int GetInt1(int input1, int input2) => input1;

        public int GetInt2(int input1, int input2) => input2;
        public string GetString1(string input1, string input2) => input1;

        public string GetString2(string input1, string input2) => input2;

        public TInput GetT<TInput>(TInput input) => input;

        public T1 GetT1<T1, T2>(T1 input1, T2 input2) => input1;

        public T2 GetT2<T1, T2>(T1 input1, T2 input2) => input2;
    }

    public class SampleGenericClass : ISampleGenericClass<int, string>
    {
        public string GetCovariantResult(int input)
        {
            return input.ToString();
        }
    }
}
