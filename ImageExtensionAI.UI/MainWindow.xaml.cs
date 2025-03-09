using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.Networking.NetworkOperators;
using Microsoft.Extensions.AI;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI;
using Microsoft.UI.Xaml.Shapes;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ImageExtensionAI.UI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private string _filePath = string.Empty;
        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ImageSummary.Content = "";
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker
                {
                    ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
                };
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    _filePath = file.Path;
                    var bitmapImage = new BitmapImage();
                    using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                    {
                        bitmapImage.SetSource(stream);
                    }
                    SelectedImage.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private async void Analysis_OnClick(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(_filePath))
            {
                ImageSummary.Content = "Please select the image";
                return;
            }

            try
            {
                ProgressRing.IsActive = true;
                ProgressRing.Visibility = Visibility.Visible;

//                IChatClient? chatClient = LLMClient.CreateOllamaClient();

                IChatClient? chatClient = LLMClient.CreateAzureClient();

                var prompt = "Analyze the image, list all the items in it, and provide a summary of the details.";
                prompt += "Raise an alert with 'true' if people are present, otherwise 'false'.";

                var message = new ChatMessage()
                {
                    Text = prompt,
                };

                message.Contents.Add(new ImageContent(await File.ReadAllBytesAsync(_filePath),"image/jpg"));

                var llmFunctionCalling = AIFunctionFactory.Create(LLMCallUserFunction);

                ChatOptions chatOptions = new()
                {
                    Tools = [llmFunctionCalling]
                };

                var response = await chatClient.CompleteAsync([message],chatOptions);

                ImageSummary.Content = response.Message.Text;


            }
            catch (Exception exception)
            {
                throw; // TODO handle exception
            }
            finally
            {
                ProgressRing.IsActive = false;
                ProgressRing.Visibility = Visibility.Collapsed;
            }
        }


        private void LLMCallUserFunction(bool isPeoplePresent,string description)
        {
            ImageCanvas.DispatcherQueue.TryEnqueue(() =>
            {
                if (isPeoplePresent)
                {
                    DrawGreenArrow(SelectedImage.ActualWidth, SelectedImage.ActualHeight);
                }
                else
                {
                    DrawRedArrow(SelectedImage.ActualWidth, SelectedImage.ActualHeight);
                }
            });
        }

        
        private void DrawGreenArrow(double imageWidth, double imageHeight)
        {
            var data = new PathGeometry
            {
                Figures =
                [
                    new PathFigure
                    {
                        StartPoint = new Point(10, 20),
                        Segments =
                        [
                            new LineSegment { Point = new Point(20, 30) },
                            new LineSegment { Point = new Point(40, 10) }
                        ]
                    }
                ]
            };

            Microsoft.UI.Xaml.Shapes.Path tickMark = new Microsoft.UI.Xaml.Shapes.Path
            {
                Stroke = new SolidColorBrush(Colors.Green),
                StrokeThickness = 5,
                Data = data
            };

            Canvas.SetLeft(tickMark, imageWidth - 30); 
            Canvas.SetTop(tickMark, imageHeight - 30); 

            
            ImageCanvas.Children.Add(tickMark);
        }

        private void DrawRedArrow(double imageWidth, double imageHeight)
        {
            
            var crossLine1 = new Line
            {
                X1 = 50,
                Y1 = 50,
                X2 = imageWidth,
                Y2 = imageHeight,
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 3
            };

            ImageCanvas.Children.Add(crossLine1);

            Line crossLine2 = new Line
            {
                X1 = imageWidth, 
                Y1 = 50,  
                X2 = 50,  
                Y2 = imageHeight,
                Stroke = new SolidColorBrush(Colors.Red), 
                StrokeThickness = 3
            };

            ImageCanvas.Children.Add(crossLine2);
        }
    }
}
