using Microsoft.AspNetCore.Mvc;
using Moq;
using MRA.DTO.Models;
using MRA.DTO.ViewModels.Art;

namespace MRA.WebApi.Tests.Controllers.Art.Drawing.Filter;

public class DrawingControllerFilterTessts : DrawingControllerFilterBaseTests
{
    public DrawingControllerFilterTessts() : base(onlyIfVisible: true)
    {
    }


    //[Fact]
    //public async Task Filter_Ok()
    //{
    //    await Base_Filter_Ok();
    //}
    //protected override IEnumerable<DrawingModel> Generate_Ok_Drawings()
    //{
    //    return new List<DrawingModel>()
    //    {
    //        new DrawingModel
    //        {
    //            Id = "1",
    //            Name = "Name",
    //            ModelName = "ModelName",
    //            Visible = true
    //        }
    //    };
    //}

    protected override DrawingFilter Generate_Ok_Filter()
    {
        return new DrawingFilter()
        {
            OnlyVisible = onlyIfVisible
        };
    }

    //protected override async Task<ActionResult<FilterResults>> MockDrawingFilter(DrawingFilter filter, FilterResults expectedResults)
    //{
    //    _mockAppService
    //        .Setup(s => s.FilterDrawingsAsync(filter))
    //        .ReturnsAsync(expectedResults);

    //    return await FetchFilter(filter);
    //}

    //protected override void Assert_Filter_Ok(IEnumerable<DrawingModel> expected, FilterResults response)
    //{
    //    throw new NotImplementedException();
    //}

    [Fact]
    public async Task Filter_Error_InternalError()
    {
        await Base_Filter_Error_InternalError();
    }

    protected override async Task<ActionResult<FilterResults>> FetchFilter(DrawingFilter filter)
    {
        return await _controller.Filter(filter);
    }
}
