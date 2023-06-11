using System.Security.Cryptography;
using System.Text;
using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SkiaSharp;

namespace Mientreno.QueueWorker;

public class ProfilePhotoGeneratorWorker : BackgroundService
{
	private readonly ILogger<ProfilePhotoGeneratorWorker> _logger;
	private readonly string _baseDir;

	private readonly IConnection _connection;
	private IModel? _channel;

	public ProfilePhotoGeneratorWorker(ILogger<ProfilePhotoGeneratorWorker> logger, IConnection rabbitConnection,
		IConfiguration configuration)
	{
		_logger = logger;

		_connection = rabbitConnection;

		_baseDir = configuration["FileBase"] ?? throw new Exception("FileBase not found");
		_baseDir = Path.Combine(_baseDir.Trim(), "profile");
		Directory.CreateDirectory(_baseDir);
	}

	public override async Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Profile photo generation worker...");

		_channel = _connection.CreateModel();

		_channel.ExchangeDeclare("mientreno", ExchangeType.Direct, true, false);
		_channel.QueueDeclare(Constantes.ColaGenerarFotoPerfil, true, false, false);
		_channel.QueueBind(Constantes.ColaGenerarFotoPerfil, "mientreno", Constantes.ColaGenerarFotoPerfil);

		await base.StartAsync(cancellationToken);
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		return Task.Run(() =>
		{
			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += OnReceived;
			_channel.BasicConsume(Constantes.ColaGenerarFotoPerfil, false, consumer);
		}, stoppingToken);
	}

	private void OnReceived(object? sender, BasicDeliverEventArgs e)
	{
		var bodyBytes = e.Body.ToArray();
		var data = Serializador.Deserializar<NuevaFoto>(bodyBytes)!;
		
		_logger.LogInformation("Generating profile photo for {DataNombre} {DataApellidos}...", data.Nombre, data.Apellidos);

		if (data == null)
		{
			_logger.LogError("Invalid message");
			throw new Exception("Invalid message");
		}
		
		var bmp = GenerarImagen($"{data.Nombre} {data.Apellidos}");
		
		_logger.LogInformation("Profile photo generated for {DataNombre} {DataApellidos}...", data.Nombre, data.Apellidos);

		var pngOut = GetOutputStream($"{data.IdUsuario}.png");
		_logger.LogInformation("Saving profile photo to {Path}...", _baseDir);
		var png = bmp.Encode(SKEncodedImageFormat.Png, 90);
		png.SaveTo(pngOut);
		
		_logger.LogInformation("Png profile photo saved for {DataNombre} {DataApellidos}...", data.Nombre, data.Apellidos);

		var webpOut = GetOutputStream($"{data.IdUsuario}.webp");
		var webp = bmp.Encode(SKEncodedImageFormat.Webp, 90);
		webp.SaveTo(webpOut);
		
		_logger.LogInformation("Webp profile photo saved for {DataNombre} {DataApellidos}...", data.Nombre, data.Apellidos);

		pngOut.Close();
		webpOut.Close();
		_channel?.BasicAck(e.DeliveryTag, false);
	}

	public override Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Stopping profile photo generator...");
		_channel?.Close();
		_logger.LogInformation("Profile photo generator stopped...");
		return base.StopAsync(cancellationToken);
	}

	/// <summary>
	/// Crea la imagen a partir de las siguientes especificaciones:
	/// - Tamaño de 500x500 con fondo transparente
	/// - Un círculo con el color "fondo" de radio 250 en el centro
	/// - Las dos primeras iniciales del nombre completo en el centro del círculo, en blanco o negro según contraste
	/// - El color "fondo" se genera a partir de los 6 primeros caracteres hexadecimales del SHA1 del color de fondo
	/// </summary>
	/// <param name="nombreCompleto"></param>
	private SKBitmap GenerarImagen(string nombreCompleto)
	{
		// Genera el color de fondo a partir del hash del nombre completo
		var hash = SHA1.HashData(Encoding.UTF8.GetBytes(nombreCompleto));

		// Convierte el hash a un color, convirtiendo a hexadecimal y tomando los primeros 6 caracteres
		var hexString = BitConverter.ToString(hash).Replace("-", "");

		var fondo = SKColor.Parse(hexString[..6]);

		// Crea el bitmap de 500x500
		var bmp = new SKBitmap(500, 500);
		var canvas = new SKCanvas(bmp);

		// Crea el círculo en el centro
		var bgPaint = new SKPaint()
		{
			Color = fondo,
			IsAntialias = true
		};
		canvas.DrawCircle(250, 250, 250, bgPaint);

		// Crea el texto
		var textPaint = new SKPaint()
		{
			Typeface = SKTypeface.FromFamilyName("monospace"),
			Color = UseWhiteContrast(fondo) ? SKColors.White : SKColors.Black,
			TextSize = 220,
			TextAlign = SKTextAlign.Center,
			FakeBoldText = true
		};

		var inicialesChars = nombreCompleto.Trim().Split(" ", 2).Select(part => part[0]);
		var iniciales = string.Join("", inicialesChars);

		canvas.DrawText(
			iniciales,
			250,
			250 + (textPaint.TextSize / 2) - (textPaint.TextSize / 10),
			textPaint
		);

		return bmp;
	}

	private static bool UseWhiteContrast(SKColor background)
	{
		return !(background.Red * 0.299 + background.Green * 0.587 + background.Blue * 0.114 > 186);
	}

	private Stream GetOutputStream(string name)
	{
		return File.OpenWrite(
			Path.Combine(_baseDir, name)
		);
	}
}