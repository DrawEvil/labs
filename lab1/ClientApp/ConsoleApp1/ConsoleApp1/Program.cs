using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
public struct UserInfo
{
    public string Name { get; set; }
    public int Age { get; set; }

    public UserInfo(string name, int age)
    {
        Name = name;
        Age = age;
    }
}


namespace NamedPipeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("MyPipe", PipeDirection.InOut))
            {
                Console.WriteLine("Ожидание подключения клиента...");
                pipeServer.WaitForConnection();

                Console.WriteLine("Клиент подключен.");

                // Чтение данных из клиента
                byte[] buffer = new byte[1024];
                int bytesRead = pipeServer.Read(buffer, 0, buffer.Length);
                string jsonUserData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Десериализация данных в объект UserInfo
                UserInfo receivedUser = JsonSerializer.Deserialize<UserInfo>(jsonUserData);

                Console.WriteLine($"Получены данные от клиента: Имя: {receivedUser.Name}, Возраст: {receivedUser.Age}");

                // Отправка ответа клиенту
                UserInfo responseUser = new UserInfo();
                string jsonResponse = JsonSerializer.Serialize(responseUser);
                byte[] responseBuffer = Encoding.UTF8.GetBytes(jsonResponse);
                pipeServer.Write(responseBuffer, 0, responseBuffer.Length);
                Console.WriteLine("Ответ отправлен клиенту.");

                pipeServer.Close();
            }

            Console.ReadLine();
        }
    }
}
