namespace SupDataDll.UiInheritance
{
    public delegate void CancelDelegate();
    public delegate void ClosingDelegate();
    public interface UIDelete
    {
        event CancelDelegate EventCancel;
        event ClosingDelegate EventClosing;
        bool AutoClose { get;}
        void UpdateText(string text);

        void SetAutoClose(bool c);

        void Show_(object owner = null);

        void SetTextButtonCancel(string text);

        void Close_();
    }
}
