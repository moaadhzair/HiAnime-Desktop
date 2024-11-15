using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;


namespace HiAnime_Desktop
{
    public partial class PlayerView : Window
    {
        private Episode episode;
        public PlayerView(Episode episode)
        {
            InitializeComponent();
            this.episode = episode;
            initPlayer();
            
        }

        private async void initPlayer()
        {
            await AnimeTools.processEpsiodeServers(episode);
            await webview2.EnsureCoreWebView2Async();
            webview2.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            webview2.Source = new Uri("https://hianime.to");
            
            if (episode.isDubbed)
            {
                await webview2.ExecuteScriptAsync($"window.location.href = '{episode.DubbedServers[0].ServerEmbedLink}';");
                await webview2.CoreWebView2.ExecuteScriptAsync(@"
            document.addEventListener('fullscreenchange', () => {
                if (document.fullscreenElement) {
                    window.chrome.webview.postMessage('enter_fullscreen');
                } else {
                    window.chrome.webview.postMessage('exit_fullscreen');
                }
            });
        ");
            }
            else
            {
                await webview2.ExecuteScriptAsync($"window.location.href = '{episode.SubbedServers[0].ServerEmbedLink}';");
                await webview2.CoreWebView2.ExecuteScriptAsync(@"
            document.addEventListener('fullscreenchange', () => {
                if (document.fullscreenElement) {
                    window.chrome.webview.postMessage('enter_fullscreen');
                } else {
                    window.chrome.webview.postMessage('exit_fullscreen');
                }
            });
        ");
            }
            
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (webview2 != null)
            {
                webview2.Dispose();
            }
        }
        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString();
            if (message == "enter_fullscreen")
            {
                EnterFullscreenMode();
            }
            else if (message == "exit_fullscreen")
            {
                ExitFullscreenMode();
            }
        }
        private void EnterFullscreenMode()
        {
            // Remove borders and taskbar
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            this.Topmost = true;
        }
        private void ExitFullscreenMode()
        {
            // Restore window style and state
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Normal;
            this.Topmost = false;
        }
    }
}
