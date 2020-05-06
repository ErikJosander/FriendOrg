namespace FriendOrganizer.UI.View.Services
{
    public interface IMessageDialogService
    {
        void ShowInfoDialog(string text);
        MessageDialogResult ShowOkCancleDialog(string text, string title);
    }
}