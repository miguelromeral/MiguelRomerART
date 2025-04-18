﻿using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art;

namespace MRA.WebApi.Models.Responses
{
    public class DrawingFilterResultsResponse : FilterResults
    {
        public new IEnumerable<DrawingModel> FilteredDrawings { get; set; }
        public new int FetchedCount { get { return (FilteredDrawings != null ? FilteredDrawings.Count() : 0); } }


        public DrawingFilterResultsResponse(FilterResults results)
        {
            this.TotalDrawings = results.TotalDrawings;
            this.FilteredCollections = results.FilteredCollections;
            this.FilteredDrawingCharacters = results.FilteredDrawingCharacters;
            this.FilteredDrawingModels = results.FilteredDrawingModels;
            this.NDrawingFavorites = results.NDrawingFavorites;
            this.FilteredDrawingPapers = results.FilteredDrawingPapers;
            this.FilteredDrawingProducts = results.FilteredDrawingProducts;
            this.FilteredDrawingProductTypes = results.FilteredDrawingProductTypes;
            this.FilteredDrawings = results.FilteredDrawings;
            this.FilteredDrawingSoftwares = results.FilteredDrawingSoftwares;
            this.FilteredDrawingStyles = results.FilteredDrawingStyles;
        }
    }
}
