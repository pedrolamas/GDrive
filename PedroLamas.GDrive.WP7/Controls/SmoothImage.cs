// Most work on this class is from Jeff Wilcox
// You can find the original file here: https://github.com/jeffwilcox/thejeffwilcox/blob/master/src/Shared/Images/SmoothImage.cs

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PedroLamas.GDrive.Controls
{
    public class SmoothImage : Control
    {
        public event EventHandler FinalImageAvailable;

        private bool _imageIsVisible;

        #region Properties

        public bool AreAnimationsEnabled { get; set; }

        public Stretch Stretch
        {
            get
            {
                return (Stretch)GetValue(StretchProperty);
            }
            set
            {
                SetValue(StretchProperty, value);
            }
        }

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(SmoothImage), new PropertyMetadata(Stretch.None));

        public BitmapImage ActualImageSource
        {
            get
            {
                return GetValue(ActualImageSourceProperty) as BitmapImage;
            }
            set
            {
                SetValue(ActualImageSourceProperty, value);
            }
        }

        public static readonly DependencyProperty ActualImageSourceProperty =
            DependencyProperty.Register("ActualImageSource", typeof(BitmapImage), typeof(SmoothImage), new PropertyMetadata(null));

        public Uri ImageSource
        {
            get
            {
                return (Uri)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(Uri), typeof(SmoothImage), new PropertyMetadata(null, OnImageSourcePropertyChanged));

        private static void OnImageSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (SmoothImage)d;

            var value = (Uri)e.NewValue;

            source.OnImageSourceChanged(value);
        }

        #endregion

        public SmoothImage()
            : base()
        {
            DefaultStyleKey = typeof(SmoothImage);

            AreAnimationsEnabled = true;
        }

        private void OnImageSourceChanged(Uri value)
        {
            var bitmapImage = new BitmapImage()
            {
                CreateOptions = BitmapCreateOptions.BackgroundCreation | BitmapCreateOptions.DelayCreation,
                UriSource = value
            };

            bitmapImage.ImageOpened += OnImageOpened;

            _imageIsVisible = false;

            UpdateVisualStates(false);

            ActualImageSource = bitmapImage;
        }

        public void SetBitmapImage(BitmapImage bitmapImage)
        {
            if (bitmapImage != null)
            {
                bitmapImage.ImageOpened += OnImageOpened;

                _imageIsVisible = false;

                UpdateVisualStates(false);
            }

            ActualImageSource = bitmapImage;
        }

        private void OnImageOpened(object sender, RoutedEventArgs e)
        {
            var bitmapImage = (BitmapImage)sender;

            bitmapImage.ImageOpened -= OnImageOpened;

            _imageIsVisible = true;

            UpdateVisualStates(true);

            OnFinalImageAvailable(EventArgs.Empty);
        }

        protected virtual void OnFinalImageAvailable(EventArgs e)
        {
            var handler = FinalImageAvailable;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void UpdateVisualStates(bool useTransitions)
        {
            if (!AreAnimationsEnabled)
            {
                useTransitions = false;
            }

            VisualStateManager.GoToState(this, _imageIsVisible ? "Loaded" : "Normal", useTransitions);
        }
    }
}