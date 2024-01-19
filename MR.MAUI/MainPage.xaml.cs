using Grpc.Net.Client.Balancer;
using MRA.Services;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
using Plugin.CloudFirestore;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Google.Protobuf;
using MRA.Services.AzureStorage;
using MR.MAUI.Classes;

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
        public AzureBlobInfo azureBlobInfo { get; set; }

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

            SelectorTipos = Drawing.DRAWING_TYPES.Select(x => x.Value).ToList();
            SelectorProductTypes = Drawing.DRAWING_PRODUCT_TYPES.Select(x => x.Value).ToList();
            SelectorSoftware = Drawing.DRAWING_SOFTWARE.Select(x => x.Value).ToList();
            SelectorPaperSize = Drawing.DRAWING_PAPER_SIZE.Select(x => x.Value).ToList();

            LoadDrawingsId();
            LimpiarDatos();
        }

        private void btnLimpiar_Clicked(object sender, EventArgs e) => LimpiarDatos();

        private void LimpiarDatos()
        {
            cbDrawingId.SelectedIndex = -1;
            drawing = new Drawing()
            {
                UrlBase = _drawingService.GetAzureUrlBase()
            };

            azureBlobInfo = new AzureBlobInfo()
            {
                ThumbnailSize = 350
            };

            barcodeImage.Source = "";
            cbSelectOperation.SelectedIndex = 0;

            tbDrawingId.Text = "";
            tbCollectionId.Text = "";

            imageDrawing.Source = "";
            imageDrawingThumbnail.Source = "";

            tbDrawingTags.Text = "";
            tbDrawingAzureUrl.Text = "";
            outputText.Text = "";
            tbDrawingTitle.Text = "";
            sDrawingFavorite.IsToggled = false;
            tbDrawingName.Text = "";
            tbDrawingModelName.Text = "";
            cbDrawingType.SelectedIndex = 0;
            cbDrawingSoftware.SelectedIndex = 0;
            cbDrawingPaperSize.SelectedIndex = 0;
            tbDrawingScore.Text = "";
            tbDrawingTime.Text = "";
            cbDrawingProductType.SelectedIndex = 0;
            tbDrawingProductName.Text = "";
            tbDrawingSpotifyUrl.Text = "";


            ListaComentarios = new List<string>();
            ReloadComentarios();

            ListaComentariosPros = new List<string>();
            ReloadComentariosPros();

            ListaComentariosCons = new List<string>();
            ReloadComentariosCons();

            tbDrawingReference.Text = "";

            btnSave.IsEnabled = true;

            BindingContext = this;
        }

        private async void LoadDrawingsId(string id = "")
        {
            ListaDrawings = (await _drawingService.GetAllDrawings()).OrderBy(x => x.Id).ToList();
            ListaDrawingsId = ListaDrawings.Select(x => x.ToString()).ToList();
            cbDrawingId.ItemsSource = ListaDrawingsId;
            if (!String.IsNullOrEmpty(id))
            {
                cbSelectOperation.SelectedIndex = 1;
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
            tbDrawingAzureUrl.Text = drawing.Path ?? "";
            tbDrawingTitle.Text = drawing?.Title ?? "";
            cbDrawingType.SelectedIndex = drawing.Type;
            cbDrawingSoftware.SelectedIndex = drawing.Software;
            cbDrawingProductType.SelectedIndex = drawing.ProductType;
            tbDrawingSpotifyUrl.Text = drawing.SpotifyUrl ?? "";
            cbDrawingPaperSize.SelectedIndex = drawing.Paper;

            tbDrawingTags.Text = String.Join(" ", drawing.Tags);
            
            ListaComentarios = drawing.ListComments;
            ReloadComentarios();
            ListaComentariosPros = drawing.ListCommentPros;
            ReloadComentariosPros();
            ListaComentariosCons = drawing.ListCommentCons;
            ReloadComentariosCons();
        }

        private void ReloadComentarios()
        {
            //drawing.Comment = string.Join(Drawing.SEPARATOR_COMMENTS, ListaComentarios);
            //ListaComentarios = drawing.ListComments;
            ReloadStackLayout(slDrawingComments, ListaComentarios,
            OnDeleteButtonClicked, comment_Unfocused, "Comentario", 1);
        }
        private void ReloadComentariosPros()
        {
            //drawing.CommentPros = string.Join(Drawing.SEPARATOR_COMMENTS, ListaComentariosPros);
            //ListaComentariosPros = drawing.ListCommentPros;
            ReloadStackLayout(slDrawingCommentsPros, ListaComentariosPros,
            OnDeleteButtonClickedPros, comment_Unfocused_pros, "Comentarios Positivos", 2);
        }
        private void ReloadComentariosCons()
        {
            //drawing.CommentCons = string.Join(Drawing.SEPARATOR_COMMENTS, ListaComentariosCons);
            //ListaComentariosCons = drawing.ListCommentCons;
            ReloadStackLayout(slDrawingCommentsCons, ListaComentariosCons,
            OnDeleteButtonClickedCons, comment_Unfocused_cons, "Comentario Negativos", 3);
        }

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

        private void EntryComments_Unfocused(object sender, CommentEventArgs e)
        {
            Editor entry = (Editor) sender;
            int index = -1;
            StackLayout layout = slDrawingComments;
            switch (e.TipoLista)
            {
                case 1: 
                    index = e.Indice;
                    if (index > -1 && index < slDrawingComments.Children.Count)
                    {
                        ListaComentarios[index] = entry.Text;
                    }
                    break;
                case 2:
                    index = e.Indice;
                    if (index > -1 && index < slDrawingCommentsPros.Children.Count)
                    {
                        ListaComentariosPros[index] = entry.Text;
                    }
                    break;
                case 3:
                    index = e.Indice;
                    if (index > -1 && index < slDrawingCommentsCons.Children.Count)
                    {
                        ListaComentariosCons[index] = entry.Text;
                    }
                    break;
            }
        }


        private void OnAddComentario(object sender, EventArgs e)
        {
            ListaComentarios.Add(string.Empty);
            ReloadComentarios();
        }
        private void OnAddComentarioPros(object sender, EventArgs e)
        {
            ListaComentariosPros.Add(string.Empty);
            ReloadComentariosPros();
        }
        private void OnAddComentarioCons(object sender, EventArgs e)
        {
            ListaComentariosCons.Add(string.Empty);
            ReloadComentariosCons();
        }

        private void ReloadStackLayout(StackLayout layout, List<string> list, EventHandler handler, 
            EventHandler<FocusEventArgs> handlerUnfocused, string placeholder, int type)
        {
            layout.Children.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var comment = list[i];

                var entry = new Editor
                {
                    Text = comment,
                    Placeholder = placeholder,
                };
                entry.HeightRequest = 5 * entry.FontSize;
                int currentIteration = i;
                entry.Unfocused += (s, e) =>
                {
                    EntryComments_Unfocused(s, new CommentEventArgs(type, currentIteration));
                };
                entry.BindingContext = i;
                entry.Unfocused += handlerUnfocused;

                var button = new Button
                {
                    Text = "🗑",
                    CommandParameter = i
                };

                entry.SetDynamicResource(Entry.StyleProperty, "mrEntry");

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
            drawing.Date = ParseDate(e.NewDate);
        }

        private string ParseDate(DateTime date) => date.ToString("yyyy/MM/dd");

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
                if (String.IsNullOrEmpty(drawing.Id))
                {
                    DisplayAlert("Error al Guardar", $"El dibujo necesita un ID", "Vale");
                    btnSave.IsEnabled = true;
                    return;
                }


                drawing.Paper = cbDrawingPaperSize.SelectedIndex;


                drawing.Date = ParseDate(dpDrawingDate.Date);

                drawing.Tags = tbDrawingTags.Text.Split(" ").ToList();

                drawing.Comment = string.Join(Drawing.SEPARATOR_COMMENTS, ListaComentarios);
                drawing.CommentPros = string.Join(Drawing.SEPARATOR_COMMENTS, ListaComentariosPros);
                drawing.CommentCons = string.Join(Drawing.SEPARATOR_COMMENTS, ListaComentariosCons);

                drawing.ProductName = tbDrawingProductName.Text ?? "";
                drawing.ModelName = tbDrawingModelName.Text ?? "";
                drawing.Name = tbDrawingName.Text ?? "";

                drawing.SpotifyUrl = tbDrawingSpotifyUrl.Text ?? "";

                await _drawingService.AddAsync(drawing);
                DisplayAlert("Actualizado", $"El dibujo con ID '{drawing.Id}' ha sido guardado con éxito.", "Vale");
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
            if (ListaDrawings.Count(x => x.Id.Equals(newValue)) > 0)
            {
                tbDrawingId.Text = "";
                DisplayAlert("ID Usado", $"Ya existe un dibujo con ID '{newValue}'.\nCambia el ID y vuelve a intentarlo", "Vale");
            }
            else
            {
                drawing.Id = newValue;
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

        private async void SelectBarcode(object sender, EventArgs e)
        {
            var images = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Pick Barcode/QR Code Image",
                FileTypes = FilePickerFileType.Images
            });
            if(images != null)
            {
                var imageSource = images.FullPath.ToString();
                azureBlobInfo.LocalPath = imageSource;
                barcodeImage.Source = imageSource;
                outputText.Text = imageSource;
            }
        }

        private async void btnAzureUpload_Clicked(object sender, EventArgs e)
        {
            
                try
                {
                    btnAzureUpload.IsEnabled = false;
                    var blobLocation = tbDrawingAzureUrl.Text;
                    var blobLocationThumbnail = _drawingService.CrearThumbnailName(blobLocation);
                    drawing.PathThumbnail = blobLocationThumbnail;
                    await _drawingService.RedimensionarYGuardarEnAzureStorage(azureBlobInfo.LocalPath, blobLocation, 0);
                    await _drawingService.RedimensionarYGuardarEnAzureStorage(azureBlobInfo.LocalPath, blobLocationThumbnail, azureBlobInfo.ThumbnailSize);
                    DisplayAlert("Blob subido con éxito", $"Blob subido a Azure con éxito.", "Vale");
                    layoutAzureDetails.IsVisible = false;
                    imageDrawing.Source = drawing.Url;
                    imageDrawingThumbnail.Source = drawing.UrlThumbnail;
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error al Subir Blob", $"Ha ocurrido un error al guardar el blob en Azure.\n{ex.Message}", "Vale");
                }
                btnAzureUpload.IsEnabled = true;
        }

        private async void tbDrawingAzureUrl_Unfocused(object sender, FocusEventArgs e)
        {
            var path = ((Entry)sender).Text;
            var existe = await _drawingService.ExistsBlob(path);
            if (!existe)
            {
                DisplayAlert("No Blob Azure", $"La ruta especificada no pertenece a ningún Blob de Azure. Puede crear uno.", "Vale");
                layoutAzureDetails.IsVisible = true;
            }
            else
            {
                layoutAzureDetails.IsVisible = false;
            }
        }

        //private async void btnColecciones_Clicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new PageCollections());
        //}
    }

}
