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

        public List<string> ListaComentarios { get; set; }
        public List<string> ListaComentariosPros { get; set; }
        public List<string> ListaComentariosCons { get; set; }
        public List<string> SelectAdd { get; set; }

        public List<string> ListaDrawingsId { get; set; }
        public List<Drawing> ListaDrawings { get; set; }

        public MainPage()
        {
            InitializeComponent();

            //var tmp = drawingService;

            _drawingService = Application.Current.MainPage
                .Handler
                .MauiContext
                .Services
                .GetService<DrawingService>();

            SelectAdd = new List<string>() { "Crear", "Editar" };

            drawing = new Drawing();
            SelectorTipos = Drawing.DRAWING_TYPES.Select(x => x.Value).ToList();
            SelectorProductTypes = Drawing.DRAWING_PRODUCT_TYPES.Select(x => x.Value).ToList();
            SelectorSoftware = Drawing.DRAWING_SOFTWARE.Select(x => x.Value).ToList();
            SelectorPaperSize = Drawing.DRAWING_PAPER_SIZE.Select(x => x.Value).ToList();

            ListaComentarios = new List<string>();
            ListaComentariosPros = new List<string>();
            ListaComentariosCons = new List<string>();

            LoadDrawingsId();

            BindingContext = this;

            cbSelectOperation.SelectedIndex = 0;
        }

        private async void LoadDrawingsId(string id = "")
        {
            ListaDrawings = (await _drawingService.GetAllDrawings()).OrderBy(x => x.Id).ToList();
            ListaDrawingsId = ListaDrawings.Select(x => x.ToString()).ToList();
            cbDrawingId.ItemsSource = ListaDrawingsId;
            if (!String.IsNullOrEmpty(id))
            {
                cbDrawingId.SelectedIndex = ListaDrawings.FindIndex(x => x.Id.Equals(id));
            }
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
            ActualizarDrawing();
        }

        private void ActualizarDrawing()
        {
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

            ListaComentarios = drawing.ListComments;
            ReloadComentarios();
            ListaComentariosPros = drawing.ListCommentPros;
            ReloadComentariosPros();
            ListaComentariosCons = drawing.ListCommentCons;
            ReloadComentariosCons();
        }

        private void ReloadComentarios() => ReloadStackLayout(slDrawingComments, ListaComentarios, 
            OnDeleteButtonClicked, comment_Unfocused, "Comentario");
        private void ReloadComentariosPros() => ReloadStackLayout(slDrawingCommentsPros, ListaComentariosPros, 
            OnDeleteButtonClickedPros, comment_Unfocused_pros, "Comentarios Positivos");
        private void ReloadComentariosCons() => ReloadStackLayout(slDrawingCommentsCons, ListaComentariosCons, 
            OnDeleteButtonClickedCons, comment_Unfocused_cons, "Comentario Negativos");

        private void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var index = IsValidDeleteEvent(sender, e);
            if (index > -1)
            {
                ListaComentarios.RemoveAt(index);
                ReloadComentarios();
            }
        }
        private void OnDeleteButtonClickedPros(object sender, EventArgs e)
        {
            var index = IsValidDeleteEvent(sender, e);
            if (index > -1)
            {
                ListaComentariosPros.RemoveAt(index);
                ReloadComentariosPros();
            }
        }
        private void OnDeleteButtonClickedCons(object sender, EventArgs e)
        {
            var index = IsValidDeleteEvent(sender, e);
            if (index > -1)
            {
                ListaComentariosCons.RemoveAt(index);
                ReloadComentariosCons();
            }
        }

        private int IsValidDeleteEvent(object sender, EventArgs e) {
            if (sender is Button button && button.CommandParameter is int index)
            {
                return index;
            }
            return -1;
        }

        private void OnAddComentario(object sender, EventArgs e)
        {
            ListaComentarios.Add("");
            ReloadComentarios();
        }
        private void OnAddComentarioPros(object sender, EventArgs e)
        {
            ListaComentariosPros.Add("");
            ReloadComentariosPros();
        }
        private void OnAddComentarioCons(object sender, EventArgs e)
        {
            ListaComentariosCons.Add("");
            ReloadComentariosCons();
        }

        private void ReloadStackLayout(StackLayout layout, List<string> list, EventHandler handler, 
            EventHandler<FocusEventArgs> handlerUnfocused, string placeholder)
        {
            layout.Children.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var comment = list[i];

                var entry = new Entry
                {
                    Text = comment,
                    Placeholder = placeholder
                };

                entry.BindingContext = i;
                entry.Unfocused += handlerUnfocused;

                var button = new Button
                {
                    Text = "🗑",
                    CommandParameter = i
                };

                button.Clicked += handler;

                var gridDrawingComments = new Grid();
                gridDrawingComments.Margin = new Thickness(0, 0, 0, 10);

                gridDrawingComments.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                gridDrawingComments.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                Grid.SetColumn(entry, 0);
                Grid.SetColumn(button, 1);


                gridDrawingComments.Children.Add(entry);
                gridDrawingComments.Children.Add(button);

                layout.Children.Add(gridDrawingComments);
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            drawing.Date = e.NewDate.ToString("yyyy/MM/dd");
        }

        private void comment_Unfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry &&  entry.BindingContext is int myInteger)
            {
                ListaComentarios[myInteger] = ((Entry)sender).Text;
            }
        }
        private void comment_Unfocused_pros(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry && entry.BindingContext is int myInteger)
            {
                ListaComentariosPros[myInteger] = ((Entry)sender).Text;
            }
        }
        private void comment_Unfocused_cons(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry && entry.BindingContext is int myInteger)
            {
                ListaComentariosCons[myInteger] = ((Entry)sender).Text;
            }
        }

        private async void OnSaveDrawing(object sender, EventArgs e)
        {
            btnSave.IsEnabled = false;

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

            //Debug.WriteLine("--> " + drawing.Date);

            try
            {
                if(drawing.Paper > 0)
                {
                    drawing.Paper += 3;
                }

                drawing.Comment = string.Join(Drawing.SEPARATOR_COMMENTS, ListaComentarios);
                drawing.CommentPros = string.Join(Drawing.SEPARATOR_COMMENTS, ListaComentariosPros);
                drawing.CommentCons = string.Join(Drawing.SEPARATOR_COMMENTS, ListaComentariosCons);

                await _drawingService.AddAsync(drawing);
                DisplayAlert("Actualziado", $"El dibujo con ID '{drawing.Id}' ha sido guardado con éxito.", "Vale");
                LoadDrawingsId(drawing.Id);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error al Actualizar", $"Ha ocurrido un error al guardar el dibujo con ID '{drawing.Id}'.\n{ex.Message}", "Vale");
            }
            btnSave.IsEnabled = true;
        }

        private void cbDrawingId_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex > -1)
            {
                drawing = ListaDrawings[selectedIndex];
                ActualizarDrawing();
            }
        }

        private void tbDrawingId_Unfocused(object sender, FocusEventArgs e)
        {
            var newValue = ((Entry)sender).Text;
            if (ListaDrawingsId.Count(x => x.Equals(newValue)) > 0)
            {
                DisplayAlert("ID Usado", $"Ya existe un dibujo con ID '{newValue}'.\nCambia el ID y vuelve a intentarlo", "Vale");
            }
        }

        private void cbSelectOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newIndex = ((Picker)sender).SelectedIndex;

            switch (newIndex)
            {
                case 0:
                    layoutAddDrawing.IsVisible = true;
                    layoutEditDrawing.IsVisible = false;
                    break;
                case 1:
                    layoutAddDrawing.IsVisible = false;
                    layoutEditDrawing.IsVisible = true;
                    break;
            }
        }
    }

}
