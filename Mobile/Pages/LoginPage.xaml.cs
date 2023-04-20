using Mientreno.Compartido.Peticiones;
using Mientreno.Compartido.Recursos;
using System.Net;
using System.Net.Http.Json;

namespace Mientreno.Mobile;

public partial class LoginPage : ContentPage
{
    public bool IsLoading { get; set; }

    public LoginPage()
    {
        InitializeComponent();
        usuarioEntry.Focus();
        usuarioEntry.Completed += (s, e) => contraseñaEntry.Focus();
        contraseñaEntry.Completed += DoLogin;
    }

    private async void DoLogin(object sender, EventArgs e)
    {
        errorDisplay.Text = " ";
        IsLoading = true;

        if (
            usuarioEntry.Text.Trim() == string.Empty ||
            contraseñaEntry.Text.Trim() == string.Empty)
        {
            errorDisplay.Text = AppStrings.enterUsernameOrPassword;
            return;
        }

        try
        {
            HttpClient cli = new()
            {
                BaseAddress = new Uri(Global.BaseUrl)
            };

            var resp = await cli.PostAsync(
                "/Autenticacion/Iniciar",
                JsonContent.Create<LoginInput>(new()
                {
                    Identificador = usuarioEntry.Text,
                    Credencial = contraseñaEntry.Text
                })
            );

            if (resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception(AppStrings.errorCredencialesInvalidas);
            }

            resp.EnsureSuccessStatusCode();

            var body = await resp.Content.ReadFromJsonAsync<LoginOutput>();
            if (body == null)
            {
                throw new Exception(AppStrings.errorUnexpected);
            }

            Preferences.Default.Set(SettingsKeys.Refresco, body.TokenRefresco);
            Global.Token = body.TokenAcceso;

            await Shell.Current.GoToAsync("///TareasPage");
        }
        catch (Exception ex)
        {
            errorDisplay.Text = ex.Message;
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        IsLoading = false;
    }

    private async void GoToRegister(object sender, EventArgs e) => await Shell.Current.GoToAsync("///RegistroPage");
}
