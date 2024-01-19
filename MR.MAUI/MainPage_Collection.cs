using Google.Cloud.Firestore;
using MR.MAUI.Classes;
using MRA.Services.Firebase.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.MAUI
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<ImageItem> imageItems;

        Collection collection { get; set; }
        public List<string> ListaCollectionsId { get; set; }

        public List<Collection> ListaCollections { get; set; }



        private void btnLoadCollections_Clicked(object sender, EventArgs e)
        {
            btnLoadCollections.IsVisible = false;
            MainPage_Collection();
        }

        public void MainPage_Collection()
        {
            collection = new Collection();
            var listThumbnails = ListaDrawings.Select(x => new ImageItem()
            {
                ImageSource = x.UrlThumbnail,
                Id = x.Id,
                IsSelected = false,
                UrlThumbnail = x.UrlThumbnail
            }).ToList();

            imageItems = new ObservableCollection<ImageItem>();
            foreach(var thum in listThumbnails)
            {
                imageItems.Add(thum);
            }

            BindableLayout.SetItemsSource(imageFlexLayout, imageItems);

            LoadCollectionsId();
            layoutCollections.IsVisible = true;
        }


        private async void LoadCollectionsId(string id = "")
        {
            ListaCollections = (await _drawingService.GetAllCollections()).OrderBy(x => x.Id).ToList();
            ListaCollectionsId = ListaCollections.Select(x => x.ToString()).ToList();
            cbCollectionId.ItemsSource = ListaCollectionsId;
            if (!String.IsNullOrEmpty(id))
            {
                cbSelectOperationCollection.SelectedIndex = 1;
                cbCollectionId.SelectedIndex = ListaCollections.FindIndex(x => x.Id.Equals(id));
            }
        }


        private void tbCollectionId_Unfocused(object sender, FocusEventArgs e)
        {
            var newValue = ((Entry)sender).Text;
            if (String.IsNullOrEmpty(newValue)) { 
                if (ListaCollections.Count(x => x.Id.Equals(newValue)) > 0)
                {
                    tbCollectionId.Text = "";
                    DisplayAlert("ID Usado", $"Ya existe una colección con ID '{newValue}'.\nCambia el ID y vuelve a intentarlo", "Vale");
                }
                else
                {
                    collection.Id = newValue;
                }
            }
        }


        private void cbSelectOperationCollection_SelectedIndexChanged(object sender, EventArgs e)
        {
            var newIndex = ((Picker)sender).SelectedIndex;

            switch (newIndex)
            {
                case 0:
                    layoutAddCollectionOperation.IsVisible = true;
                    layoutEditCollection.IsVisible = false;
                    break;
                case 1:
                    layoutAddCollectionOperation.IsVisible = false;
                    layoutEditCollection.IsVisible = true;
                    break;
            }
        }


        private void cbCollectionId_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex > -1)
            {
                collection = ListaCollections[selectedIndex];
                ActualizarCollection();
            }
        }

        private void ActualizarCollection()
        {
            foreach(var thumb in imageItems)
            {
                thumb.IsSelected = collection.Drawings.Count(x => x.Id.Equals(thumb.Id)) > 0;
            }

            tbCollectionName.Text = collection.Name;
            tbCollectionDescription.Text = collection.Description;
            tbCollectionOrder.Text = collection.Order.ToString();
        }

        private void OnImageTapped(object sender, EventArgs e)
        {
            if (sender is Image tappedImage)
            {
                if (tappedImage.BindingContext is ImageItem imageItem)
                {
                    imageItem.IsSelected = !imageItem.IsSelected;
                    Debug.WriteLine($"Selected: {imageItem.IsSelected} - {imageItem.IdInCollection}");
                    OnPropertyChanged(nameof(imageItem.IsSelected));
                }
            }
        }

        private async void OnSaveSelectedImagesClicked(object sender, EventArgs e)
        {
            btnSaveCollection.IsEnabled = false;
            try
            {
                var selectedImages = imageItems.Where(item => item.IsSelected).ToList();
                Debug.WriteLine("--> " + string.Join(", ", selectedImages.Select(x => x.IdInCollection)));

                var selected = new List<DocumentReference>();
                foreach (var d in ListaDrawings)
                {
                    if (selectedImages.Count(x => x.Id.Equals(d.Id)) > 0)
                    {
                        selected.Add(_drawingService.GetDbDocumentDrawing(d.Id));
                    }
                }

                collection.DrawingsReferences = selected;
                collection.Name = tbCollectionName.Text;
                collection.Order = int.Parse(tbCollectionOrder.Text);
                collection.Description = tbCollectionDescription.Text;

                await _drawingService.AddAsync(collection);
                LoadCollectionsId(collection.Id);
                DisplayAlert("Actualizado", $"La colección con ID '{collection.Id}' ha sido guardado con éxito.", "Vale");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error al Actualizar", $"Ha ocurrido un error al guardar la colección con ID '{collection.Id}'.\n{ex.Message}", "Vale");
            }
            btnSaveCollection.IsEnabled = true;
        }
    }
}
