using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using HavocsToolkit;
using HavocsToolkit.Forms;

namespace HavocClicks
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Hooks
        private KeyboardHook _keyboardHook = new KeyboardHook();

        // Modules
        private AutoClicker _autoClicker = new AutoClicker();
        private Repeater _repeater = new Repeater();

        private HotKeys _hotkeyTeachingMode = HotKeys.None;
        private HotKeys HotkeyTeachingMode
        {
            get { return _hotkeyTeachingMode; }
            set
            {
                if (value == HotKeys.AutoClicker)
                {
                    textboxHotKeyAutoClicker.BorderThickness = new Thickness(2);
                    if (!_keyboardHook.IsActive)
                        _keyboardHook.Install();
                }
                else if (value == HotKeys.Repeater)
                {
                    textboxHotKeyRepeater.BorderThickness = new Thickness(2);
                    if (!_keyboardHook.IsActive)
                        _keyboardHook.Install();
                }
                else if (value == HotKeys.AutoClickerKeyboardTrigger)
                {
                    tbAutoClickerTriggerKeyboard.BorderThickness = new Thickness(2);
                    if (!_keyboardHook.IsActive)
                        _keyboardHook.Install();
                }
                else
                {
                    textboxHotKeyAutoClicker.ClearValue(BorderThicknessProperty);
                    textboxHotKeyRepeater.ClearValue(BorderThicknessProperty);
                    tbAutoClickerTriggerKeyboard.ClearValue(BorderThicknessProperty);
                    if (_keyboardHook.IsActive)
                        _keyboardHook.Uninstall();
                }
                _hotkeyTeachingMode = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            // Protect hotkey textboxes
            textboxHotKeyRepeater.IsReadOnly = true;
            textboxHotKeyAutoClicker.IsReadOnly = true;
            tbAutoClickerTriggerKeyboard.IsReadOnly = true;
            tbStatisticsClicksPerSecond.IsReadOnly = true;
            tbStatisticsTotalClicks.IsReadOnly = true;
            tbStatisticsTotalTime.IsReadOnly = true;

            // Initialize comboboxes
            // Repeater
            comboboxRepeaterMouseButton.SelectedValuePath = "Key";
            comboboxRepeaterMouseButton.DisplayMemberPath = "Value";
            comboboxRepeaterMouseButton.Items.Add(new KeyValuePair<MouseButtons, string>(MouseButtons.Left, "Left"));
            comboboxRepeaterMouseButton.Items.Add(new KeyValuePair<MouseButtons, string>(MouseButtons.Right, "Right"));
            comboboxRepeaterMouseButton.Items.Add(new KeyValuePair<MouseButtons, string>(MouseButtons.Middle, "Middle"));
            comboboxRepeaterMouseButton.Items.Add(new KeyValuePair<MouseButtons, string>(MouseButtons.Left | MouseButtons.Right, "Left and Right"));
            comboboxRepeaterMouseButton.Items.Add(new KeyValuePair<MouseButtons, string>(MouseButtons.Middle | MouseButtons.Left, "Middle and Left"));
            comboboxRepeaterMouseButton.Items.Add(new KeyValuePair<MouseButtons, string>(MouseButtons.Middle | MouseButtons.Right, "Middle and Right"));
            comboboxRepeaterMouseButton.Items.Add(new KeyValuePair<MouseButtons, string>(MouseButtons.All, "All"));
            comboboxRepeaterMouseButton.SelectedIndex = 0;

            comboboxAutoClickerClickedMouseButton.SelectedValuePath = "Key";
            comboboxAutoClickerClickedMouseButton.DisplayMemberPath = "Value";
            comboboxAutoClickerClickedMouseButton.Items.Add(new KeyValuePair<MouseButtons, string>(MouseButtons.Left, "Left"));
            comboboxAutoClickerClickedMouseButton.Items.Add(new KeyValuePair<MouseButtons, string>(MouseButtons.Right, "Right"));
            comboboxAutoClickerClickedMouseButton.SelectedIndex = 0;

            cbAutoClickerTriggerMouse.SelectedValuePath = "Key";
            cbAutoClickerTriggerMouse.DisplayMemberPath = "Value";
            cbAutoClickerTriggerMouse.Items.Add(new KeyValuePair<MouseButtons, string>(MouseButtons.Middle, "Middle"));
            cbAutoClickerTriggerMouse.SelectedIndex = 0;

            // Initialize Keyboard Hook
            _keyboardHook.KeyDown += new KeyboardHook.KeyboardHookCallback(keyboardHook_KeyDown);

            // Initialize Auto Clicker
            _autoClicker.StatisticsRecorder.UpdateStatistics += new StatisticsRecorder.UpdateStatisticsHandler(UpdateStatistics);
        }

        private void UpdateStatistics(double clicksPerSecond, long totalClicks, long elapsedMilliseconds)
        {
            tbStatisticsClicksPerSecond.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    tbStatisticsClicksPerSecond.Text = string.Format("{0:0.000}", clicksPerSecond);
                    tbStatisticsTotalClicks.Text = totalClicks.ToString();
                    tbStatisticsTotalTime.Text = elapsedMilliseconds.ToString();
                } ));
        }

        private void keyboardHook_KeyDown(KeyboardHook.VKeys key)
        {
            if (HotkeyTeachingMode == HotKeys.None)
            {
                if (key.ToString() == textboxHotKeyAutoClicker.Text.ToString())
                    ToggleAutoClicker();
                else if (key.ToString() == textboxHotKeyRepeater.Text.ToString())
                    ToggleRepeater();
            }
            else if (HotkeyTeachingMode == HotKeys.AutoClicker)
            {
                if (key.ToString() == textboxHotKeyRepeater.Text.ToString() || key.ToString() == tbAutoClickerTriggerKeyboard.Text.ToString())
                {
                    ShowHotkeyAlreadyInUse();
                    return;
                }       

                textboxHotKeyAutoClicker.Text = key.ToString();
                HotkeyTeachingMode = 0;
            }
            else if (HotkeyTeachingMode == HotKeys.Repeater)
            {
                if (key.ToString() == textboxHotKeyAutoClicker.Text.ToString() || key.ToString() == tbAutoClickerTriggerKeyboard.Text.ToString())
                {
                    ShowHotkeyAlreadyInUse();
                    return;
                }

                textboxHotKeyRepeater.Text = key.ToString();
                HotkeyTeachingMode = HotKeys.None;
            }
            else if (HotkeyTeachingMode == HotKeys.AutoClickerKeyboardTrigger)
            {
                if (key.ToString() == textboxHotKeyAutoClicker.Text.ToString() || key.ToString() == textboxHotKeyRepeater.Text.ToString())
                {
                    ShowHotkeyAlreadyInUse();
                    return;
                }

                tbAutoClickerTriggerKeyboard.Text = key.ToString();
                HotkeyTeachingMode = HotKeys.None;
            }
        }

        private void ShowHotkeyAlreadyInUse()
        {
            MessageBox.Show("Your selected HotKey is already in use! Try another one or make sure your selected HotKey is not used.", "HavocClicks");
            HotkeyTeachingMode = HotKeys.None;
        }

        private void ToggleAutoClicker()
        {
            if (_repeater.IsEnabled)
                ToggleRepeater();

            if (!_autoClicker.IsEnabled)
            {
                // Lock ui-controls
                textboxAutoClickerDelay.IsEnabled = false;
                textboxAutoClickerPressDuration.IsEnabled = false;
                comboboxAutoClickerClickedMouseButton.IsEnabled = false;
                cbAutoClickerTriggerMouse.IsEnabled = false;
                tbAutoClickerTriggerKeyboard.IsEnabled = false;
                rbAutoClickerTriggerMouse.IsEnabled = false;
                rbAutoClickerTriggerKeyboard.IsEnabled = false;

                // Apply settings
                _autoClicker.RepeatDelay = Convert.ToInt32(textboxAutoClickerDelay.Text);
                _autoClicker.PressDuration = Convert.ToInt32(textboxAutoClickerPressDuration.Text);
                var clickedMouseButton = (KeyValuePair<MouseButtons, string>)comboboxAutoClickerClickedMouseButton.SelectedItem;
                _autoClicker.ClickedMouseButton = clickedMouseButton.Key;
                var triggerMouseButton = (KeyValuePair<MouseButtons, string>)cbAutoClickerTriggerMouse.SelectedItem;
                _autoClicker.TriggerMouseButton = triggerMouseButton.Key;
                _autoClicker.UseKeyboardTrigger = rbAutoClickerTriggerKeyboard.IsChecked.Value;
                _autoClicker.KeyboardTriggerKey = tbAutoClickerTriggerKeyboard.Text;

                _autoClicker.Enable();
                Appearance.ChangeButtonAppearance(buttonEnableAutoClicker, Appearance.ButtonAppearance.kEnabled);
            }
            else
            {
                // Unlock ui-controls
                textboxAutoClickerDelay.IsEnabled = true;
                textboxAutoClickerPressDuration.IsEnabled = true;
                comboboxAutoClickerClickedMouseButton.IsEnabled = true;
                cbAutoClickerTriggerMouse.IsEnabled = true;
                tbAutoClickerTriggerKeyboard.IsEnabled = true;
                rbAutoClickerTriggerMouse.IsEnabled = true;
                rbAutoClickerTriggerKeyboard.IsEnabled = true;

                _autoClicker.Disable();
                Appearance.ChangeButtonAppearance(buttonEnableAutoClicker, Appearance.ButtonAppearance.kDisabled);
            }
        }

        private void ToggleRepeater()
        {
            if (_autoClicker.IsEnabled)
                ToggleAutoClicker();

            if (!_repeater.IsEnabled)
            {
                // Lock ui-controls
                textboxClickRepeats.IsEnabled = false;
                textboxRepeatDelay.IsEnabled = false;
                textboxPressDuration.IsEnabled = false;
                comboboxRepeaterMouseButton.IsEnabled = false;

                // Apply settings
                _repeater.Repeats = Convert.ToInt32(textboxClickRepeats.Text);
                _repeater.RepeatDelay = Convert.ToInt32(textboxRepeatDelay.Text);
                _repeater.PressDuration = Convert.ToInt32(textboxPressDuration.Text);
                var mouseButton = (KeyValuePair<MouseButtons, string>)comboboxRepeaterMouseButton.SelectedItem;
                _repeater.RepeatedMouseButton = mouseButton.Key;

                _repeater.Enable();
                Appearance.ChangeButtonAppearance(buttonEnableRepeater, Appearance.ButtonAppearance.kEnabled);
            }
            else
            {
                // Unlock ui-controls
                textboxClickRepeats.IsEnabled = true;
                textboxRepeatDelay.IsEnabled = true;
                textboxPressDuration.IsEnabled = true;
                comboboxRepeaterMouseButton.IsEnabled = true;

                _repeater.Disable();
                Appearance.ChangeButtonAppearance(buttonEnableRepeater, Appearance.ButtonAppearance.kDisabled);
            }
        }

        private void ToggleHotkey()
        {
            if (!_keyboardHook.IsActive)
            {
                _keyboardHook.Install();
                Appearance.ChangeButtonAppearance(buttonEnableHotkeys, Appearance.ButtonAppearance.kEnabled);
            }
            else
            {
                _keyboardHook.Uninstall();
                Appearance.ChangeButtonAppearance(buttonEnableHotkeys, Appearance.ButtonAppearance.kDisabled);
            }
        }

        private void buttonEnableAutoClicker_Click(object sender, RoutedEventArgs e)
        {
            ToggleAutoClicker();
        }

        private void buttonEnableRepeater_Click(object sender, RoutedEventArgs e)
        {
            ToggleRepeater();
        }

        private void buttonEnableHotkeys_Click(object sender, RoutedEventArgs e)
        {
            ToggleHotkey();
        }

        private void textboxHotkeyAutoClicker_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HotkeyTeachingMode = HotKeys.AutoClicker;
        }

        private void textboxHotkeyRepeater_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HotkeyTeachingMode = HotKeys.Repeater;
        }

        private void tbAutoClickerTriggerKeyboard_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HotkeyTeachingMode = HotKeys.AutoClickerKeyboardTrigger;
        }

        private void textboxNumerical_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            // Filter for numerics
            if (System.Text.RegularExpressions.Regex.IsMatch(tb.Text, "[^0-9]"))
            {
                tb.Text = tb.Text.Remove(tb.Text.Length - 1);
                tb.CaretIndex = tb.Text.Length;
            }

            // Default value of 0
            if (tb.Text.Length < 1)
            {
                tb.Text = "0";
                tb.CaretIndex = tb.Text.Length;
            }
            else
            {
                // Remove leading zeros
                string tbText = string.Copy(tb.Text);
                for (; tbText[0] == '0' && tbText.Length > 1;)
                    tbText = tbText.Remove(0, 1);
                tb.Text = tbText;
                if (tb.Text.Length == 1)
                    tb.CaretIndex = tb.Text.Length;
            }
        }
    }
}
