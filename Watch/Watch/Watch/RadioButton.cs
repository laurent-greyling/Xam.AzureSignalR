using System;
using System.Collections.Generic;
using Watch.Serverless;
using Xamarin.Forms;

namespace Watch
{
	public class RadioButton : ContentView
	{
        public string CheckedValue { get; set; }

        public RadioButton(Array labels,
            string checkValue = "",
            double fontSize = 20,
            string fontFamily = "Arial",
            FontAttributes fontAttributes = FontAttributes.None,
            Color? textColor = null)
        {
            Padding = new Thickness(0);
            Margin = new Thickness(40, 20, 40, 20);

            var categoryLabels = new List<string>();

            foreach (var label in labels)
            {
                if (!string.IsNullOrEmpty(checkValue) && checkValue.Contains(label.ToString()))
                {
                    categoryLabels.Add(checkValue);
                    continue;
                }
                if (!label.ToString().Contains("\u25CB") || !label.ToString().Contains("\u25CF"))
                {
                    categoryLabels.Add(label.ToString().Insert(0, "\u25CB "));
                }
            }

            var dataTemplate = new DataTemplate(() =>
            {
                var templateGrid = new Grid
                {
                    RowSpacing = 1,
                    ColumnSpacing = 1,
                };

                var labelText = new Label
                {
                    FontFamily = fontFamily,
                    FontSize = fontSize,
                    FontAttributes = fontAttributes,
                    TextColor = textColor ?? Color.Black,
                    VerticalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(10, 0, 10, 0)
                };

                labelText.SetBinding(Label.TextProperty, ".");

                //var checBoxLabel = new Label()
                //{
                //    Text = string.IsNullOrEmpty(checkValue) ? $"\u25CB {labelText.Text}" : $"{checkValue} {labelText.Text}",
                //    FontFamily = fontFamily,
                //    FontSize = fontSize * 2,
                //    TextColor = Color.FromHex("#7f8082")
                //};

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (sender, args) =>
                {
                    labelText.Text = labelText.Text.Contains("\u25CB") ? labelText.Text.Replace("\u25CB", "\u25CF") : labelText.Text.Replace("\u25CF", "\u25CB");
                    CheckedValue = labelText.Text;
                    Clicked?.Invoke(this, args);
                };                

                foreach (var label in labels)
                {
                    templateGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                }

                StackLayout stackLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    Children = { labelText }
                };

                stackLayout.GestureRecognizers.Add(tapGestureRecognizer);
                templateGrid.Children.Add(stackLayout, 0, labels.Length);

                return new ViewCell { View = templateGrid };
            });

            Content = new ListView
            {
                ItemsSource = categoryLabels,
                ItemTemplate = dataTemplate,
                Margin = 5,
                SelectionMode = ListViewSelectionMode.None
            };
        }

        public event EventHandler Clicked;
    }
}