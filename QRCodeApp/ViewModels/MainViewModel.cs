using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using QRCodeApp.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QRCodeApp.ViewModels
{
    public class MainViewModel : BindableBase, IDisposable
    {
        private string qrCodeContent;
        private Bitmap qrCodeImage;
        private IQRCodeService qrCodeService;

        public string QrCodeContent
        {
            get { return qrCodeContent; }
            set { SetProperty(ref qrCodeContent, value); }
        }

        public ImageSource QrCodeImage
        {
            get { return BitmapToImageSource(qrCodeImage); }
        }

        public DelegateCommand CreateQRCodeCommand { get; }
        public DelegateCommand SaveQRCodeCommand { get; }
        public DelegateCommand LoadQRCodeCommand { get; }
        public DelegateCommand CopyQrCodeToClipboardCommand { get; }
        public DelegateCommand PasteQrCodeFromClipboardCommand { get; }

        public MainViewModel()
        {
            qrCodeService = new QRCodeService();
            CreateQRCodeCommand = new DelegateCommand(CreateQRCode, CanCreateQRCode)
                .ObservesProperty(() => QrCodeContent);
            SaveQRCodeCommand = new DelegateCommand(SaveQRCode, CanSaveQRCode)
                .ObservesProperty(() => QrCodeImage);
            LoadQRCodeCommand = new DelegateCommand(LoadQRCode);
            CopyQrCodeToClipboardCommand = new DelegateCommand(CopyToClipboard, CanCopyToClipboard)
                .ObservesProperty(() => QrCodeImage);
            PasteQrCodeFromClipboardCommand = new DelegateCommand(PasteFromClipboard);
        }

        private void PasteFromClipboard()
        {
            if (!Clipboard.ContainsImage()) return;

            var img = Clipboard.GetImage();
            Bitmap bmp = new Bitmap(
                img.PixelWidth,
                img.PixelHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
                new Rectangle(System.Drawing.Point.Empty, bmp.Size),
                ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            img.CopyPixels(
                Int32Rect.Empty,
                data.Scan0,
                data.Height * data.Stride,
                data.Stride);
            bmp.UnlockBits(data);
            qrCodeImage = bmp;
            QrCodeContent = qrCodeService.GetContent(qrCodeImage);
            RaisePropertyChanged(nameof(QrCodeImage));
        }
        
        private void CopyToClipboard()
        {
            Clipboard.SetImage(BitmapToImageSource(qrCodeService.GetGraphic(qrCodeService.GetContent(qrCodeImage))));
        }

        private bool CanCopyToClipboard()
        {
            return CanSaveQRCode();
        }

        private void LoadQRCode()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "(.png)|*.png"
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var filename = dlg.FileName;
                qrCodeImage = new Bitmap(filename);
                QrCodeContent = qrCodeService.GetContent(qrCodeImage);
                RaisePropertyChanged(nameof(QrCodeImage));
            }
        }

        private void CreateQRCode()
        {
            qrCodeImage = qrCodeService.GetGraphic(QrCodeContent);
            RaisePropertyChanged(nameof(QrCodeImage));
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            if (bitmap == null) return null;

            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                
                return bitmapimage;
            }
        }

        private bool CanCreateQRCode()
        {
            return !string.IsNullOrWhiteSpace(QrCodeContent);
        }

        private void SaveQRCode()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "qrcode_" + DateTime.Now.ToString("ddMMyy_hhmmss");
            dlg.DefaultExt = ".png"; 
            dlg.Filter = "(.png)|*.png"; 
            
            var result = dlg.ShowDialog();
            if (result == true)
            {
                var filename = dlg.FileName;
                qrCodeImage?.Save(filename);
            }
        }

        private bool CanSaveQRCode()
        {
            return QrCodeImage != null;
        }

        public void Dispose()
        {
            qrCodeImage?.Dispose();
        }
    }
}
