using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ControladoShopping.Views
{
    public class NotificationMessage(string value) : ValueChangedMessage<string>(value)
    {
    }
}
