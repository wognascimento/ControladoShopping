using Camera.MAUI.ZXing;
using Camera.MAUI;
using ControladoShopping.Data.Local;
using System.Text.RegularExpressions;
using ControladoShopping.Data.Local.Models;
using CommunityToolkit.Mvvm.Messaging;


namespace ControladoShopping.Views;

public partial class Scanner : ContentPage
{
    private readonly VolumeScannerRepository _volumeScannerRepository;

    public Scanner(VolumeScannerRepository volumeScannerRepository)
	{
		InitializeComponent();

        _volumeScannerRepository = volumeScannerRepository;

        cameraView.BarCodeDecoder = new ZXingBarcodeDecoder();
        cameraView.BarCodeOptions = new BarcodeDecodeOptions
        {
            AutoRotate = true,
            PossibleFormats = { BarcodeFormat.QR_CODE },
            ReadMultipleCodes = false,
            TryHarder = true,
            TryInverted = true
        };
        cameraView.BarCodeDetectionFrameRate = 10;
        cameraView.BarCodeDetectionMaxThreads = 5;
        cameraView.ControlBarcodeResultDuplicate = true;
        cameraView.BarCodeDetectionEnabled = true;
    }

    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        if (cameraView.Cameras.Count > 0)
        {
            cameraView.Camera = cameraView.Cameras.First();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
            });
        }
    }

    private async void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {

        string pattern = @"\|(\d+)";
        var first = args.Result?.FirstOrDefault();

        if (first is not null)
        {
            Match match = Regex.Match(first.Text, pattern);
            string[] volume = first.Text.Split('|');

            if (match.Success)
            {
                //string numeroEncontrado = match.Groups[1].Value;
                //Console.WriteLine($"Número encontrado: {numeroEncontrado}");

                try
                {
                    var controlado = await  _volumeScannerRepository.GetVolume(volume[0], long.Parse(volume[1]));
                    if (controlado == null)
                        await Task.Run(() => _volumeScannerRepository.CreateVolumeControlado(new VolumeControlado { Sigla = volume[0], Volume = long.Parse(volume[1]) }));

                    WeakReferenceMessenger.Default.Send(new NotificationMessage("Caixa Escaniada"));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
                //ResultLabel.Text = $"Barcodes: {first.BarcodeFormat} -> {first.Text}";
                

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ResultLabel.Text = $"Barcodes: {first.BarcodeFormat} -> {first.Text}";
                });
            }
            else
            {
                //Console.WriteLine("Nenhuma sequência de números encontrada após o '|'");
                ResultLabel.Text = $"QrCode não é de shopping";
            }
        }

        //MainThread.BeginInvokeOnMainThread(() =>
        //{
            //barcodeResult.Text = $"{args.Result[0].BarcodeFormat}: {args.Result[0].Text}";
        //});
    }


    private void SwitchCameraButton_Clicked(object sender, EventArgs e)
    {
        //MessagingCenter.Send(this, "Notify", "Mensagem da primeira página");
        // Envia uma mensagem com o valor "Mensagem da primeira página"
        //WeakReferenceMessenger.Default.Send(new NotificationMessage("Caixa Escaniada"));

    }

    private void TorchButton_Clicked(object sender, EventArgs e)
    {
        cameraView.TorchEnabled = !cameraView.TorchEnabled;
    }
}