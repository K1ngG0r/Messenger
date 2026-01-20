using Client.Connection;
using Client.Data;
using Client.Models;
using Client.ViewModels;
using Client.ViewModels.Patterns;
using Client.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            DataContext = mainWindowViewModel;
        }
        public MainWindow()
        {
            InitializeComponent();
        }
        /*private void ResetDatabase(AppDBContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        private void init(AppDBContext context, User user)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return;
            
            var otherUser1 = new User("Mike", "mikename",
                imagePath: AvatarsManager.GetUserAvatarPathByUsername("mikename"));
            var chat = new PrivateChat(Guid.NewGuid(), otherUser1, AvatarsManager.GetUserAvatarPathByUsername("mikename"));
            var message1 = new ChatMessage(chat, user, "hello!",DateTime.Now);
            var message2 = new ChatMessage(chat, otherUser1, "hi there", DateTime.Now);
            chat.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.Users.Add(user);
            context.Chats.Add(chat);
            context.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.SaveChanges();

            var otherUser2 = new User("Sam", "samname",
                 imagePath: AvatarsManager.GetUserAvatarPathByUsername("samname"));
            chat = new PrivateChat(Guid.NewGuid(), otherUser2, AvatarsManager.GetUserAvatarPathByUsername("samname"));
            message1 = new ChatMessage(chat, user, "hello!", DateTime.Now);
            message2 = new ChatMessage(chat, otherUser2, "hi there", DateTime.Now);
            chat.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.Chats.Add(chat);
            context.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.SaveChanges();

            var gchat = new GroupChat(Guid.NewGuid(), otherUser2,
                "Lol group",
                AvatarsManager.GetUserAvatarPathByUsername("samname"));
            gchat.Participants.AddRange(new List<Participant>(){new Participant(user, gchat),
                new Participant(otherUser1, gchat) });
            message1 = new ChatMessage(gchat, user, "hello!", DateTime.Now);
            message2 = new ChatMessage(gchat, otherUser2, "hi there", DateTime.Now);
            gchat.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.Chats.Add(gchat);
            context.Messages.AddRange(new List<ChatMessage> { message1, message2 });

            context.SaveChanges();
        }*/
        /*
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ColumnDefinition col1 = this.column1;

            GridLength currentWidth = col1.Width;

            if (currentWidth.GridUnitType == GridUnitType.Pixel)
            {
                double newWidth = currentWidth.Value + e.HorizontalChange;

                double minWidth = 50;
                double maxWidth = this.Width - 100;

                if (newWidth > minWidth && newWidth < maxWidth)
                {
                    col1.Width = new GridLength(newWidth, GridUnitType.Pixel);
                }
            }
            else if (currentWidth.GridUnitType == GridUnitType.Star)
            {
                // Если ширина задана как Star (*), нужно перевести ее в Пиксели для изменения
                // Это сложнее, так как Star зависит от общего размера Grid. 
                // Проще всего при старте задать начальные значения в Pixels, 
                // или же применить логику, основанную на изменении ширины родительского Grid.

                // Для простоты в этом примере мы ожидаем, что начальная ширина Column1 была в Pixels.
                // Если вы хотите использовать Star, нужно:
                // 1. Установить Column1.Width = new GridLength(0, GridUnitType.Pixel);
                // 2. Вычислить, сколько пикселей "отобрал" Thumb (e.HorizontalChange)
                // 3. Применить изменение к Column2, если Column1 использует Star.

                // **Рекомендация**: Для простого перетаскивания границы используйте GridUnitType.Pixel.
            }
        }*/
    }
}