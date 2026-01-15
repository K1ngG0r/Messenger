using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid? ChatId { get; set; }
        
        [Required]
        [MaxLength(1000)]
        [Column("MessageText")]
        public string MessageText { get; set; } = string.Empty; // Инициализация по умолчанию
        
        [MaxLength(100)]
        public string? Sender { get; set; }
        
        [ForeignKey("User")]
        public string? UserSessionKey { get; set; }
        
        public virtual User? User { get; set; }

        public Message(){}
        
        public Message(string messageText, Guid? chatId = null, string? sender = null)
        {
            MessageText = messageText ?? throw new ArgumentNullException(nameof(messageText));
            ChatId = chatId;
            Sender = sender;
        }
    }
}