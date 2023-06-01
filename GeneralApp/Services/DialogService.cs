using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace GeneralApp.Services
{
    public class DialogService
    {
        private readonly int SecondsDurationSnackBar = 10;

        private readonly SnackbarOptions snackbarOptionsError;

        private readonly SnackbarOptions snackbarOptionsSuccess;

        private readonly SnackbarOptions snackbarOptionsWarning;

        private readonly SnackbarOptions snackbarOptionsInfo;

        private readonly SnackbarOptions snackbarOptionsNormal;

        public DialogService()
        {

            snackbarOptionsError = new SnackbarOptions
            {
                BackgroundColor = Colors.DarkRed,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.White,
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(12),
                CharacterSpacing = 0
            };

            snackbarOptionsSuccess = new SnackbarOptions
            {
                BackgroundColor = Colors.DarkGreen,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.White,
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(12),
                CharacterSpacing = 0
            };

            snackbarOptionsWarning = new SnackbarOptions
            {
                BackgroundColor = Colors.DarkOrange,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.White,
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(12),
                CharacterSpacing = 0
            };

            snackbarOptionsInfo = new SnackbarOptions
            {
                BackgroundColor = Colors.DarkBlue,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.White,
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(12),
                CharacterSpacing = 0
            };

            snackbarOptionsNormal = new SnackbarOptions
            {
                BackgroundColor = Colors.Black,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.White,
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(12),
                CharacterSpacing = 0
            };

        }

        public async Task SnackbarNormal(string message)
        {
            TimeSpan duration = TimeSpan.FromSeconds(SecondsDurationSnackBar);

            var snackbar = Snackbar.Make(message, null, "Ok", duration, snackbarOptionsNormal);

            await snackbar.Show();
        }

        public async Task SnackbarNormal(string message, string actionButtonText, Action action)
        {
            TimeSpan duration = TimeSpan.FromSeconds(SecondsDurationSnackBar);

            var snackbar = Snackbar.Make(message, action, actionButtonText, duration, snackbarOptionsNormal);

            await snackbar.Show();
        }

        public async Task SnackbarInfo(string message)
        {
            TimeSpan duration = TimeSpan.FromSeconds(SecondsDurationSnackBar);

            var snackbar = Snackbar.Make(message, null, "Ok", duration, snackbarOptionsInfo);

            await snackbar.Show();

        }

        public async Task SnackbarInfo(string message, string actionButtonText, Action action)
        {
            TimeSpan duration = TimeSpan.FromSeconds(SecondsDurationSnackBar);

            var snackbar = Snackbar.Make(message, action, actionButtonText, duration, snackbarOptionsInfo);

            await snackbar.Show();

        }

        public async Task SnackbarSuccessAsync(string message)
        {
            TimeSpan duration = TimeSpan.FromSeconds(SecondsDurationSnackBar);

            var snackbar = Snackbar.Make(message, null, "Ok", duration, snackbarOptionsSuccess);

            await snackbar.Show();
        }

        public async Task SnackbarSuccessAsync(string message, string actionButtonText, Action action)
        {
            TimeSpan duration = TimeSpan.FromSeconds(SecondsDurationSnackBar);

            var snackbar = Snackbar.Make(message, action, actionButtonText, duration, snackbarOptionsSuccess);

            await snackbar.Show();
        }

        public async Task SnackbarWarningAsync(string message)
        {
            TimeSpan duration = TimeSpan.FromSeconds(SecondsDurationSnackBar);

            var snackbar = Snackbar.Make(message, null, "Ok", duration, snackbarOptionsWarning);

            await snackbar.Show();
        }

        public async Task SnackbarWarningAsync(string message, string actionButtonText, Action action)
        {
            TimeSpan duration = TimeSpan.FromSeconds(SecondsDurationSnackBar);

            var snackbar = Snackbar.Make(message, action, actionButtonText, duration, snackbarOptionsWarning);

            await snackbar.Show();
        }

        public async Task SnackbarErrorAsync(string message)
        {
            TimeSpan duration = TimeSpan.FromSeconds(SecondsDurationSnackBar);

            var snackbar = Snackbar.Make(message, null, "Ok", duration, snackbarOptionsError);

            await snackbar.Show();
        }

        public async Task SnackbarErrorAsync(string text, string actionButtonText, Action action)
        {
            TimeSpan duration = TimeSpan.FromSeconds(SecondsDurationSnackBar);

            var snackbar = Snackbar.Make(text, action, actionButtonText, duration, snackbarOptionsError);

            await snackbar.Show();
        }

        public async Task Toast(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            CommunityToolkit.Maui.Core.ToastDuration duration = CommunityToolkit.Maui.Core.ToastDuration.Short;

            double fontSize = 14;

            var toast = CommunityToolkit.Maui.Alerts.Toast.Make(message, duration, fontSize);

            await toast.Show(cancellationTokenSource.Token);
        }

    }
}