using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MRA.DTO.Firebase.Models;
using MRA.DTO.Logger;
using MRA.DTO.ViewModels.Art.Select;
using MRA.Services;
using MRA.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Test.Controllers
{
    public class ArtControllerTest : TestBase
    {
        private readonly ArtController _controller;
        private readonly Mock<IDrawingService> _drawingServiceMock;
        private readonly Mock<ILogger<ArtController>> _loggerMock;

        public ArtControllerTest()
        {
            // Crea un mock de IDrawingService y ILogger
            _drawingServiceMock = new Mock<IDrawingService>();
            _loggerMock = new Mock<ILogger<ArtController>>();

            // Crea el controlador usando los mocks
            _controller = new ArtController(_loggerMock.Object, _drawingServiceMock.Object);
        }

        [Fact]
        public async Task DrawingProducts_ReturnsOkResult_WithProductList()
        {
            // Configura el mock de IDrawingService para que devuelva datos de prueba
            var drawings = new List<Drawing> { new Drawing(), new Drawing() }; // Datos de prueba para "drawings"
            var products = new List<ProductListItem>
            {
                new ProductListItem { ProductName = "Product 1", ProductType = "Type", ProductTypeId = 1 },
                new ProductListItem { ProductName = "Product 2", ProductType = "Type", ProductTypeId = 1 },
            }; // Datos de prueba para "products"

            // Configura los métodos que serán llamados en el servicio mock
            _drawingServiceMock.Setup(ds => ds.GetAllDrawings()).ReturnsAsync(drawings);
            _drawingServiceMock.Setup(ds => ds.GetProducts(drawings)).Returns(products);

            // Actúa: llama al método del controlador
            var result = await _controller.DrawingProducts();

            // Asegura que el resultado es de tipo OkObjectResult con la lista de productos
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsType<List<ProductListItem>>(okResult.Value);

            Assert.Equal(products.Count, returnedProducts.Count); // Comprueba la cantidad de productos
        }
    }
}
