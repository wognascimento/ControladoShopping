using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControladoShopping.Data.Local.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text;
using ControladoShopping.Data.Local;
using ControladoShopping.Views;

namespace ControladoShopping.ViewModels
{
    public partial class PrincipalViewModel : ObservableObject
    {

        [ObservableProperty]
        ObservableCollection<VolumeControlado> volumeControlados;

        [ObservableProperty]
        string status = "Enviando volumes controlado para Cipolatti.";

        [ObservableProperty]
        bool isLoading = false;

        private readonly VolumeScannerRepository _volumeScannerRepository;

        public PrincipalViewModel(VolumeScannerRepository volumeScannerRepository)
        {
            _volumeScannerRepository = volumeScannerRepository;
        }

        public async Task<ObservableCollection<VolumeControlado>> GetVolumesAsync()
        {
            try
            {
                var dados = await _volumeScannerRepository.GetAllVolumeScanners();
                return new ObservableCollection<VolumeControlado>(dados);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [RelayCommand]
        public async Task SendVolumes()
        {
            //Console.WriteLine($"delete");
            Status = "BUSCANDO VOLUMES CONTROLADO NÃO ENVIADOS NO DISPOSITIVO.";
            IsLoading = true;
            var dados = await Task.Run(_volumeScannerRepository.GetVolumesNotSenders);
            int tot = dados.Count();
            if (tot == 0)
            {
                //await Page.DisplayAlert("Enviar ", "Não tem volumes para serem enviados", "OK");
                await Application.Current.MainPage.DisplayAlert("ENVIO CONTROLADO", "NÃO EXISTE VOLUMES HA SER ENVIADOS.", "OK");
                IsLoading = false;
                return;
            }

            JsonSerializerOptions _serializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            foreach (var volume in dados)
            {
                try
                {
                    Status = $"ENVIANDO VOLUME CONTROLADO {volume.Sigla} - {volume.Volume}";
                    var httpClient = new HttpClient();

                    JsonSerializerOptions options = new()
                    {
                        WriteIndented = true
                    };
                    string jsonParametro = JsonSerializer.Serialize(new { sigla = volume.Sigla, volume = volume.Volume, conferido = DateOnly.FromDateTime(volume.Created.Value)}, options);
                    HttpClientHandler handler = new()
                    {
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                    };
                    var content = new StringContent(jsonParametro, Encoding.UTF8, "application/json");
                    using HttpClient client = new(handler);
                    //using HttpResponseMessage response = await httpClient.PostAsync("https://api.cipolatti.com.br:44366/api/VolumeControlado/ReceberControlado", content);
                    HttpResponseMessage response = await client.PostAsync("https://api.cipolatti.com.br:44366/api/VolumeControlado/ReceberControlado", content);
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        if (jsonResponse == "Volume controlado enviado com sucesso!" || jsonResponse == "Nada a fazer.")
                        {
                            volume.Enviado = true;
                            await Task.Run(() => _volumeScannerRepository.UpdateVolumeScanner(volume));
                        }
                    }
                    IsLoading = false;
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro ao carregar Siglas", ex.Message, "OK");
                    IsLoading = false;
                }
            }

            await Application.Current.MainPage.DisplayAlert("Envio", "Volume controlado enviado com sucesso!", "OK");

        }

        [RelayCommand]
        public async Task ScannerVolume()
        {
            await Shell.Current.GoToAsync(nameof(Scanner));
        }
    }
}
