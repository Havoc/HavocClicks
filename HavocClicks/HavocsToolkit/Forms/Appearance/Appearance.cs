using System.Windows.Controls;
using System.Windows.Media;

namespace HavocsToolkit.Forms
{
    public static class Appearance
    {
        public enum ButtonAppearance
        {
            kDisabled,
            kEnabled,
        }

        public static void ChangeButtonAppearance(Button button, ButtonAppearance buttonAppearance)
        {
            switch (buttonAppearance)
            {
                case ButtonAppearance.kDisabled:
                    //button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFA1A1"));
                    button.Content = button.Content.ToString().Replace("Disable", "Enable");
                    button.Content = button.Content.ToString().Replace("Stop", "Start");
                    break;
                case ButtonAppearance.kEnabled:
                    //button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAAFFA1"));
                    button.Content = button.Content.ToString().Replace("Enable", "Disable");
                    button.Content = button.Content.ToString().Replace("Start", "Stop");
                    break;
                default:
                    break;
            }
        }

        public static void ChangeButtonBackgroundColor(Button button, string colorHexValue)
        {
            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHexValue));
        }
    }
}
