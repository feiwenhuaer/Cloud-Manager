namespace SupDataDll.UiInheritance
{
    public delegate void CancelDelegate();
    public interface UIDelete
    {
        event CancelDelegate EventCancelDelegate;

        bool AutoClose { get;}
        void UpdateText(string text);

        void SetAutoClose(bool c);

        void ShowDialog_();

        void SetTextButtonCancel(string text);

        void Close_();
    }
}
