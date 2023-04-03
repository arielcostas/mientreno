using System.Net;
using System.Net.Http.Json;

namespace Mientreno.Mobile;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        usuarioEntry.Focus();
        usuarioEntry.Completed += (s, e) => contraseñaEntry.Focus();
        contraseñaEntry.Completed += DoLogin;
    }

    private async void DoLogin(object sender, EventArgs e)
    {
        errorDisplay.Text = string.Empty;
        ActivityIndicator indicator = new()
        {
            Color = Colors.Orange,
            IsRunning = true,
            IsVisible = true,
        };

        if (
            usuarioEntry.Text.Trim() == string.Empty ||
            contraseñaEntry.Text.Trim() == string.Empty)
        {
            errorDisplay.Text = "Debe ingresar usuario y contraseña";
            return;
        }

        try
        {
            HttpClient client = new HttpClient();
            var resp = await client.PostAsync(
                "http://199.9.9.20:5072/Auth/Login",
                JsonContent.Create(new
                {
                    id = usuarioEntry.Text,
                    contraseña = contraseñaEntry.Text
                })
            );

            if (resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception("Credenciales incorrectas");
            }

            resp.EnsureSuccessStatusCode();

            var token = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            if (token == null)
            {
                throw new Exception("No se pudo iniciar sesión: respuesta inesperada");
            }

            Sesion.Token = token.Token;

            await Shell.Current.GoToAsync("///TareasPage");
        }
        catch (Exception ex)
        {
            errorDisplay.Text = ex.Message;
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        
        indicator.IsRunning = false;
        indicator.IsVisible = false;
    }
}

internal class LoginResponse
{
    public string Token { get; set; }
}

