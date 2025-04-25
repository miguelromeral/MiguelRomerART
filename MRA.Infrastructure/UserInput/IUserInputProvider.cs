namespace MRA.Infrastructure.UserInput;

public interface IUserInputProvider
{
    string ReadStringValue(string prompt);

    bool FillBoolValue(bool isNew, bool previous, string field);
    bool FillBoolValue(string field);

    void ReadKey();
}
