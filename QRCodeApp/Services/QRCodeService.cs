using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace QRCodeApp.Services
{
    public class QRCodeService : IQRCodeService
    {
        private BarcodeWriter writer;
        private BarcodeReader reader;

        public QRCodeService()
        {
            writer = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE
            };
            reader = new BarcodeReader();
        }

        public string GetContent(Bitmap bitmap)
        {
            var data = reader.Decode(bitmap);
            if (data == null) return null;
            return data.Text;
        }

        public Bitmap GetGraphic(string content)
        {
            return writer.Write(content);
        }
    }
}
