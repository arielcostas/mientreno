using Mientreno.Compartido.Peticiones;
using Mientreno.Compartido.Recursos;
using Mientreno.Compartido.Validadores;
using System.Net;
using System.Net.Http.Json;

namespace Mientreno.Mobile;

public partial class RegistroPage : ContentPage
{
    public bool IsLoading { get; set; }

    public RegistroPage()
    {
        InitializeComponent();
        usuarioEntry.Focus();
        usuarioEntry.Completed += (s, e) => nombreEntry.Focus();
        nombreEntry.Completed += (s, e) => apellidosEntry.Focus();
        apellidosEntry.Completed += (s, e) => emailEntry.Focus();
        emailEntry.Completed += (s, e) => contraseñaEntry.Focus();
        contraseñaEntry.Completed += (s, e) => confirmarContraseñaEntry.Focus();
    }

    private async void DoRegister(object sender, EventArgs e)
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

        if (contraseñaEntry.Text != confirmarContraseñaEntry.Text)
        {
            errorDisplay.Text = AppStrings.passwordsDoNotMatch;
            return;
        }

        var passwordStrength = ValidadorContraseña.ObtenerFuerza(contraseñaEntry.Text);
        if (passwordStrength < 0.7)
        {
            errorDisplay.Text = AppStrings.passwordTooWeak;
            return;
        }

        try
        {
            HttpClient cli = new()
            {
                BaseAddress = new Uri(Global.BaseUrl)
            };

            var resp = await cli.PostAsync(
                "/Autenticacion/Registrar",
                JsonContent.Create<RegistroInput>(new()
                {
                    Login = usuarioEntry.Text,
                    Nombre = nombreEntry.Text,
                    Apellido = apellidosEntry.Text,
                    Correo = emailEntry.Text,
                    Contraseña = contraseñaEntry.Text
                })
            );

            if (resp.StatusCode == HttpStatusCode.Conflict)
            {
                errorDisplay.Text = AppStrings.usernameOrEmailAlreadyInUse;
                return;
            }

            resp.EnsureSuccessStatusCode();

            await DisplayAlert(
                "Registro",
                "Usuario registrado correctamente. Confirme su dirección de correo para poder iniciar sesión.",
                "OK"
            );

            await Shell.Current.GoToAsync("///LoginPage");
        }
        catch (Exception ex)
        {
            errorDisplay.Text = ex.Message;
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        IsLoading = false;
    }

    private async void GoToLogin(object sender, EventArgs e) => await Shell.Current.GoToAsync("///LoginPage");
}
