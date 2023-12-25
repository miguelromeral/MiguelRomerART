using Microsoft.Extensions.Logging;
using MRA.Services.AzureStorage;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
using MRA.Web.Models.Art;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services
{
    public class DrawingService
    {

        private readonly AzureStorageService _azureStorageService;
        private readonly IFirestoreService _firestoreService;

        public DrawingService(AzureStorageService storageService, IFirestoreService firestoreService)
        {
            _azureStorageService = storageService;
            _firestoreService = firestoreService;
        }

        public async Task<List<Drawing>> GetAllDrawings()
        {
            var drawings = await _firestoreService.GetAll();
            SetBlobUrl(ref drawings);
            return drawings;
        }

        public async Task<List<Drawing>> FilterDrawings(FilterDrawingModel filter)
        {
            List<Drawing> drawings = new List<Drawing>();
            if (filter.Type.Equals("all"))
            {
                drawings = await GetAllDrawings();
            }
            else
            {
                drawings = await _firestoreService.Filter(filter.Type);
            }

            SetBlobUrl(ref drawings);

            switch (filter.Sortby){
                //case "date-desc": 
                case "date-asc": 
                    drawings = drawings.OrderBy(x => x.Date).ToList();
                    break;
                default:
                    drawings = drawings.OrderByDescending(x => x.Date).ToList();
                    break;
            }

            return drawings;
        }

        private void SetBlobUrl(ref List<Drawing> drawings)
        {
            drawings.ForEach(d => d.UrlBase = _azureStorageService.BlobURL);
        }

        public async Task<Drawing> FindDrawingById(string documentId) => await _firestoreService.FindDrawingById(documentId);
    }
}
