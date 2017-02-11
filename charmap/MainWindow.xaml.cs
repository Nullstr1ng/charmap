using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace charmap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void cbFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnExportToPNG_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            {
                sfd.Filter = "PNG Files|*.png";
                sfd.ShowDialog();
                if(!string.IsNullOrEmpty(sfd.FileName))
                {
                    ConvertToBitmapSource(this.borderSelChar, sfd.FileName);
                }
            }
        }

        public void ConvertToBitmapSource(UIElement element, string filepathToSave)
        {
            var target = new RenderTargetBitmap(
                (int)element.RenderSize.Width, (int)element.RenderSize.Height,
                96, 96, PixelFormats.Pbgra32);

            //var target = RenderVisual(element);

            target.Render(element);

            var encoder = new PngBitmapEncoder();
            var outputFrame = BitmapFrame.Create(target);
            encoder.Frames.Add(outputFrame);

            using (var file = File.OpenWrite(filepathToSave))
            {
                encoder.Save(file);
            }

            //ToEncodedImage(element, ImageEncodeType.PNG, filepathToSave);
        }

        RenderTargetBitmap RenderVisual(UIElement elt)
        {
            PresentationSource source = PresentationSource.FromVisual(elt);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)elt.RenderSize.Width,
                  (int)elt.RenderSize.Height, 96, 96, PixelFormats.Default);

            VisualBrush sourceBrush = new VisualBrush(elt);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            using (drawingContext)
            {
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0),
                      new Point(elt.RenderSize.Width, elt.RenderSize.Height)));
            }
            rtb.Render(drawingVisual);

            return rtb;
        }

        public enum ImageEncodeType
        {
            BMP, PNG, JPG
        }
        void ToEncodedImage(UIElement elem, ImageEncodeType encodeType, string filename, int JPGQuality = 80)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)elem.DesiredSize.Width, (int)elem.DesiredSize.Height,
                96, 96, PixelFormats.Pbgra32);
            DrawingVisual draw = new DrawingVisual();
            using (DrawingContext dc = draw.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(elem);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(elem.DesiredSize.Width, elem.DesiredSize.Height)));
            }
            rtb.Render(draw);

            using (Stream stream = File.Create(filename))
            {
                switch (encodeType)
                {
                    case ImageEncodeType.BMP:
                        BmpBitmapEncoder bmp = new BmpBitmapEncoder();
                        bmp.Frames.Add(BitmapFrame.Create(rtb));
                        bmp.Save(stream);
                        break;
                    case ImageEncodeType.JPG:
                        JpegBitmapEncoder jpg = new JpegBitmapEncoder()
                        {
                            QualityLevel = JPGQuality
                        };
                        jpg.Frames.Add(BitmapFrame.Create(rtb));
                        jpg.Save(stream);

                        break;
                    case ImageEncodeType.PNG:
                        PngBitmapEncoder png = new PngBitmapEncoder();
                        png.Frames.Add(BitmapFrame.Create(rtb));
                        png.Save(stream);
                        break;
                }
            }
        }
    }
}
