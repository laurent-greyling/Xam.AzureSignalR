using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Watch
{
    public class ContentViewBuilder
    {
        public StackLayout Build(string jsonObject, string interviewerId)
        {
            var stack = new StackLayout
            {
                Children =
                {
                    new Label
                    {
                        Text = interviewerId,
                        FontAttributes = FontAttributes.Bold
                    }
                }
            };

            View child;

            foreach (JObject item in JArray.Parse(jsonObject))
            {
                switch ((string)item["Type"])
                {
                    case "Label":
                        {
                            child = new Label
                            {
                                Text = (string)item["Text"]
                            };
                            child.IsEnabled = false;
                            stack.Children.Add(child);
                            break;
                        }
                    case "CheckBox":
                        {
                            child = new CheckBox(item["Categories"].ToArray(), item["SelectedCategries"].ToArray())
                            {
                                IsEnabled = false
                            };
                            stack.Children.Add(child);
                            break;
                        }
                    case "RadioButton":
                        {
                            child = new RadioButton(item["Categories"].ToArray(), (string)item["Text"])
                            {
                                IsEnabled = false
                            };
                            
                            stack.Children.Add(child);
                            break;
                        }
                    case "Entry":
                        {
                            child = new Entry { Text = (string)item["Text"] };
                            child.IsEnabled = false;
                            stack.Children.Add(child);
                            break;
                        }
                    case "Button":
                        {
                            child = new Button
                            {
                                Text = (string)item["Text"]
                            };
                            stack.Children.Add(child);
                            break;
                        }
                }
            }

            return stack;
        }
    }
}
