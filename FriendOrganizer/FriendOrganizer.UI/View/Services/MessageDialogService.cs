using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FriendOrganizer.UI.View.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogResult ShowOkCancleDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            return result == MessageBoxResult.OK
                ? MessageDialogResult.OK
                : MessageDialogResult.Cancle;
        }
        public void ShowInfoDialog(string text)
        {
            // Shows a messagebox with the string input and "Info" as Title
            MessageBox.Show(text,"Info");
        }
    }
    public enum MessageDialogResult
    {
        OK,
        Cancle
    }
}
