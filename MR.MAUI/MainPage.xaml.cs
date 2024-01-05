using Grpc.Net.Client.Balancer;
using MRA.Services;
using MRA.Services.Firebase.Interfaces;
using Plugin.CloudFirestore;
using System.Diagnostics;

namespace MR.MAUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        DrawingService _drawingService;

        public MainPage(/*IFirestoreService drawingService*/)
        {
            InitializeComponent();

            //var tmp = drawingService;

            _drawingService = Application.Current.MainPage
                .Handler
                .MauiContext
                .Services  // IServiceProvider
                .GetService<DrawingService>();

            Load();
        }

        public async void Load()
        {
            var cloud = await _drawingService.FindDrawingById("cloud");
            Debug.WriteLine("Cloud: " + cloud?.Name);
        }


        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
