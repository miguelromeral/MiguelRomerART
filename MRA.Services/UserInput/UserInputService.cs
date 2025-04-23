using MRA.Infrastructure.UserInput;
using System.ComponentModel;

namespace MRA.Services.UserInput;

public class UserInputService : IUserInputService
{
    private readonly IUserInputProvider _provider;

    public UserInputService(IUserInputProvider provider)
    {
        _provider = provider;
    }

    public string ReadStringValue(string prompt) => _provider.ReadStringValue(prompt);

    public bool ReadBoolValue(bool isNew, bool previous, string field) => _provider.FillBoolValue(isNew, previous, field);
    public bool ReadBoolValue(string prompt) => _provider.FillBoolValue(prompt);

    public void ReadKey() => _provider.ReadKey();
}
