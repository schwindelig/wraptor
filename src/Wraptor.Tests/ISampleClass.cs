namespace Wraptor.Tests
{
    public interface ISampleClass
    {
        int IntProperty { get; set; }
        string StringProperty { get; set; }
        int GetInt(int input);
        string GetString(string input);
    }
}