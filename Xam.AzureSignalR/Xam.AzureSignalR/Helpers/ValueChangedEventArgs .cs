using System;
using Xamarin.Forms;

namespace Xam.AzureSignalR.Helpers
{
    public class ValueChangedEventArgs : EventArgs
    {
        public string Command { get; set; }
        public double SliderValue { get; set; }
        public Color TextColor { get; set; }
        public double NewValue { get; set; }
        public ValueChangedEventArgs(string command, double sliderValue, Color textColor, double newValue)
        {
            Command = command;
            SliderValue = sliderValue;
            TextColor = textColor;
            NewValue = newValue;
        }
    }
}
