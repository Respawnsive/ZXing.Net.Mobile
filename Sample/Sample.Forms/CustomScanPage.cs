using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace Sample.Forms
{
    public class CustomScanPage : ContentPage
	{
        readonly ZXingScannerView zxing;
        readonly ZXingDefaultOverlay overlay;

		public CustomScanPage() : base()
		{
			zxing = new ZXingScannerView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				AutomationId = "zxingScannerView",
				Options = new ZXing.Mobile.MobileBarcodeScanningOptions()
				{
					PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.QR_CODE, ZXing.BarcodeFormat.DATA_MATRIX, ZXing.BarcodeFormat.CODE_39, ZXing.BarcodeFormat.CODE_128 }
                }
			};
			zxing.OnScanResult += (result) =>
				Device.BeginInvokeOnMainThread(async () =>
				{

					// Stop analysis until we navigate away so we don't keep reading barcodes
					zxing.IsAnalyzing = false;

					// Show an alert
					await DisplayAlert("Scanned Barcode", result.Text, "OK");

					// Navigate away
					await Navigation.PopAsync();
				});

			overlay = new ZXingDefaultOverlay
			{
				TopText = "Hold your phone up to the barcode",
				BottomText = "Scanning will happen automatically",
				ShowFlashButton = zxing.HasTorch,
				AutomationId = "zxingDefaultOverlay",
			};
			overlay.FlashButtonClicked += (sender, e) =>
			{
				zxing.IsTorchOn = !zxing.IsTorchOn;
			};
			var grid = new Grid
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};
			grid.Children.Add(zxing);
			grid.Children.Add(overlay);

			// The root page of your application
			Content = grid;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			zxing.IsScanning = true;
		}

		protected override void OnDisappearing()
		{
			zxing.IsScanning = false;

			base.OnDisappearing();
		}

        public CameraResolution HandleCameraResolutionSelector(List<CameraResolution> availableResolutions)
        {
            //Don't know if this will ever be null or empty
            if (availableResolutions == null || availableResolutions.Count < 1)
                return new CameraResolution() { Width = 800, Height = 600 };
            var bestresolution = availableResolutions.OrderByDescending(x => x.Height).First();

            return bestresolution;
        }
    }
}
