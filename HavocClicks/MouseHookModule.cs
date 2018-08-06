using HavocsToolkit;

namespace HavocClicks
{
    public class MouseHookModule
    {
        protected bool _isEnabled = false;
        public bool IsEnabled { get { return _isEnabled; } private set { _isEnabled = value; } }

        protected MouseHook _mouseHook = new MouseHook();

        public MouseHookModule() { }

        public virtual void Enable()
        {
            _isEnabled = true;
            _mouseHook.Install();
        }

        public virtual void Disable()
        {
            _mouseHook.Uninstall();
            _isEnabled = false;
        }
    }
}
