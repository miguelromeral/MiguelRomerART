using Grpc.Net.Client.Balancer;
using MRA.Services;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
using Plugin.CloudFirestore;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Google.Protobuf;

namespace MR.MAUI
{
    public partial class MainPage : ContentPage
    {
        private DrawingService _drawingService;
        public List<string> SelectorTipos { get; set; }
        public List<string> SelectorProductTypes { get; set; }
        public List<string> SelectorSoftware { get; set; }
        public List<string> SelectorPaperSize { get; set; }
        public Drawing drawing { get; set; }

        public MainPage()
        {
            InitializeComponent();

            //var tmp = drawingService;

            _drawingService = Application.Current.MainPage
                .Handler
                .MauiContext
                .Services  // IServiceProvider
                .GetService<DrawingService>();

            drawing = new Drawing();
            SelectorTipos = Drawing.DRAWING_TYPES.Select(x => x.Value).ToList();
            SelectorProductTypes = Drawing.DRAWING_PRODUCT_TYPES.Select(x => x.Value).ToList();
            SelectorSoftware = Drawing.DRAWING_SOFTWARE.Select(x => x.Value).ToList();
            SelectorPaperSize = Drawing.DRAWING_PAPER_SIZE.Select(x => x.Value).ToList();

            BindingContext = this;
        }

        private async void OnFindDrawingId(object sender, EventArgs e)
        {
            // Acceder al valor del TextBox (Entry)
            string textoIngresado = tbDrawingId.Text;


            drawing = await _drawingService.FindDrawingById(textoIngresado);

            if(drawing == null)
            {
                // Mostrar la alerta
                await DisplayAlert("", "No se ha encontrado ningún dibujo con ID '" + textoIngresado + "'", "Vale");
                return;
            }

            DisplayAlert("¡Encontrado!", "Existe un dibujo con ID '" + textoIngresado + "'", "Vale");

            imageDrawing.Source = drawing.Url;
            imageDrawingThumbnail.Source = drawing.UrlThumbnail;
            if (DateTime.TryParseExact(drawing.Date, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out DateTime selectedDate))
            {
                // Asignar la fecha al DatePicker
                dpDrawingDate.Date = selectedDate;
            }
            sDrawingFavorite.IsToggled = drawing.Favorite;
            tbDrawingName.Text = drawing?.Name ?? "";
            tbDrawingModelName.Text = drawing?.ModelName ?? "";
            tbDrawingProductName.Text = drawing?.ProductName ?? "";
            tbDrawingReference.Text = drawing?.ReferenceUrl ?? "";
            tbDrawingTitle.Text = drawing?.Title ?? "";
            cbDrawingType.SelectedIndex = drawing.Type;
            cbDrawingSoftware.SelectedIndex = drawing.Software;
            cbDrawingProductType.SelectedIndex = drawing.ProductType;
            cbDrawingPaperSize.SelectedIndex = drawing.Paper;
        }


        //private void OnPickerSelectedIndexChangedType(object sender, EventArgs e)
        //{
        //    drawing.Type = ((Picker)sender).SelectedIndex;
        //}
        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            drawing.Date = e.NewDate.ToString("yyyy/MM/dd");
        }

        private void OnSaveDrawing(object sender, EventArgs e)
        {
            //drawing.ModelName = tbDrawingModelName.Text;
            //drawing.ProductName = tbDrawingProductName.Text;
            //drawing.Title = tbDrawingTitle.Text;
            //drawing.ReferenceUrl = tbDrawingReference.Text;
            //drawing.Type = cbDrawingType.SelectedIndex;

            //drawing.Favorite = sDrawingFavorite.IsToggled;

            //cbDrawingType.SelectedIndex = drawing.Type;
            //cbDrawingSoftware.SelectedIndex = drawing.Software;
            //cbDrawingProductType.SelectedIndex = drawing.ProductType;
            //cbDrawingPaperSize.SelectedIndex = drawing.Paper;

            Debug.WriteLine("--> " + drawing.Date);
        }
    }

}
