using MRA.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MRA.DTO.ViewModels.Art
{
    public class SaveDrawingRequest
    {
        public bool IsEditing { get; set; }
        public string Id { get; set; }
        public string Path { get; set; }
        public string PathThumbnail { get; set; }
        public bool Visible { get; set; }
        public int Type { get; set; }
        public string TagsText { get; set; }
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string SpotifyUrl { get; set; }
        public string Title { get; set; }
        public string DateHyphen { get; set; }
        public int Software { get; set; }
        public int Paper { get; set; }
        public int Time { get; set; }
        public int ProductType { get; set; }
        public string ProductName { get; set; }
        public bool Favorite { get; set; }
        public string ReferenceUrl { get; set; }
        public int ScoreCritic { get; set; }
        public List<string> ListComments { get; set; }
        public List<string> ListCommentPros { get; set; }
        public List<string> ListCommentCons { get; set; }
    }
}
