namespace GSM_Designer.Pages
{
    internal interface iCustomNavigationService
    {
        bool IsDialog { get; }
        void ShowWindow(object payload, bool isBacknav = false, bool isDialogPage = false);
        void ShowDialog(object payload);
        //void Navigate(object payload, bool isBacknav = false);
    }
}