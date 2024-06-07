using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.RegularExpressions;

// Clase que produce mensajes y los envía a una cola de RabbitMQ
public class MessageProducer
{
    // Método para enviar un mensaje a la cola
    public void SendMessage(string message)
    {
        // Crear una conexión a RabbitMQ usando la configuración de fábrica con el host "localhost"
        var factory = new ConnectionFactory() { HostName = "localhost" };

        // Establecer la conexión y el canal de comunicación con RabbitMQ
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Declarar una cola llamada "Cola 1" con las siguientes propiedades:
            // - durable: false (la cola no persiste después de reiniciar RabbitMQ)
            // - exclusive: false (la cola no está restringida a esta conexión)
            // - autoDelete: false (la cola no se elimina automáticamente cuando no esté en uso)
            // - arguments: null (sin argumentos adicionales)
            channel.QueueDeclare(queue: "Cola 1",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Convertir el mensaje a un arreglo de bytes usando codificación UTF-8
            var body = Encoding.UTF8.GetBytes(message);

            // Publicar el mensaje en la cola especificada
            channel.BasicPublish(exchange: "",
                                 routingKey: "Cola 1",
                                 basicProperties: null,
                                 body: body);
            // Imprimir mensaje de confirmación en la consola
            Console.WriteLine(" [x] Se envió '{0}'", message);
        }
    }
}

// Clase principal del programa
class Program
{
    static void Main(string[] args)
    {
        // Crear una instancia del productor de mensajes
        var producer = new MessageProducer();

        // Bucle para enviar mensajes hasta que el usuario decida salir
        bool continuar = true;
        while (continuar)
        {
            // Solicitar al usuario que ingrese el mensaje a enviar
            Console.WriteLine("Ingrese el mensaje que desea enviar:");
            string message = Console.ReadLine();

            // Enviar el mensaje usando el productor de mensajes
            producer.SendMessage(message);

            string respuesta;
            do
            {
                // Preguntar al usuario si desea enviar otro mensaje
                Console.WriteLine("¿Desea enviar otro mensaje? (Sí/No)");
                respuesta = Console.ReadLine();

                // Validar la respuesta del usuario
                if (!EsRespuestaValida(respuesta))
                {
                    Console.WriteLine("Por favor, elige 'Sí' o 'No'.");
                }
            } while (!EsRespuestaValida(respuesta));

            // Determinar si el bucle debe continuar en función de la respuesta del usuario
            continuar = (respuesta.ToLower() == "si");
        }
    }

    // Método para validar la respuesta del usuario
    static bool EsRespuestaValida(string respuesta)
    {
        // Expresión regular que coincide con "sí" o "no" en cualquier combinación de mayúsculas/minúsculas.
        Regex regex = new Regex(@"^(si|no)$", RegexOptions.IgnoreCase);
        return regex.IsMatch(respuesta);
    }
}
