using InteliVision.Common;
using InteliVision.Models;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.ProjectOxford.Vision.Contract;
using Plugin.Connectivity;
using Microsoft.ProjectOxford.Vision;

namespace InteliVision.ViewModels
{
    public class PictorialViewModel : BaseViewModel
    {
        public PictorialViewModel()
        {
            Title = "Pictorial";
            visionClient = new VisionServiceClient("229a9f2466f84c3abff08bd37097c1be", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");
        }
        private string _myImage;
        VisionServiceClient visionClient;
        private string _outputText;
        private bool _isProcessing;
        public bool IsProcessing
        {
            get { return _isProcessing; }
            set { SetProperty(ref _isProcessing, value, nameof(IsProcessing)); }
        }
        public string OutputText
        {
            get { return _outputText; }
            set { SetProperty(ref _outputText,value, nameof(OutputText),() => MyImage = string.IsNullOrEmpty(_outputText) ? "" : MyImage ); }
        }

        public ICommand TakePhotoCommand => new Command( OnTakePhotoCommand );
        public ICommand UploadImageCommand => new Command( OnUploadImageCommand );
        public ICommand SpeakTextCommand => new Command( OnSpeakTextCommand);

        private void OnSpeakTextCommand(object obj)
        {
            try
            {
                IsProcessing = true;
                var speaker = ServiceLocator.Instance.Resolve<ITextToSpeech>();
                speaker.Speak(_outputText);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public string MyImage
        {
            get { return _myImage; }
            set { SetProperty(ref _myImage, value, nameof(MyImage)); }
        }
        private async void OnUploadImageCommand(object obj)
        {
            try
            {
                IsProcessing = true;
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    MessagingCenter.Send(this, Constants.MC_Camera, new AlertMessage { Token = Constants.MC_Camera, Title = "No Camera", Message = ":( No camera available.", DialogStyle = "Ok" });
                    return;
                }
                var img = await CrossMedia.Current.PickPhotoAsync();
                if (!string.IsNullOrEmpty(img.Path))
                {
                    MyImage = img.Path;
                }
                if(!CrossConnectivity.Current.IsConnected)
                {
                    OutputText = "Please connect to internet!";
                    return;
                }
                VisionServiceClient vsc = new VisionServiceClient("cdc6a1e7d90147be91069a18f63cc267", "http://westcentralus.api.cognitive.microsoft.com/vision/v1.0");
                var imgStream = img.GetStream();
                var resultData = await vsc.RecognizeTextAsync(imgStream);
                var textualData = PopulateUIWithRegions(resultData);
                ProcessTextualData(textualData);
            }
            catch (Exception ex)
            {
                OutputText = "There was some problem processing data. Try again.";
            }
            finally
            {
                IsProcessing = false;
            }
        }
        private async void OnTakePhotoCommand(object obj)
        {
            try
            {
                IsProcessing = true;
                OutputText = "";
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    MessagingCenter.Send(this, Constants.MC_Camera, new AlertMessage {Token = Constants.MC_Camera, Title="No Camera", Message = ":( No camera available.", DialogStyle="Ok" });
                    return;
                }
                var dt = DateTime.Now;
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = $"img{dt.Day}{dt.Month}{dt.Year}{dt.Hour}{dt.Second}{dt.Millisecond}.jpg"
                });

                if (file == null)
                    return;
                if (!string.IsNullOrEmpty(file.Path))
                {
                    MyImage = file.Path;
                }
                if (!CrossConnectivity.Current.IsConnected)
                {
                    OutputText = "Please connect to internet!";
                    return;
                }

                VisionServiceClient vsc = new VisionServiceClient("cb48e412016a427384b2474931d98582", "http://westcentralus.api.cognitive.microsoft.com/vision/v1.0");
                var imgBytes = file.GetStream();
                var data = await vsc.RecognizeTextAsync(imgBytes);
                var textualData = PopulateUIWithRegions(data);
                ProcessTextualData(textualData);

            }
            catch (Exception ex)
            {
                OutputText = "There was some problem processing data. Try again.";
            }
            finally
            {
                IsProcessing = false;
            }

        }

        private void ProcessTextualData(string textualData)
        {
            if(textualData == null)
            {
                OutputText = "Could not recognize. Try again!";
                return;
            }
            textualData = textualData.Replace('\n', ' ');
            textualData = textualData.ToLower();
            if (textualData.Contains("ten") )
            {
                OutputText = "10 Rupees";
            }
            else if (textualData.Contains("fifty"))
            {
                OutputText = "50 Rupees";
            }
            else if (textualData.Contains("one") && textualData.Contains("hundred") )
            {
                OutputText = "100  Rupees";
            }
            else if (textualData.Contains("five") && textualData.Contains("hundred"))
            {
                OutputText = "500 Rupees";
            }
            else if (textualData.Contains("one") && textualData.Contains("thousand"))
            {
                OutputText = "1000 Rupees";
            }
            else if (textualData.Contains("five") && textualData.Contains("thousand"))
            {
                OutputText = "5000  Rupees";
            }
            else
            {
                OutputText = "Not a currency!";
            }
        }

        private string PopulateUIWithRegions(OcrResults ocrResult)
        {
            string s = null;
            // Iterate the regions
            foreach (var region in ocrResult.Regions)
            {
                // Iterate lines per region
                foreach (var line in region.Lines)
                {
                    // For each line, add a panel
                    // to present words horizontally
                    var lineStack = new StackLayout
                    { Orientation = StackOrientation.Horizontal };
                    // Iterate words per line and add the word
                    // to the StackLayout
                    foreach (var word in line.Words)
                    {
                        s += word.Text + " ";
                        //var textLabel = new Label { Text = word.Text };
                        //lineStack.Children.Add(textLabel);
                    }
                    s += Environment.NewLine;
                    // Add the StackLayout to the UI
                    //this.DetectedText.Children.Add(lineStack);
                }
            }
            return s;
        }
        public byte[] GetImageStreamAsBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        private async Task<AnalysisResult> AnalyzePictureAsync(Stream inputFile)
        {
            // Use the connectivity plug-in to detect
            // if a network connection is available
            // Remember using Plugin.Connectivity directive
            if (!CrossConnectivity.Current.IsConnected)
            {
                MessagingCenter.Send(this,Constants.MC_Camera,new AlertMessage { Token= Constants.MC_Camera ,Title= "Network error" ,DialogStyle="OK",Message = "Please check your network connection and retry." });
                return null;
            }
            VisualFeature[] visualFeatures = new VisualFeature[] { VisualFeature.Adult,
            VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description,
            VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags };
            AnalysisResult analysisResult =
              await visionClient.AnalyzeImageAsync(inputFile,
              visualFeatures);
            return analysisResult;
        }
        
    }
}
