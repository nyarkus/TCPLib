using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class PortFinder
    {
        private static object _locker = new object();
        public static int GetAvailablePort()
        {
            TcpListener tcpListener = null;
            UdpClient udpClient = null;

            try
            {
                // Создаем временный TCP слушатель
                tcpListener = new TcpListener(IPAddress.Any, 0); // 0 означает, что система выберет случайный доступный порт
                tcpListener.Start();
                int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port; // Получаем выбранный TCP порт

                // Создаем временный UDP клиент с тем же портом
                udpClient = new UdpClient(port);

                tcpListener?.Stop();
                udpClient?.Close();

                return port; // Возвращаем порт
            }
            catch (Exception ex)
            {
                // Логируем исключение, если оно произошло
                Console.WriteLine($"Error in GetAvailablePort: {ex.Message}");
                throw; // Пробрасываем исключение дальше
            }
            finally
            {
                // Останавливаем TCP слушатель и закрываем UDP клиент
                tcpListener?.Stop();
                udpClient?.Close();
            }
        }
    }
}
