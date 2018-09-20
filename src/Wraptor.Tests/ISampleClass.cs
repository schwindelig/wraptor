namespace Wraptor.Tests
{
    public interface ISampleClass
    {
        int IntProperty { get; set; }
        string StringProperty { get; set; }
        int GetInt(int input);
        string GetString(string input);
        TInput GetT<TInput>(TInput input);
        T1 GetT1<T1, T2>(T1 input1, T2 input2);
        T2 GetT2<T1, T2>(T1 input1, T2 input2);
        int GetInt1(int input1, int input2);
        int GetInt2(int input1, int input2);
        string GetString1(string input1, string input2);
        string GetString2(string input1, string input2);
    }
}