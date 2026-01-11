using Client.Connection;
using Client.Data;
using Client.Models;
using Client.ViewModels;
using Client.ViewModels.Patterns;
using Client.Views;
using Microsoft.EntityFrameworkCore;
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

namespace Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var me = new User("Me", "me", 
                imagePath: AvatarsManager
                    .GetChatAvatarPathByUsername("me"));

            var context = new AppDBContext();
            init(context, me);
            IPEndPoint serverIP = IPEndPoint.Parse("127.0.0.1:1234");
            var clientConnection = new ClientConnection(serverIP);
            var mediator = new Mediator();
            var chatService = new ChatService(context, clientConnection);
            var userService = new CurrentUserService(me);
            var mainWindowViewModel = new MainWindowViewModel(mediator,
                chatService, userService);
            DataContext = mainWindowViewModel;
        }
        private void init(AppDBContext context, User user)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            var otherUser = new User("Mike", "mikename",
                imagePath: AvatarsManager.GetChatAvatarPathByUsername("mikename"));
            var chat = new PrivateChat(otherUser, AvatarsManager.GetChatAvatarPathByUsername("mikename"));
            var message1 = new ChatMessage(chat, user, "hello!",DateTime.Now);
            var message2 = new ChatMessage(chat, otherUser, "hi there", DateTime.Now);
            chat.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.Users.Add(user);
            context.Chats.Add(chat);
            context.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.SaveChanges();

            otherUser = new User("Sam", "samname",
                 imagePath: AvatarsManager.GetChatAvatarPathByUsername("samname"));
            chat = new PrivateChat(otherUser, AvatarsManager.GetChatAvatarPathByUsername("samname"));
            message1 = new ChatMessage(chat, user, "hello!", DateTime.Now);
            message2 = new ChatMessage(chat, otherUser, "hi there", DateTime.Now);
            chat.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.Chats.Add(chat);
            context.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.SaveChanges();

            var gchat = new GroupChat(otherUser, "Lol group", AvatarsManager.GetChatAvatarPathByUsername("samname"));
            gchat.Participants.Add(new Participant(user,gchat));
            message1 = new ChatMessage(gchat, user, "hello!", DateTime.Now);
            message2 = new ChatMessage(gchat, otherUser, "hi there", DateTime.Now);
            gchat.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.Chats.Add(gchat);
            context.Messages.AddRange(new List<ChatMessage> { message1, message2 });
            context.SaveChanges();
        }
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