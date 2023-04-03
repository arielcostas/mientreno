using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace Mientreno.Mobile.Models;

public class Tarea : INotifyPropertyChanged
{
    private int _tareaId;
    private string _titulo;
    private bool _completada;

    public int TareaId
    {
        get => _tareaId;
        set
        {
            if (_tareaId != value)
            {
                _tareaId = value;
                OnPropertyChangedAsync();
            }
        }
    }

    public string Titulo
    {
        get => _titulo;
        set
        {
            if (_titulo != value)
            {
                _titulo = value;
                OnPropertyChangedAsync();
            }
        }
    }

    public bool Completada
    {
        get => _completada;
        set
        {
            if (_completada != value)
            {
                _completada = value;
                OnPropertyChangedAsync();
            }
        }
    }

    public void CompletarTarea(object sender, EventArgs args)
    {
        Completada = true;
        OnPropertyChangedAsync(nameof(Completada));
    }

    protected virtual async Task OnPropertyChangedAsync([CallerMemberName] string propertyName = null)
    {
        try
        {


            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            HttpClient cli = new();
            cli.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Global.Token);

            var tareas = await cli.PatchAsJsonAsync($"http://199.9.9.20:5072/Tareas/{TareaId}", new
            {
                titulo = Titulo,
                completada = Completada
            });

            tareas.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            File.WriteAllText(@"C:\\Users\\desar\\Desktop\\error.log", ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}