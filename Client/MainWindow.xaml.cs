using Client.Data;
using Client.Models;
using Client.ViewModels;
using Client.ViewModels.Patterns;
using Client.Views;
using Microsoft.EntityFrameworkCore;
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
            var context = new AppDBContext();
            //init(context);
            var chatsList = context.Chats
                .Include(x => x.Messages)
                    .ThenInclude(x=>x.Who)
                .ToList();
                
            //MessageBox.Show(context.Messages.ToList().Count.ToString());
            var mediator = new Mediator();
            var chatPageViewModel = new ChatPageViewModel(mediator);
            var mainPageViewModel = new MainPageViewModel(mediator);
            var mainWindowViewModel = new MainWindowViewModel(mainPageViewModel, chatPageViewModel);
            mainPageViewModel.UpdateChatsList(chatsList);
            DataContext = mainWindowViewModel;
        }
        private void init(AppDBContext context)
        {
            var user = new User()
            {
                Username = "Me"
            };
            var chat = new Chat()
            {
                ChatName = "Chat 1"
            };
            var message = new ChatMessage()
            {
                Message = "Message 1",
                When = DateTime.Now,
                Who = user,
                Chat = chat
            };
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            chat.Messages.Add(message);
            context.Users.Add(user);
            context.Chats.Add(chat);
            context.Messages.Add(message);
            context.SaveChanges();
        }
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            // 1. Получаем ссылку на ColumnDefinition, которую мы хотим изменить
            ColumnDefinition col1 = this.column1;

            // 2. Получаем текущую ширину столбца (как GridLength)
            GridLength currentWidth = col1.Width;

            // 3. Проверяем, что ширина задана в фиксированных единицах (Pixels, т.е. Star/Auto игнорируем для простоты)
            if (currentWidth.GridUnitType == GridUnitType.Pixel)
            {
                double newWidth = currentWidth.Value + e.HorizontalChange;

                // Ограничения: не даем ширине стать слишком маленькой или слишком большой
                double minWidth = 50;
                double maxWidth = this.Width - 100; // Макс ширина окна минус запас

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
        }
    }
}