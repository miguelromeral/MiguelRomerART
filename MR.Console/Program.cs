using Google.Api.Gax.ResourceNames;
using Google.Cloud.Firestore;
using Google.Type;
using MR.Console;
using MRA.Services.AzureStorage;
using MRA.Services.Firebase;
using MRA.Services.Firebase.Documents;
using MRA.Services.Firebase.Models;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;

Console.WriteLine("Setting up Firestore.");

// Configura Firestore con el archivo de configuración JSON descargado
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @".\Credentials\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json");

var firebaseProjecTId = "romerart-6a6c3";
var collection = "drawings";
var urlbase = "https://romerartstorageaccount.blob.core.windows.net/romerartblobcontainer/";

var connectionString = "DefaultEndpointsProtocol=https;AccountName=romerartstorageaccount;AccountKey=yCxyi7V17daYKT6qRSKtc4DyGD4lhxgHSAFD2AXayjDPZW1qtey3Et1bx+bYbdkgn9TCyen82g0q+AStdx4Xmw==;EndpointSuffix=core.windows.net";
var blobStorageContainer = "romerartblobcontainer";
var blobURL = "https://romerartstorageaccount.blob.core.windows.net/romerartblobcontainer/";

var azureStorageService = new AzureStorageService(connectionString, blobStorageContainer, blobURL);


await azureStorageService.ExistsBlob("tests/test_tn.png");

var helper = new ConsoleHelper();

// Inicializa Firestore
FirestoreDb db = FirestoreDb.Create(firebaseProjecTId);

var firebaseService = new FirestoreService(collection, urlbase, db);


Console.WriteLine("ID of the Drawing you're editing. If it doesn't exist. A new one will be created.");
var input = Console.ReadLine();

bool isNew = false;


Console.WriteLine("Looking for drawing with  ID '" + input + "'");
Drawing drawing = await firebaseService.FindDrawingById(input);

if(drawing == null) { 
    Console.WriteLine("No drawing was found with ID '" + input + "'. Proceeding to register a new drawing.");
    isNew = true;
    drawing = new MRA.Services.Firebase.Models.Drawing()
    {
        Id = input,
        Views = 0,
        Likes = 0,
        UrlBase = urlbase
    };
}

drawing.Favorite = helper.FillBoolValue(isNew, drawing.Favorite, "Favorite");


//try
//{

//    Console.WriteLine("Getting ready to upload blob to Azure Storage");
//    var upload = helper.FillBoolValue(isNew, (isNew ? true : false), "Upload Blob to Azure");

//    if (upload)
//    {
//        string rutaEntrada = "", blobLocation = "", blobLocationThumbnail = "";
//        int widthThumbnail = 0;

//        while (String.IsNullOrEmpty(rutaEntrada) || String.IsNullOrEmpty(blobLocation))
//        {
//            //var rutaEntrada = "M:\\Escritorio\\RomerART\\Imagenes Subidas\\markers\\link.jpg";
//            rutaEntrada = helper.FillStringValue(false, "", "Local path of the image");
//            blobLocation = helper.FillStringValue(false, "", "Blob Location");
//            blobLocationThumbnail = azureStorageService.CrearThumbnailName(blobLocation);
//            widthThumbnail = helper.FillFreeIntValue(true, 300, "Thumbnail Width");
//        }

//        Console.WriteLine("We're ready to create this blob. Please, review the data:");
//        Console.WriteLine($"- Local File Path: {rutaEntrada}");
//        Console.WriteLine($"- Blob Location: {blobLocation}");
//        Console.WriteLine($"- Blob Thumbnail Location: {blobLocationThumbnail}");
//        Console.WriteLine($"- Blob Thumbnail Width: {widthThumbnail}");

//        var confirm = helper.FillBoolValue(false, false, "Proceed to upload to AZURE?");

//        if (confirm)
//        {
//            await azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, blobLocation, 0);
//            await azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, blobLocationThumbnail, widthThumbnail);
//        }
//        else
//        {
//            Console.WriteLine("UPLOAD BLOB TO AZURE CANCELLED");
//        }

//    }
//    else
//    {
//        Console.WriteLine("Not uploading file to Azure");
//    }
//}
//catch (Exception ex)
//{
//    Debug.WriteLine("Error whe uploading file: " + ex.Message);
//}


drawing.Path = helper.FillStringValue(isNew, drawing.Path, "Path");
if (!await azureStorageService.ExistsBlob(drawing.Path))
{
    Console.WriteLine($"The path '{drawing.Path}' does not exist in Azure. Do you want to create it?");
    var upload = helper.FillBoolValue(isNew, (isNew ? true : false), "Upload Blob to Azure");

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

        Console.WriteLine("We're ready to create this blob. Please, review the data:");
        Console.WriteLine("*****");
        Console.WriteLine($"- Local File Path: {rutaEntrada}");
        Console.WriteLine($"- Blob Path: {blobLocation}");
        Console.WriteLine($"- Blob Thumbnail Path: {blobLocationThumbnail}");
        Console.WriteLine($"- Blob Thumbnail Width: {widthThumbnail}");
        Console.WriteLine("*****");

        var confirm = helper.FillBoolValue(false, false, "Proceed to upload to AZURE?");

        if (confirm)
        {
            Console.WriteLine("*****");
            Console.WriteLine("Uploading to Azure Storage Account");

            await azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, blobLocation, 0);
            await azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, blobLocationThumbnail, widthThumbnail);

            drawing.Path = blobLocation;
            drawing.PathThumbnail = blobLocationThumbnail;
        }
        else
        {
            Console.WriteLine("UPLOAD BLOB TO AZURE CANCELLED");
        }

    }
    else
    {
        Console.WriteLine("Not uploading file to Azure");
        Console.WriteLine("THE DRAWING WILL NOT CONTAIN ANY IMAGE, PLEASE REVIEW LATER.");
    }

}
else
{
    drawing.PathThumbnail = azureStorageService.CrearThumbnailName(drawing.Path);
}
drawing.Name = helper.FillStringValue(isNew, drawing.Name, "Character's Name");
drawing.ModelName = helper.FillStringValue(isNew, drawing.ModelName, "Model's Name");
drawing.Title = helper.FillStringValue(isNew, drawing.Title, "Drawing's Title");
drawing.Type = helper.FillIntValue(isNew, drawing.Type, "Type", Drawing.DRAWING_TYPES);
drawing.Date = helper.FillStringValue(isNew, drawing.Date, "Date (YYYY/MM/DD)");
drawing.Time = helper.FillFreeIntValue(isNew, drawing.Time, "Time Spent");
drawing.ProductType = helper.FillIntValue(isNew, drawing.ProductType, "Product Type", Drawing.DRAWING_PRODUCT_TYPES);
drawing.ProductName = helper.FillStringValue(isNew, drawing.ProductName, "Product Name");
drawing.Comment = helper.FillStringValue(isNew, drawing.Comment, "Comments (Type '"+Drawing.SEPARATOR_COMMENTS+"' to separate many comments.)");
drawing.CommentPros = helper.FillStringValue(isNew, drawing.CommentPros, "Comments Pros (Type '"+Drawing.SEPARATOR_COMMENTS+ "' to separate many comments.)");
drawing.CommentCons = helper.FillStringValue(isNew, drawing.CommentCons, "Comments Cons (Type '"+Drawing.SEPARATOR_COMMENTS+ "' to separate many comments.)");
drawing.ReferenceUrl = helper.FillStringValue(isNew, drawing.ReferenceUrl, "Reference Image URL");


Console.WriteLine("-----------------------------");
Console.WriteLine("Everything's set and ready to go. Are you sure?");
helper.PrintPropreties(drawing);
Console.WriteLine("Press 'Enter' to continue.");
Console.ReadLine();


if (isNew)
{
    Console.WriteLine("Inserting the new Drawing to Firestore.");
}
else
{
    Console.WriteLine("Updating the Drawing to Firestore");
}

Console.WriteLine("Please wait...");

await firebaseService.AddAsync(drawing);

Console.WriteLine("Inserted drawing with ID '"+drawing.Id+"' into Firestore.");