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
        
        [ForeignKey("User")]
        public string? UserSessionKey { get; set; }
        
        public virtual User? Sender { get; set; }

        public Message(){}
        
        public Message(string messageText, Guid? chatId = null, User? sender = null)
        {
            MessageText = messageText ?? throw new ArgumentNullException(nameof(messageText));
            ChatId = chatId;
            Sender = sender;
        }
    }
}