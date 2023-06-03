using CommunityToolkit.Maui.Alerts;
using GeneralApp.MVVM.ViewModels.ColorPicker;

namespace GeneralApp.MVVM.Views.ColorPicker;

public partial class ColorPickerView : ContentPage
{
	private bool isRandom = false;
	private string hexValue;

	public ColorPickerView(ColorPickerViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
		if (isRandom) return;

		var red = sldRed.Value;
		var green = sldGreen.Value;
		var blue = sldBlue.Value;

		Color color = Color.FromRgb(red, green, blue);

		SetColor(color);
    }

	private void SetColor(Color color)
	{
		btnRandom.BackgroundColor = color;
		Container.BackgroundColor = color;
		hexValue = color.ToHex();
		lblHex.Text = hexValue;
	}

    private void btnRandom_Clicked(object sender, EventArgs e)
    {
		isRandom = true;

		var random = new Random();
		var color = Color.FromRgb(
			random.Next(0,265),
			random.Next(0,265),
			random.Next(0,265)
			);

		SetColor(color);

		sldRed.Value = color.Red;
		sldGreen.Value = color.Green;
		sldBlue.Value = color.Blue;

		isRandom = false;
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
		await Clipboard.SetTextAsync(hexValue);

		var toast = Toast.Make("Color copied", CommunityToolkit.Maui.Core.ToastDuration.Short, 12);
		await toast.Show();
	}
}