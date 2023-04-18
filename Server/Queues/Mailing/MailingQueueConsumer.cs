using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using Mientreno.Server.Helpers.Mailing;

namespace Mientreno.Server.Queues.Mailing;

public class MailingQueueConsumer
{

    public async Task Listen(CancellationToken ct = default)
    {

        while (!ct.IsCancellationRequested)
        {
            QueueProperties props = await _queue.GetPropertiesAsync(ct);

            // Si no hay mensajes, espera 5 segundos y vuelve a intentar
            if (props.ApproximateMessagesCount == 0)
            {
                await Task.Delay(5000, ct);
                continue;
            }

            // Obtiene el mensaje de la cola y lo deserializa
            QueueMessage message =
                (await _queue.ReceiveMessagesAsync(1, cancellationToken: ct)).Value[0];

            var c = _serializer.Deserialize(message.MessageText);

            // Aplica la plantilla markdown
            var par = EmailTemplateEngine.Apply(c.Plantila, c.Idioma, c.Parametros);

            // Envía el correo
            await _mailSender.SendMailAsync(c.DireccionDestino, c.NombreDestino, par.subject, par.plain, par.html);

            // Borra el mensaje de la cola
            await _queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, ct);

        }

    }
}