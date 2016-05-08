namespace GSM_Designer.Pages
{
    internal interface iCustomNavigationService
    {
        void Navigate(object payload, bool isBacknav = false);
    }
}