using MRA.Services.AzureStorage;
using System.Diagnostics;

Console.WriteLine("Setting up connection with Azure");



var connectionString = "DefaultEndpointsProtocol=https;AccountName=romerartstorageaccount;AccountKey=yCxyi7V17daYKT6qRSKtc4DyGD4lhxgHSAFD2AXayjDPZW1qtey3Et1bx+bYbdkgn9TCyen82g0q+AStdx4Xmw==;EndpointSuffix=core.windows.net";
var blobStorageContainer = "romerartblobcontainer";
var blobURL = "https://romerartstorageaccount.blob.core.windows.net/romerartblobcontainer/";


var azureStorageService = new AzureStorageService(connectionString, blobStorageContainer, blobURL);

try
{

    Console.WriteLine("Getting ready to upload");
    var rutaEntrada = "M:\\Escritorio\\RomerART\\Imagenes Subidas\\markers\\link.jpg";
    var nombreBlob = "tests/test.png";
    var nombreBlobTn = "tests/test_tn.png";
    var anchoDeseado = 300;
    await azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, nombreBlob, 0);
    await azureStorageService.RedimensionarYGuardarEnAzureStorage(rutaEntrada, nombreBlobTn, anchoDeseado);

}catch(Exception ex)
{
    Debug.WriteLine("Error whe uploading file: " + ex.Message);
}