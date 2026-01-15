using System.Net;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Server
{
    public class User
    {
        // Первичный ключ
        [Key]
        public int Id{get;set;}

        [Required]
        [MaxLength(100)]
        public string SessionKey { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }
        
        // Для хранения IPEndPoint разделяем на части
        [Required]
        [MaxLength(45)] // Для IPv6
        public string LocalServerAddress { get; set; }
        
        [Required]
        public int LocalServerPort { get; set; }
        
        // Навигационное свойство для сообщений
        public virtual ICollection<Message> UnreadMessages { get; set; } = new List<Message>();

        // ОБЯЗАТЕЛЬНО: Пустой конструктор для Entity Framework
        public User() { }

        // Ваш конструктор
        public User(string key, string userName, IPEndPoint localServer)
        {
            SessionKey = key;
            UserName = userName;
            LocalServerAddress = localServer.Address.ToString();
            LocalServerPort = localServer.Port;
        }

        // Свойство только для чтения для удобства работы
        [NotMapped]
        public IPEndPoint LocalServer
        {
            get => new IPEndPoint(IPAddress.Parse(LocalServerAddress), LocalServerPort);
            set
            {
                LocalServerAddress = value.Address.ToString();
                LocalServerPort = value.Port;
            }
        }
    }
}