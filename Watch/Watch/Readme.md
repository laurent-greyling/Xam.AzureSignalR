# POC for Nfield CATI watch with SignalR

To run this POC you will need an Azure SignalR connection string. You can do this by creating a [SignalR service in Azure](https://github.com/NIPOSoftwareBV/Nfield-Documentation/tree/master/features/nfield-cati/POC/Xam.AzureSignalR)

This project is a serverless SignalR, so remember to put it to serverless in __Settings__.

Copy your connection string over to the `ClientHandler` and the `ServerHandler`.

This will allow you to start up the project.

To see the functionality work you can do the following:

- Start up 1 - 2 Android emulators, these will be the interviewers
	- currently interviewers are not really added dynamically, so cannot handle more than 3
	- Also in this project we can use android emulators for multi interviewers as we did not look into creating multiple instances of uwp in this POC (though possible)
- start up the UWP app as the manager.
- do interviews and see them appear in manager.
- because of how interviewers are added, you might need to clear the SQLite db in manager side (if things start becoming `Twilight Zone` on you), there is a delete button in manager side in navigation bar (3 dots)
- the manager can also stop listening and then no action of the interviewer will come through.

Will add Gif of how it works once we discussed it in the Demo.

