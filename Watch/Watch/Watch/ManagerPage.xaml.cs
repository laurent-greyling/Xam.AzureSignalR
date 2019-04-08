using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Serverless;
using Watch.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Watch
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ManagerPage : ContentPage
	{
        readonly ISqliteService<InterviewerEntity> _sqlite;
        private bool switchOnListen;

        public ManagerPage ()
		{
			InitializeComponent ();
            switchOnListen = true;

            ToolbarItems.Add(new ToolbarItem("Delete", "", () =>
            {
                _sqlite.DeleteAll();
            }));

            ToolbarItems.Add(new ToolbarItem("StopListen", "", () =>
            {
                Task.Run(async () => await SendMessage("managerId", "stop"));
                switchOnListen = false;
            }));

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                Task.Run(async () => await SendMessage("managerId", "start"));
                return switchOnListen;
            });

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                return true;
            });

            _sqlite = DependencyService
                .Get<ISqliteService<InterviewerEntity>>();

            var client = new ClientHandler("Manager", "SendMessageInterviewer");

            Task.Run(async () => await client.StartAsync());
            var interviewer = new Label();
            var watchGrid = new Grid();

            client.Message += (sender, e) =>
            {
                if (e.Id != "managerId")
                {
                    var interviewerEntity = new InterviewerEntity { InterviewerId = e.Id };
                    var interviewerExist = _sqlite.Get().Any(x => x.InterviewerId == e.Id);
                    var initialInterviewerCount = _sqlite.Get().Count();

                    if (!interviewerExist)
                    {
                        _sqlite.Add(interviewerEntity);
                    }

                    var interviewerAddedCount = _sqlite.Get().Count();

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var stack = new StackLayout();

                        int numberOfRows = watchGrid.RowDefinitions.Count;
                        var column = 0;
                        var row = 0;

                        if (numberOfRows > 0)
                        {
                            var numberOfChildren = watchGrid.Children.Count;
                            if (interviewerAddedCount % 2 != 0 && initialInterviewerCount != interviewerAddedCount)
                            {
                                watchGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(600) });
                            }

                            for (int childIndex = 0; childIndex < numberOfChildren; ++childIndex)
                            {
                                var view = watchGrid.Children[childIndex];
                                var element = ((StackLayout)view).Children.ElementAt(0);
                                row = Grid.GetRow(watchGrid.Children[childIndex]);
                                column = Grid.GetColumn(watchGrid.Children[childIndex]);

                                if (interviewerAddedCount % 2 != 0 && initialInterviewerCount != interviewerAddedCount)
                                {
                                    row++;
                                }

                                if (((Label)element).Text == e.Id)
                                {
                                    watchGrid.Children.RemoveAt(childIndex);
                                    stack = new ContentViewBuilder().Build(e.Message, e.Id);
                                    watchGrid.Children.Add(stack, column, row);
                                    break;
                                }

                                if (((Label)element).Text != e.Id && row == Grid.GetRow(watchGrid.Children[childIndex]) && initialInterviewerCount != interviewerAddedCount)
                                {
                                    stack = new ContentViewBuilder().Build(e.Message, e.Id);
                                    watchGrid.Children.Add(stack, column + 1, row);
                                    break;
                                }

                                if (((Label)element).Text != e.Id && row != Grid.GetRow(watchGrid.Children[childIndex]) && initialInterviewerCount != interviewerAddedCount)
                                {
                                    stack = new ContentViewBuilder().Build(e.Message, e.Id);
                                    watchGrid.Children.Add(stack, column, row);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            watchGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300) });
                            watchGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300) });
                            watchGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(600) });

                            stack = new ContentViewBuilder().Build(e.Message, e.Id);
                            watchGrid.Children.Add(stack, 0, 0);
                        }

                        Content = watchGrid;
                    });
                }
            };
        }

        private async Task<bool> SendMessage(string Id, string messageBody)
        {
            var message = new MessageModel
            {
                Id = Id,
                Message = messageBody
            };

            var server = new ServerHandler(message, "SendMessageManager");
            await server.Start($"send user Interviewer");

            return switchOnListen;
        }
    }
}