using System;
using CefSharp;
using Sandbox.Graphics;
using Sandbox.Graphics.GUI;
using VRageMath;
using VRageRender;
using VRageRender.Messages;
using Rectangle = VRageMath.Rectangle;

namespace EnhancedUI.Gui
{
    public partial class ChromiumGuiControl : MyGuiControlBase
    {
        private Chromium? chromium;
        private BatchDataPlayer? player;
        private uint videoId;

        //Returns false if the browser is not initialized else it returns true.
        private bool IsBrowserInitialized => chromium?.Browser.IsBrowserInitialized ?? false;

        private IBrowserHost? BrowserHost => chromium?.Browser.GetBrowser().GetHost();

        public readonly MyGuiControlRotatingWheel Wheel = new(Vector2.Zero)
        {
            Visible = false
        };

        private readonly WebContent content;
        private readonly string name;

        private readonly IPanelState state;

        public ChromiumGuiControl(WebContent content, string name, IPanelState state)
        {
            this.content = content;
            this.name = name;
            this.state = state;

            CanHaveFocus = true;
        }

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            // Create the player only when the exact size of the control is already known
            // FIXME: Verify whether we need to support control re-sizing
            CreatePlayerIfNeeded();
        }

        private void CreatePlayerIfNeeded()
        {
            if (chromium != null)
            {
                return;
            }

            var rect = GetVideoScreenRectangle();
                chromium = new Chromium(new Vector2I(rect.Width, rect.Height), state);

            chromium.Ready += OnChromiumReady;
            chromium.Browser.LoadingStateChanged += OnBrowserLoadingStateChanged;

            RegisterInputEvents();

            state.SetBrowser(chromium.Browser);

            player = new BatchDataPlayer(new Vector2I(rect.Width, rect.Height), chromium.GetVideoData);
            VideoPlayPatch.RegisterVideoPlayer(name, player);
        }

        public override void OnRemoving()
        {
            base.OnRemoving();

            if (chromium == null)
            {
                return;
            }

            UnregisterInputEvents();

            state.SetBrowser(null);

            chromium.Ready -= OnChromiumReady;
            chromium.Browser.LoadingStateChanged -= OnBrowserLoadingStateChanged;

            chromium.Dispose();
            MyRenderProxy.CloseVideo(videoId);

            VideoPlayPatch.UnregisterVideoPlayer(name);

            player = null;
            chromium = null;
        }

        private void OnBrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Wheel.Visible = e.IsLoading;
        }

        private void OnChromiumReady()
        {
            if (chromium == null)
            {
                throw new Exception("This should not happen");
            }

            var url = content.FormatIndexUrl(name);
            chromium.Navigate(url);

            videoId = MyRenderProxy.PlayVideo(VideoPlayPatch.VideoNamePrefix + name, 0);
        }

        // Removes the browser instance when ChromiumGuiControl is no longer needed.

        // Returns the on-screen rectangle of the video player (browser) in pixels

        private Rectangle GetVideoScreenRectangle()
        {
            var pos = (Vector2I)MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(GetPositionAbsoluteTopLeft());

            var size = (Vector2I)MyGuiManager.GetScreenSizeFromNormalizedSize(Size);

            return new Rectangle(pos.X, pos.Y, size.X, size.Y);
        }

        // Renders the HTML document on the screen using the video player
        public override void Draw(float transitionAlpha, float backgroundTransitionAlpha)
        {
            if (!MyRenderProxy.IsVideoValid(videoId))
            {
                return;
            }

            if (chromium == null)
            {
                throw new Exception("This should not happen");
            }

            chromium.Draw();
            MyRenderProxy.UpdateVideo(videoId);
            MyRenderProxy.DrawVideo(videoId, GetVideoScreenRectangle(), new Color(Vector4.One),
                MyVideoRectangleFitMode.AutoFit, false);
        }

        // Reloads the HTML document
        private void ReloadPage()
        {
            state.Reload();
        }

        private void OpenWebDeveloperTools()
        {
            chromium?.Browser.ShowDevTools();
        }

        // Clears the cookies from the CEF browser
        private void ClearCookies()
        {
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
        }

        public override void OnFocusChanged(bool focus)
        {
            BrowserHost?.SetFocus(focus);

            base.OnFocusChanged(focus);
        }

        private void DebugDraw()
        {
            MyGuiManager.DrawBorders(GetPositionAbsoluteTopLeft(), Size, Color.White, 1);
        }
    }
}