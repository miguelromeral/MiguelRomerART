using Google.Api.Gax.ResourceNames;
using Google.Cloud.Firestore;
using Google.Type;
using MRA.Services.Helpers;
using MRA.Services.AzureStorage;
using MRA.Services.Firebase;
using MRA.DTO.Firebase.Documents;
using MRA.DTO.Firebase.Models;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

var helper = new ConsoleHelper();

try
{
    // Configuración de la aplicación
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    var configuration = builder.Build();

    // Inicializa Firestore
    helper.ShowMessageInfo("Setting up FIRESTORE.");
    var firebaseService = new FirestoreService(configuration, configuration.GetValue<string>("AzureStorage:BlobPath"));

    helper.ShowMessageInfo("Setting up AZURE STORAGE ACCOUNT.");

    var azureStorageService = new AzureStorageService(
        configuration.GetValue<string>("AzureStorage:ConnectionString"),
        configuration.GetValue<string>("AzureStorage:BlobContainer"), 
        configuration.GetValue<string>("AzureStorage:BlobPath"));

    helper.ShowMessageInfo("Everything's working fine!");



    var TIPOS_FIRESTORE = new Dictionary<int, string>()
        {
            {0, "Dibujo"},
            {1, "Inspiracion"},
            {2, "Colección"},
        };

    var opcion = helper.FillIntValue(false, 0, "Type of Record", TIPOS_FIRESTORE);
    var input = "";


    switch (opcion)
    {
        case 0:
            while (String.IsNullOrEmpty(input))
            {
                input = helper.FillStringValue(false, "", "ID of the Drawing you're editing. If it doesn't exist. A new one will be created.");
            }

            bool isNew = false;

            helper.ShowMessageInfo("Looking for drawing with  ID '" + input + "'");
            Drawing drawing = await firebaseService.FindDrawingByIdAsync(input);

            if (drawing == null)
            {
                helper.ShowMessageWarning("No drawing was found with ID '" + input + "'. Proceeding to register a new drawing.");

                isNew = true;
                drawing = new Drawing()
                {
                    Id = input,
                    Views = 0,
                    Likes = 0,
                    UrlBase = configuration.GetValue<string>("AzureStorage:BlobPath")
                };
            }
            else
            {
                helper.ShowMessageWarning("FOUND drawing with ID '" + input + "'. Now edit the fields you want.");

            }

            drawing.Favorite = helper.FillBoolValue(isNew, drawing.Favorite, "Favorite");

            drawing.Path = helper.FillStringValue(isNew, drawing?.Path ?? "", "Path");
            if (!await azureStorageService.ExistsBlob(drawing.Path))
            {
                helper.ShowMessageWarning($"The path '{drawing.Path}' does not exist in Azure. Do you want to create it?");
                var upload = helper.FillBoolValue(isNew, true, "Upload Blob to Azure");

                if (upload)
                {
                    string rutaEntrada = "", blobLocation = drawing.Path, blobLocationThumbnail = "";
                    int widthThumbnail = 0;

                    while (String.IsNullOrEmpty(rutaEntrada) || String.IsNullOrEmpty(blobLocation))
                    {
                        rutaEntrada = helper.FillStringValue(false, "", "Local path of the image");
                        blobLocationThumbnail = azureStorageService.CrearThumbnailName(blobLocation);
                        widthThumbnail = helper.FillFreeIntValue(false, 350, "Thumbnail Width");
                    }

                    helper.ShowMessageInfo("We're ready to create this blob. Please, review the data:");
                    helper.ShowMessageInfo("*****");
                    helper.ShowMessageInfo("Local File Path".PadRight(25) + rutaEntrada);
                    helper.ShowMessageInfo("Blob Path".PadRight(25) + blobLocation);
                    helper.ShowMessageInfo("Blob Thumbnail Path".PadRight(25) + blobLocationThumbnail);
                    helper.ShowMessageInfo("Blob Thumbnail Width".PadRight(25) + widthThumbnail);
                    helper.ShowMessageInfo("*****");

                    var confirm = helper.FillBoolValue(false, true, "Proceed to upload to AZURE?");

                    if (confirm)
                    {
                        helper.ShowMessageInfo("*****");
                        helper.ShowMessageInfo("Uploading to Azure Storage Account");

                        await azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, blobLocation, 0);
                        await azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, blobLocationThumbnail, widthThumbnail);

                        drawing.Path = blobLocation;
                        drawing.PathThumbnail = blobLocationThumbnail;
                    }
                    else
                    {
                        helper.ShowMessageError("UPLOAD BLOB TO AZURE CANCELLED");
                    }

                }
                else
                {
                    helper.ShowMessageInfo("Not uploading file to Azure");
                    helper.ShowMessageError("THE DRAWING WILL NOT CONTAIN ANY IMAGE, PLEASE REVIEW LATER.");
                }

            }
            else
            {
                drawing.PathThumbnail = azureStorageService.CrearThumbnailName(drawing.Path);
            }
            drawing.Name = helper.FillStringValue(isNew, drawing?.Name ?? "", "Character's Name");
            drawing.ModelName = helper.FillStringValue(isNew, drawing?.ModelName ?? "", "Model's Name");
            drawing.Title = helper.FillStringValue(isNew, drawing?.Title ?? "", "Drawing's Title");
            drawing.Type = helper.FillIntValue(isNew, drawing.Type, "Type", Drawing.DRAWING_TYPES);
            if (drawing.Type == 2)
            {
                drawing.Paper = 0;
                drawing.Software = helper.FillIntValue(isNew, drawing.Software, "Software Used", Drawing.DRAWING_SOFTWARE);
            }
            else
            {
                drawing.Software = 0;
                drawing.Paper = helper.FillIntValue(isNew, drawing.Paper, "Paper Size", Drawing.DRAWING_PAPER_SIZE);
            }
            drawing.Date = helper.FillStringValue(isNew, drawing?.Date ?? "", "Date (YYYY/MM/DD)");
            drawing.Time = helper.FillFreeIntValue(isNew, drawing.Time, "Time Spent");
            drawing.ProductType = helper.FillIntValue(isNew, drawing.ProductType, "Product Type", Drawing.DRAWING_PRODUCT_TYPES);
            drawing.ProductName = helper.FillStringValue(isNew, drawing?.ProductName ?? "", "Product Name");
            drawing.Comment = helper.FillStringValue(isNew, drawing?.Comment ?? "", "Comments (Type '" + Drawing.SEPARATOR_COMMENTS + "' to separate many comments.)");
            drawing.CommentPros = helper.FillStringValue(isNew, drawing?.CommentPros ?? "", "Comments Pros (Type '" + Drawing.SEPARATOR_COMMENTS + "' to separate many comments.)");
            drawing.CommentCons = helper.FillStringValue(isNew, drawing?.CommentCons ?? "", "Comments Cons (Type '" + Drawing.SEPARATOR_COMMENTS + "' to separate many comments.)");
            drawing.ReferenceUrl = helper.FillStringValue(isNew, drawing?.ReferenceUrl ?? "", "Reference Image URL");


            helper.ShowMessageInfo("Everything's set and ready to go. Are you sure?");
            helper.PrintPropreties(drawing);
            helper.ShowMessageInfo("Press 'Enter' to continue.");
            Console.ReadLine();

            if (isNew)
            {

                helper.ShowMessageInfo("Inserting NEW Drawing to Firestore.");
            }
            else
            {

                helper.ShowMessageInfo("UPDATING the Drawing to Firestore");
            }


            helper.ShowMessageInfo("Please wait...");

            await firebaseService.AddDrawingAsync(drawing);


            helper.ShowMessageInfo("Inserted drawing with ID '" + drawing.Id + "' into Firestore.");

            break;
        case 1:
            while (String.IsNullOrEmpty(input))
            {
                input = helper.FillStringValue(false, "", "ID of the Inspiration you're editing. If it doesn't exist. A new one will be created.");
            }

            isNew = false;

            helper.ShowMessageInfo("Looking for inspiration with  ID '" + input + "'");
            Inspiration inspiration = await firebaseService.FindInspirationById(input);

            if (inspiration == null)
            {
                helper.ShowMessageWarning("No inspiration was found with ID '" + input + "'. Proceeding to register a new inspiration.");

                isNew = true;
                inspiration = new Inspiration()
                {
                    Id = input
                };
            }
            else
            {
                helper.ShowMessageWarning("FOUND inspiration with ID '" + input + "'. Now edit the fields you want.");

            }

            inspiration.Name = helper.FillStringValue(isNew, inspiration.Name ?? "", "Name");
            inspiration.Type = helper.FillIntValue(isNew, inspiration.Type, "Type", Inspiration.INSPIRATION_TYPES);
            inspiration.Instagram = helper.FillStringValue(isNew, inspiration.Instagram ?? "", "Instagram");
            inspiration.Twitter = helper.FillStringValue(isNew, inspiration.Twitter ?? "", "Twitter");
            inspiration.YouTube = helper.FillStringValue(isNew, inspiration.YouTube ?? "", "YouTube");
            inspiration.Twitch = helper.FillStringValue(isNew, inspiration.Twitch ?? "", "Twitch");
            inspiration.Pinterest = helper.FillStringValue(isNew, inspiration.Pinterest ?? "", "Pinterest");

            helper.ShowMessageInfo("Everything's set and ready to go. Are you sure?");
            helper.PrintPropreties(inspiration);
            helper.ShowMessageInfo("Press 'Enter' to continue.");
            Console.ReadLine();

            if (isNew)
            {

                helper.ShowMessageInfo("Inserting NEW Inspiration to Firestore.");
            }
            else
            {

                helper.ShowMessageInfo("UPDATING the Inspiration to Firestore");
            }


            helper.ShowMessageInfo("Please wait...");

            await firebaseService.AddAsync(inspiration);


            helper.ShowMessageInfo("Inserted inspiration with ID '" + inspiration.Id + "' into Firestore.");
            break;
        case 2:
            while (String.IsNullOrEmpty(input))
            {
                input = helper.FillStringValue(false, "", "ID of the Collection you're editing. If it doesn't exist. A new one will be created.");
            }

            isNew = false;

            helper.ShowMessageInfo("Looking for collection with  ID '" + input + "'");
            Collection col = await firebaseService.FindCollectionByIdAsync(input, new List<Drawing>());

            if (col == null)
            {
                helper.ShowMessageWarning("No collection was found with ID '" + input + "'. Proceeding to register a new collection.");

                isNew = true;
                col = new Collection()
                {
                    Id = input,
                    DrawingsReferences = new List<DocumentReference>()
                };
            }
            else
            {
                helper.ShowMessageWarning("FOUND collection with ID '" + input + "'. Now edit the fields you want.");

            }

            col.Name = helper.FillStringValue(isNew, col.Name ?? "", "Name");
            col.Description = helper.FillStringValue(isNew, col.Description, "Description");

            var usedReferences = new DocumentReference[col.DrawingsReferences.Count];
            col.DrawingsReferences.CopyTo(usedReferences);

            foreach(var reference in usedReferences)
            {
                if(helper.FillBoolValue(isNew, true, $"Keep '{reference.Id}'"))
                {
                    helper.ShowMessageInfo("Keeping " + reference.Id);
                }
                else
                {
                    col.DrawingsReferences.Remove(reference);
                }
            }
            input = "";
            do
            {
                input = helper.FillStringValue(true, "", "New reference [empty if exit]:");
                if (!String.IsNullOrEmpty(input))
                {
                    var tmp = firebaseService.DocumentReference(firebaseService.CollectionDrawings, input);
                    col.DrawingsReferences.Add(tmp);
                }
            } while (!String.IsNullOrEmpty(input));


            helper.ShowMessageInfo("Everything's set and ready to go. Are you sure?");
            helper.PrintPropreties(col);
            helper.ShowMessageInfo("Press 'Enter' to continue.");
            Console.ReadLine();

            if (isNew)
            {

                helper.ShowMessageInfo("Inserting NEW collection to Firestore.");
            }
            else
            {

                helper.ShowMessageInfo("UPDATING the collection to Firestore");
            }


            helper.ShowMessageInfo("Please wait...");

            await firebaseService.AddCollectionAsync(col, new List<Drawing>());


            helper.ShowMessageInfo("Inserted collection with ID '" + col.Id + "' into Firestore.");
            break;
        default:
            break;
    }

}catch(Exception ex)
{
    helper.ShowMessageError("ERROR while uploading a new drawing: " + ex.Message);
    helper.ShowMessageError("Press any key to exit.");
    Console.ReadKey();
}