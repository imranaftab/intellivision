using InteliVision.Common;
using InteliVision.Models;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Plugin.Connectivity;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InteliVision.ViewModels
{
    public class ObjectViewModel : BaseViewModel
    {
        private VisionServiceClient _visionClient;
        private bool _isProcessing;
        private string _myImage;
        private ObservableCollection<string> _objectsInImg;

        public ICommand TakePhotoCommand => new Command(OnTakePhotoCommand);
        public ICommand UploadImageCommand => new Command(OnUploadImageCommand);
        public ObservableCollection<string> ObjectsInImg => _objectsInImg;

        public bool IsProcessing
        {
            get { return _isProcessing; }
            set { SetProperty(ref _isProcessing, value, nameof(IsProcessing)); }
        }
        public string MyImage
        {
            get { return _myImage; }
            set { SetProperty(ref _myImage, value, nameof(MyImage)); }
        }
        private string _selectedTag;
        public string SelectedTag
        {
            get { return _selectedTag; }
            set {
                SetProperty(ref _selectedTag, value, nameof(_selectedTag));
                try
                {
                    IsProcessing = true;
                    var speaker = ServiceLocator.Instance.Resolve<ITextToSpeech>();
                    speaker.Speak(_selectedTag);
                }
                finally
                {
                    IsProcessing = false;
                }
            }
        }
        public ObjectViewModel()
        {
            _visionClient = new VisionServiceClient("cb48e412016a427384b2474931d98582", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");
            _objectsInImg = new ObservableCollection<string>();
        }
        private async void OnTakePhotoCommand(object obj)
        {
            try
            {
                if (IsProcessing)
                    return;

                _objectsInImg.Clear();

                IsProcessing = true;
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    MessagingCenter.Send(this, Constants.MC_Camera, new AlertMessage { Token = Constants.MC_Camera, Title = "No Camera", Message = ":( No camera available.", DialogStyle = "Ok" });
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
                    MessagingCenter.Send(this, Constants.MC_Camera, new AlertMessage { Token = Constants.MC_Camera, Title = "No Internet", Message = "Please connect to Internet.", DialogStyle = "Ok" });
                    return;
                }

                var imgBytes = file.GetStream();
                var data = await _visionClient.DescribeAsync(imgBytes);
                UpdateImgObjectsList(data?.Description?.Tags);

            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, Constants.MC_Camera, new AlertMessage { Token = Constants.MC_Camera, Title = "Error", Message = "Something went wrong. Try again!", DialogStyle = "Ok" });
            }
            finally
            {
                IsProcessing = false;
            }

        }

        private void UpdateImgObjectsList(string[] tags)
        {
            if (tags == null)
            {
                MessagingCenter.Send(this, Constants.MC_Camera, new AlertMessage { Token = Constants.MC_Camera, Title = "No Data Found", Message = "Did not find any information from selected image.", DialogStyle = "Ok" });
                return;
            }
            foreach (var item in tags)
                _objectsInImg.Add(item);

        }

        private async void OnUploadImageCommand(object obj)
        {
            try
            {
                if (IsProcessing)
                    return;
                _objectsInImg.Clear();

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
                if (!CrossConnectivity.Current.IsConnected)
                {
                    MessagingCenter.Send(this, Constants.MC_Camera, new AlertMessage { Token = Constants.MC_Camera, Title = "No Internet", Message = "Please connect to Internet.", DialogStyle = "Ok" });
                    return;
                }
                var imgStream = img.GetStream();
                var resultData = await _visionClient.DescribeAsync(imgStream);
                UpdateImgObjectsList(resultData?.Description?.Tags);
                
            }
            catch (Exception ex)
            {
                MessagingCenter.Send(this, Constants.MC_Camera, new AlertMessage { Token = Constants.MC_Camera, Title = "Error", Message = "Something went wrong. Try again!", DialogStyle = "Ok" });
            }
            finally
            {
                IsProcessing = false;
            }
        }

    }
}
