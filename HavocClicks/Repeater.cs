using System.Threading;
using System.Threading.Tasks;
using HavocsToolkit;

namespace HavocClicks
{
    public class Repeater : MouseHookModule
    {
        public int Repeats { get; set; }
        public int RepeatDelay { get; set; }
        public int PressDuration { get; set; }
        public MouseButtons RepeatedMouseButton { get; set; }

        // Prevents triggering by simulated clicks
        private bool _triggerProtection = false;

        public Repeater()
        {
            _mouseHook.LeftButtonUp += new MouseHook.MouseHookCallback(mouseHook_LeftButtonUp);
            _mouseHook.RightButtonUp += new MouseHook.MouseHookCallback(mouseHook_RightButtonUp);
            _mouseHook.MiddleButtonUp += new MouseHook.MouseHookCallback(mouseHook_MiddleButtonUp);
        }

        private void mouseHook_LeftButtonUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            Handler(MouseButtons.Left);
        }

        private void mouseHook_RightButtonUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            Handler(MouseButtons.Right);
        }

        private void mouseHook_MiddleButtonUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            Handler(MouseButtons.Middle);
        }

        private void Handler(MouseButtons trigger)
        {
            if (_triggerProtection)
                return;

            if (RepeatedMouseButton.HasFlag(trigger))
            {
                _triggerProtection = true;

                Task t = Task.Run(() =>
                {
                    for (int i = 0; i < Repeats; i++)
                    {
                        if (i > 0 && RepeatDelay > 0)
                            Thread.Sleep(RepeatDelay);

                        if (trigger.HasFlag(MouseButtons.Left))
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                        else if (trigger.HasFlag(MouseButtons.Right))
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                        else if (trigger.HasFlag(MouseButtons.Middle))
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.MiddleDown);

                        Thread.Sleep(PressDuration);

                        if (trigger.HasFlag(MouseButtons.Left))
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                        else if (trigger.HasFlag(MouseButtons.Right))
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                        else if (trigger.HasFlag(MouseButtons.Middle))
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.MiddleUp);
                    }

                    _triggerProtection = false;
                });
            }
        }
    }
}
