using MRA.DTO.Enums.Drawing;
using System.ComponentModel.DataAnnotations;

namespace MRA.DTO.ViewModels.Art;

public class SaveDrawingRequest
{   
    public string Id { get; set; }

    [Required]
    public string? Path { get; set; }
    [Required]
    public string? PathThumbnail { get; set; }

    [Required]
    public bool Visible { get; set; }

    [Required]
    public DrawingTypes Type { get; set; }
    public string? TagsText { get; set; }
    public string? Name { get; set; }
    public string? ModelName { get; set; }
    public string? SpotifyUrl { get; set; }
    public string? Title { get; set; }

    [Required]
    public string DateHyphen { get; set; }

    [Required]
    public DrawingSoftwares Software { get; set; }

    [Required]
    public DrawingPaperSizes Paper { get; set; }

    public int? Time { get; set; }

    [Required]
    public DrawingProductTypes ProductType { get; set; }
    public string? ProductName { get; set; }

    [Required]
    public bool Favorite { get; set; }
    
    public string? ReferenceUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? BlueskyUrl { get; set; }

    [Required]
    public int ScoreCritic { get; set; }

    [Required]
    public DrawingFilterTypes Filter { get; set; }

    public List<string>? ListComments { get; set; }
    public List<string>? ListCommentsPros { get; set; }
    public List<string>? ListCommentsCons { get; set; }
    public List<string>? ListCommentsStyle { get; set; }
}
