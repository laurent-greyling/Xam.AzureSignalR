using System;
using System.Collections.Generic;
using Watch.Serverless;
using Xamarin.Forms;
using System.Linq;

namespace Watch
{
	public class CheckBox : ContentView
	{
        public string CheckedValue { get; set; }
        public List<string> CategoryLabels { get; set; }

        public CheckBox(
            Array labels,
            Array checkValue = null,
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
                if (checkValue != null && checkValue.Length > 0)
                {
                    foreach (var item in checkValue)
                    {
                        if (item.ToString().Contains(label.ToString()))
                        {
                            categoryLabels.Add(item.ToString());
                        }                        
                    }                    
                    continue;
                }
                if (!label.ToString().Contains("\u2610") || !label.ToString().Contains("\u2611"))
                {
                    categoryLabels.Add(label.ToString().Insert(0, "\u2610 "));
                }
            }

            var dataTemplate = new DataTemplate(() =>
            {
                var templateGrid = new Grid
                {
                    RowSpacing = 1,
                    ColumnSpacing = 1,
                };

                //var checBoxLabel = new Label()
                //{
                //    Text = string.IsNullOrEmpty(checkValue) ? "\u2610" : checkValue,
                //    FontFamily = fontFamily,
                //    FontSize = fontSize * 2,
                //    TextColor = Color.FromHex("#7f8082")
                //};

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

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (sender, args) =>
                {
                    CategoryLabels = categoryLabels;
                    CategoryLabels.Remove(labelText.Text);

                    labelText.Text = labelText.Text.Contains("\u2610") ? labelText.Text.Replace("\u2610", "\u2611") : labelText.Text.Replace("\u2611", "\u2610");
                    
                    CategoryLabels.Add(labelText.Text);
                    
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