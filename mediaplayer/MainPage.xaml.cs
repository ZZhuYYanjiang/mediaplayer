using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace mediaplayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            DispatcherTimer dTimer = new DispatcherTimer();
            dTimer.Interval = TimeSpan.FromMilliseconds(100);
            dTimer.Start();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await SetLocalMedia();
        }

        async private System.Threading.Tasks.Task SetLocalMedia()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");
            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);

                mediaPlayer.MediaPlayer.Play();
            }
        }
        private void Play_Click_1(object sender, RoutedEventArgs e)
        {
            mediaPlayer.MediaPlayer.Play();
        }

        private void Pause_Click_1(object sender, RoutedEventArgs e)
        {
            mediaPlayer.MediaPlayer.Pause();
        }

        private void Stop_Click_1(object sender, RoutedEventArgs e)
        {
            mediaPlayer.MediaPlayer.Pause();
        }
        private void TxtFilePath_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                TextBox tbPath = sender as TextBox;

                if (tbPath != null)
                {
                    LoadMediaFromString(tbPath.Text);
                }
            }
        }

        private void LoadMediaFromString(string path)
        {
            try
            {
                mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(path));
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    // handle exception. 
                    // For example: Log error or notify user problem with file
                }
            }
        }

        private void DoenLoad_Click(object sender, RoutedEventArgs e)
        {

            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(txtFilePath.Text));
            //  Debug.WriteLine(KnownFolders.MusicLibrary.); // 路径位置

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage response = httpClient.GetAsync(new Uri(txtFilePath.Text)).Result)
                {
                    var filename = txtFilePath.Text.Split('/');
                    Write(filename[filename.Length - 1], response.Content.ReadAsByteArrayAsync().Result);

                }
            }
        }



        private async void Write(string fileName, byte[] html)
        {
            try
            {
                StorageFolder folder = KnownFolders.MusicLibrary;
                StorageFile a = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using (StorageStreamTransaction x = await a.OpenTransactedWriteAsync())
                {
                    using (DataWriter w = new DataWriter(x.Stream))
                    {
                        w.WriteBytes(html);
                        x.Stream.Size = await w.StoreAsync();
                        await x.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
