using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HavocsToolkit;

namespace HavocClicks
{
    public class AutoClicker : MouseHookModule
    {
        public int RepeatDelay { get; set; }
        public int PressDuration { get; set; }
        public MouseButtons ClickedMouseButton { get; set; }

        // Statistics
        public StatisticsRecorder StatisticsRecorder = new StatisticsRecorder();

        // Trigger
        public MouseButtons TriggerMouseButton { get; set; }
        public bool UseKeyboardTrigger { get; set; }
        public string KeyboardTriggerKey { get; set; }

        private KeyboardHook _keyboardHook = new KeyboardHook();

        private bool _interruptFlag = false;

        public AutoClicker()
        {
            _mouseHook.MiddleButtonDown += new MouseHook.MouseHookCallback(mouseHook_MiddleButtonDown);
            _mouseHook.MiddleButtonUp += new MouseHook.MouseHookCallback(mouseHook_MiddleButtonUp);

            _keyboardHook.KeyDown += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);
            _keyboardHook.KeyUp += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyUp);
        }

        public override void Enable()
        {
            base.Enable();

            if (UseKeyboardTrigger)
                _keyboardHook.Install();
        }

        public override void Disable()
        {
            base.Disable();

            if (_keyboardHook.IsActive)
                _keyboardHook.Uninstall();
        }

        private void mouseHook_MiddleButtonDown(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            if (UseKeyboardTrigger)
                return;

            Handler();
        }

        private void mouseHook_MiddleButtonUp(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            if (UseKeyboardTrigger)
                return;

            _interruptFlag = true;
        }

        private void keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {
            if (!UseKeyboardTrigger)
                return;

            if (key.ToString() == KeyboardTriggerKey)
                Handler();
        }

        private void keyboardHook_KeyUp(KeyboardHook.VKeys key)
        {
            if (!UseKeyboardTrigger)
                return;

            if (key.ToString() == KeyboardTriggerKey)
                _interruptFlag = true;
        }

        private Stopwatch _stopWatch = new Stopwatch();

        private void Handler()
        {
            _interruptFlag = false;

            Task t = Task.Run(() =>
            {
                long clickCounter = 0;

                if (ClickedMouseButton.HasFlag(MouseButtons.Left))
                {
                    _stopWatch.Start();
                    while (!_interruptFlag)
                    {
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                        Thread.Sleep(PressDuration);
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                        Thread.Sleep(RepeatDelay);
                        clickCounter++;
                    }
                    _stopWatch.Stop();
                }
                else if (ClickedMouseButton.HasFlag(MouseButtons.Right))
                {
                    _stopWatch.Start();
                    while (!_interruptFlag)
                    {
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                        Thread.Sleep(PressDuration);
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                        Thread.Sleep(RepeatDelay);
                        clickCounter++;
                    }
                    _stopWatch.Stop();
                }

                /*StatisticsRecorder._stopWatch.Start();
                while (!_interruptFlag)
                {
                    if (ClickedMouseButton.HasFlag(MouseButtons.Left))
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    else if (ClickedMouseButton.HasFlag(MouseButtons.Right))
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);

                    Thread.Sleep(PressDuration);

                    if (ClickedMouseButton.HasFlag(MouseButtons.Left))
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                    else if (ClickedMouseButton.HasFlag(MouseButtons.Right))
                        MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);

                    Thread.Sleep(RepeatDelay);
                    clickCounter++;
                }*/

                //StatisticsRecorder._stopWatch.Stop();
                StatisticsRecorder.FinishTest(clickCounter, _stopWatch.ElapsedMilliseconds);
                _stopWatch.Reset();
            });
        }
    }
}
