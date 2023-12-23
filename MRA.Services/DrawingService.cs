using Microsoft.Extensions.Logging;
using MRA.Services.AzureStorage;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase.Models;
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

        public async Task<List<Drawing>> FilterDrawings(string type)
        {
            if (type.Equals("all"))
            {
                return await GetAllDrawings();
            }

            var drawings = await _firestoreService.Filter(type);
            SetBlobUrl(ref drawings);
            return drawings;
        }

        private void SetBlobUrl(ref List<Drawing> drawings)
        {
            drawings.ForEach(d => d.UrlBase = _azureStorageService.BlobURL);
        }

        public async Task<Drawing> FindDrawingById(string documentId) => await _firestoreService.FindDrawingById(documentId);
    }
}
