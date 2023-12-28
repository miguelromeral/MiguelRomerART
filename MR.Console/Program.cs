using Google.Api.Gax.ResourceNames;
using Google.Cloud.Firestore;
using Google.Type;
using MR.Console;
using MRA.Services.Firebase;
using MRA.Services.Firebase.Documents;
using MRA.Services.Firebase.Models;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;

Console.WriteLine("Setting up Firestore.");

// Configura Firestore con el archivo de configuración JSON descargado
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @".\Credentials\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json");

var firebaseProjecTId = "romerart-6a6c3";
var collection = "drawings";
var urlbase = "https://romerartstorageaccount.blob.core.windows.net/romerartblobcontainer/";

var input = "";

var helper = new ConsoleHelper();

// Inicializa Firestore
FirestoreDb db = FirestoreDb.Create(firebaseProjecTId);

var firebaseService = new FirestoreService(collection, urlbase, db);

Console.WriteLine("Type the details of you new Drawing to Firestore");

var drawing = new MRA.Services.Firebase.Models.Drawing()
{
    Views = 0,
    Likes = 0,
    UrlBase = urlbase
};

Console.WriteLine("ID of the Drawing you're editing (empty if new):");
drawing.Id = Console.ReadLine();

var isNew = String.IsNullOrEmpty(drawing.Id);

if (isNew)
{
    Console.WriteLine("ID of your new drawing:");
    drawing.Id = Console.ReadLine();
}
else
{
    Console.WriteLine("Looking for drawing with  ID '"+drawing.Id+"'");
    drawing = await firebaseService.FindDrawingById(drawing.Id);
}

drawing.Favorite = helper.FillBoolValue(isNew, drawing.Favorite, "Favorite");
drawing.Path = helper.FillStringValue(isNew, drawing.Path, "Path");
drawing.PathThumbnail = helper.FillStringValue(isNew, drawing.PathThumbnail, "Path Thumbnail");
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