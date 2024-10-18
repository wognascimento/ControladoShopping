using CommunityToolkit.Mvvm.Messaging;
using ControladoShopping.Data.Local;
using ControladoShopping.ViewModels;

namespace ControladoShopping.Views;

public partial class Principal : ContentPage
{
    private readonly VolumeScannerRepository _volumeScannerRepository;

    public Principal(PrincipalViewModel viewModel, VolumeScannerRepository volumeScannerRepository)
	{
		InitializeComponent();

        _volumeScannerRepository = volumeScannerRepository;
        BindingContext = viewModel;

        WeakReferenceMessenger.Default.Register<NotificationMessage>(this, async (recipient, message) =>
        {
            // Ação quando a mensagem é recebida
            PrincipalViewModel vm = (PrincipalViewModel)BindingContext;
            vm.VolumeControlados = await Task.Run(vm.GetVolumesAsync);
            //Console.WriteLine($"Mensagem recebida: {message.Value}");
        });
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        try
        {
            PrincipalViewModel vm = (PrincipalViewModel)BindingContext;
            vm.VolumeControlados = await Task.Run(vm.GetVolumesAsync);

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}