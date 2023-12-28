using Google.Api.Gax.ResourceNames;
using Google.Cloud.Firestore;
using Google.Type;
using MR.ConsoleMR;
using MRA.Services.AzureStorage;
using MRA.Services.Firebase;
using MRA.Services.Firebase.Documents;
using MRA.Services.Firebase.Models;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;

var helper = new ConsoleHelper();

try
{
    helper.ShowMessageInfo("Setting up FIRESTORE.");

    // Configura Firestore con el archivo de configuración JSON descargado
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @".\Credentials\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json");

    var firebaseProjecTId = "romerart-6a6c3";
    var collection = "drawings";
    var urlbase = "https://romerartstorageaccount.blob.core.windows.net/romerartblobcontainer/";


    // Inicializa Firestore
    FirestoreDb db = FirestoreDb.Create(firebaseProjecTId);

    var firebaseService = new FirestoreService(collection, urlbase, db);

    helper.ShowMessageInfo("Setting up AZURE STORAGE ACCOUNT.");

    var connectionString = "DefaultEndpointsProtocol=https;AccountName=romerartstorageaccount;AccountKey=yCxyi7V17daYKT6qRSKtc4DyGD4lhxgHSAFD2AXayjDPZW1qtey3Et1bx+bYbdkgn9TCyen82g0q+AStdx4Xmw==;EndpointSuffix=core.windows.net";
    var blobStorageContainer = "romerartblobcontainer";
    var blobURL = "https://romerartstorageaccount.blob.core.windows.net/romerartblobcontainer/";

    var azureStorageService = new AzureStorageService(connectionString, blobStorageContainer, blobURL);

    helper.ShowMessageInfo("Everything's working fine!");

    var input = "";

    while (String.IsNullOrEmpty(input))
    {
        input = helper.FillStringValue(false, "", "ID of the Drawing you're editing. If it doesn't exist. A new one will be created.");
    }

    bool isNew = false;

    helper.ShowMessageInfo("Looking for drawing with  ID '" + input + "'");
    Drawing drawing = await firebaseService.FindDrawingById(input);

    if (drawing == null)
    {
        helper.ShowMessageWarning("No drawing was found with ID '" + input + "'. Proceeding to register a new drawing.");

        isNew = true;
        drawing = new MRA.Services.Firebase.Models.Drawing()
        {
            Id = input,
            Views = 0,
            Likes = 0,
            UrlBase = urlbase
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
                widthThumbnail = helper.FillFreeIntValue(true, 300, "Thumbnail Width");
            }

            helper.ShowMessageInfo("We're ready to create this blob. Please, review the data:");
            helper.ShowMessageInfo("*****");
            helper.ShowMessageInfo("Local File Path".PadRight(25)+rutaEntrada);
            helper.ShowMessageInfo("Blob Path".PadRight(25)+ blobLocation);
            helper.ShowMessageInfo("Blob Thumbnail Path".PadRight(25)+ blobLocationThumbnail);
            helper.ShowMessageInfo("Blob Thumbnail Width".PadRight(25)+ widthThumbnail);
            helper.ShowMessageInfo("*****");

            var confirm = helper.FillBoolValue(false, false, "Proceed to upload to AZURE?");

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

    await firebaseService.AddAsync(drawing);


    helper.ShowMessageInfo("Inserted drawing with ID '" + drawing.Id + "' into Firestore.");

}catch(Exception ex)
{
    helper.ShowMessageError("ERROR while uploading a new drawing: " + ex.Message);
    helper.ShowMessageError("Press any key to exit.");
    Console.ReadKey();
}