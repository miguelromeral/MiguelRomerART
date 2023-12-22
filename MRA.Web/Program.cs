using Google.Cloud.Firestore;
using MRA.Services;
using MRA.Services.Firebase.Interfaces;
using MRA.Services.Firebase;
using Microsoft.Extensions.Configuration;
using MRA.Services.AzureStorage;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Firestore.V1;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var azureStorageService = new AzureStorageService(builder.Configuration);

builder.Services.AddSingleton(azureStorageService);

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @".\Credentials\romerart-6a6c3-firebase-adminsdk-4yop5-839e7a0035.json");

//var json = @"{
//  ""type"": ""service_account"",
//  ""project_id"": ""romerart-6a6c3"",
//  ""private_key_id"": ""839e7a0035ffa5c4326b6b84c4796cbc2dd2bd22"",
//  ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC8SgFI/+KiVI7s\n313YN7oK/nosztPa8ylRJav6TxVTjx8sjcq8SyoQhxEBu668HHWl/1Ucthy/eLBu\nVakV1aG3GNxJU4N3QBYvT6NRXIOaAGOpL1zH0N1m+OcRIs/rxvVOll0MTDBTSI61\nWG3L6Lkai8Yw0sfpfsxtmOkk12WExQZKsMvPom5lFIUXSkg/a9h1eDRcIC/Z+EnR\nM2ud12aqJmCHGiKqrJtI8UWtV1YY6lqWjKbExg8x/d3GtYECU8ZFV3pu4VozsmQ3\nqO3Zog8wzJx+gUvckwdC5MVCjTbZMLra10IsUYaIYkaC2prLa9SgbRyKW6iKwpI0\nudVUwVFhAgMBAAECggEAEGqbZhP5XJHmzDxFYF6rd93sJQ/7ZLGivpJ8RhyzDVAD\nNc0/Cd+70SkVkIMenwQUNd7nOtOd+gu5xkTtsxunN+GRA0umwLTzujhFUwiv6LNz\n/QJGIp3RT/MPomzYmAQGy01M24+g/wHqf2EwYYDu8qORKhyWlYMaq8uTOEUXctXc\ng09vMwhyVMOWn/GUCBIjPJYhtVZQsOuTKWTAeNTvYAn8Z16tl+rrJ1xQNnoZ9dFV\n1riX93yOP5jM6dMVUjS8huKV1J4as8HYPcT33D/9zXxUh3jSNWtdxtTyfjqWFVaB\nJfhQVLSSVk6qzV/9OFq6/mX01rGNnZBfQD2GRWAeYQKBgQDiQM5H5lQBNGzYeyOm\nGL6hqb8RlpGoW9xYvXYi9VK35enA96yFmkOu36H00XmLl79AdXaao54Uunr0sD8G\nfpsOyABX1BUED1Xt7bAN7hoa23yBSeeldvpZhQkuF4qEiOHdgRlHYcQxVNGn+x9Q\nm99mWF5OYUINGCfibvsLdK4/DQKBgQDVC2kUEGPH57h1Km8I8XGoeLAphA4JAV4n\n3yU7JYsJC1JgmOC2Tk25rsb0+pYtbnj71ySfQajAfGSDAA/Ugl/5uWpHSeNlM6i3\nLg0NYnFfKkv72k7YNMfvroDDBYHrwOogn0yd02Hq5CFe0cyOid9BKOwpnhsd3Jtd\n76U6d8HmpQKBgQCSXtaU0T97YA0ip9dSNyPstkI0ALLOtk8A4eExkIApFIZ7Hj/w\nVvJ+iZLfLT1yv4MtElDejgdQ5atx7St46iMqFt2R9FR8QeeXe3OwL1+YDHKpucQw\n2Z3wmA5uUTB9uYhVopVWBrvEJllUUTPbFFwcfALWtjji+8Ohy6aBrMSlaQKBgFVX\njc9QxXfV9klRJ0uQ1LPlLMRktu6akqosNJDd+DU2SNiTRSkicvZX8jE+lJw8bdCM\nnYoCVmRNMEJd6vl5BJNX2CAYlDP9Hq/KjcX30myJ2Ahaqkznz9POtpvo/+N90wwg\nZCOxAr+ChW+jYWsUBc/nYbtGL5ylu8NEdY7XXGBVAoGAQiGBOeD0Gt0a0c6Xl6z/\nCMgr7arpx+JWVg/M3VGIVydWjLXATbO5dQHiY45TbkNx+KiDOJXvNrBwVyLUvt8F\nTlFag0370/gN1WZQkIDljrbf7PI0kMO1/WGjr1nG51qOyiqTY0lx5ruHPfQeOkaU\ndZlf76rpz7mL50L9F3JPzrI=\n-----END PRIVATE KEY-----\n"",
//  ""client_email"": ""firebase-adminsdk-4yop5@romerart-6a6c3.iam.gserviceaccount.com"",
//  ""client_id"": ""104945542677833019548"",
//  ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
//  ""token_uri"": ""https://oauth2.googleapis.com/token"",
//  ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
//  ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-4yop5%40romerart-6a6c3.iam.gserviceaccount.com"",
//  ""universe_domain"": ""googleapis.com""
//}
//";


//var credential = GoogleCredential.FromJson(json).ToChannelCredentials();
//var grpcChannel = new Channel("firestore.googleapis.com", credential);
//var grcpClient = new Firestore.FirestoreClient(grpcChannel);
//var firestoreClient = new FirestoreClientImpl(grcpClient, FirestoreSettings.GetDefault());
//return await FirestoreDb.CreateAsync(FirebaseProjectId, firestoreClient);


//builder.Services.AddSingleton<IFirestoreService>(s => 
//    new FirestoreService(FirestoreDb.Create(builder.Configuration.GetValue<string>("Firebase:ProjectID"))));

var firebaseService = new FirestoreService(builder.Configuration, FirestoreDb.Create(builder.Configuration.GetValue<string>("Firebase:ProjectID")));

builder.Services.AddSingleton<IFirestoreService>(firebaseService);

var drawingService = new DrawingService(azureStorageService, firebaseService);

builder.Services.AddSingleton(drawingService);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

var firestoreService = app.Services.GetRequiredService<IFirestoreService>();
Console.WriteLine($"Firestore Service created: {firestoreService}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
