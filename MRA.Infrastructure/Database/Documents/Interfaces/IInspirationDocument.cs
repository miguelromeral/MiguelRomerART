
namespace MRA.Infrastructure.Database.Documents.Interfaces;

public interface IInspirationDocument : IDocument
{
    string Id { get; set; }
    string Name { get; set; }
    string Instagram { get; set; }
    string Twitter { get; set; }
    string YouTube { get; set; }
    string Twitch { get; set; }
    string Pinterest { get; set; }
    int Type { get; set; }
}
