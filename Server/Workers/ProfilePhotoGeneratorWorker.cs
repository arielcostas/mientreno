using System.Security.Cryptography;
using System.Text;
using Mientreno.Compartido;
using Mientreno.Compartido.Mensajes;
using Mientreno.Server.Connectors.Queue;
using SkiaSharp;

namespace Mientreno.Server.Workers;

public class ProfilePhotoGeneratorWorker : BackgroundService
{
	private readonly ILogger<ProfilePhotoGeneratorWorker> _logger;
	private readonly string _baseDir;

	private readonly IQueueConsumer<NuevaFoto> _consumer;

	public ProfilePhotoGeneratorWorker(ILogger<ProfilePhotoGeneratorWorker> logger, IQueueConsumer<NuevaFoto> consumer,
		IConfiguration configuration)
	{
		_logger = logger;
		_consumer = consumer;

		_baseDir = configuration["FileBase"] ?? throw new Exception("FileBase not found");
		_baseDir = Path.Combine(_baseDir.Trim(), "profile");
		Directory.CreateDirectory(_baseDir);
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_consumer.Listen(Constantes.ColaGenerarFotoPerfil, OnGenerar);
		
		return Task.CompletedTask;
	}

	private void OnGenerar(NuevaFoto input)
	{
		_logger.LogInformation("Generating profile photo for {DataNombre} {DataApellidos}...", input.Nombre,
			input.Apellidos);

		var bmp = GenerarImagen($"{input.Nombre} {input.Apellidos}");
		_logger.LogInformation("Profile photo generated for {DataNombre} {DataApellidos}...", input.Nombre,
			input.Apellidos);
		_logger.LogInformation("Saving profile photo to {Path}...", _baseDir);

		var png = Task.Run(() =>
		{
			var pngOut = GetOutputStream($"{input.IdUsuario}.png");
			if (!pngOut.CanWrite)
			{
				throw new Exception("Can't write to PNG file");
			}
			
			var png = bmp.Encode(SKEncodedImageFormat.Png, 90);
			png.SaveTo(pngOut);

			_logger.LogInformation("Png profile photo saved for {DataNombre} {DataApellidos}...", input.Nombre,
				input.Apellidos);
			
			pngOut.Close();
		});

		var webp = Task.Run(() =>
		{
			var webpOut = GetOutputStream($"{input.IdUsuario}.webp");

			if (!webpOut.CanWrite)
			{
				throw new Exception("Can't write to WEBP file");
			}
			
			var webp = bmp.Encode(SKEncodedImageFormat.Webp, 90);
			webp.SaveTo(webpOut);

			_logger.LogInformation("Webp profile photo saved for {DataNombre} {DataApellidos}...", input.Nombre,
				input.Apellidos);

			webpOut.Close();
		});
		
		Task.WaitAll(png, webp);
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

	private FileStream GetOutputStream(string name)
	{
		return File.OpenWrite(
			Path.Combine(_baseDir, name)
		);
	}
}