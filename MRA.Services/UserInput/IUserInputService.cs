namespace MRA.Services.UserInput;

public interface IUserInputService
{
    string ReadStringValue(string prompt);
    bool ReadBoolValue(bool isNew, bool previous, string field);
    bool ReadBoolValue(string prompt);
    void ReadKey();
}
