using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace imgToQRCode
{
    public class ImgToQRCodeConverter
    {
        private Bitmap _image;
        private string _way;
        private byte[] _imageBytes;
        private string _saveWay;
        private string _SaveWay
        {
            get
            {
                return _saveWay;
            }
            set
            {
                if (!value.EndsWith(".png"))
                {
                    _saveWay = value + ".png";
                }
                else
                {
                    _saveWay = value;
                }
            }
        }


        public ImgToQRCodeConverter(string imageWay, string saveWay = "myQRCode.png")
        {
            this._image = new Bitmap(imageWay);
            this._SaveWay = saveWay;
            this._way = imageWay;
            readAsync();
        }
        public string imgToBase64()
        {
            string base64String = Convert.ToBase64String(_imageBytes.ToArray());
            return base64String;
        }
        private async Task readAsync()
        {
            using (FileStream fstream = File.OpenRead(_way))
            {
                _imageBytes = new byte[fstream.Length];
                for (int i = 0; i < fstream.Length; i++)
                {
                    _imageBytes[i] = (byte)fstream.ReadByte();
                }
            }
        }
        private string createPattern()
        {
            return $"data:text/html,<img id=\"a\" src=\"data:image/png;base64,{imgToBase64()}\">";
        }
        private Dictionary<Enum, int> _ECCLevelBytesDictionary = new Dictionary<Enum, int>()
            {
                { QRCodeGenerator.ECCLevel.L, 2953 },//l <=2953 7% may be lost before recovery is not possible
                { QRCodeGenerator.ECCLevel.M, 2331 },//m <=2331 15% may be lost before recovery is not possible
                { QRCodeGenerator.ECCLevel.Q, 1663 },//q <=1663 25% may be lost before recovery is not possible
                { QRCodeGenerator.ECCLevel.H, 1273 } //h <=1273 30% may be lost before recovery is not possible
            };
        public byte[] createQRCodeCustom(QRCodeGenerator.ECCLevel level)
        {
            var pattern = createPattern();
            if(pattern.Length > _ECCLevelBytesDictionary[level])
                return new byte[] { 0 };
            createQRCode(level, pattern);
            return _imageBytes;
        }
        public byte[] createQRCodeAutomatically()
        {
            var pattern = createPattern();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();            
            if (pattern.Length > _ECCLevelBytesDictionary[QRCodeGenerator.ECCLevel.L])
                return new byte[] { 0 };
            createQRCode((QRCodeGenerator.ECCLevel)_ECCLevelBytesDictionary.Where(x => x.Value > pattern.Length).Last().Key, pattern);
            return _imageBytes;
        }
        private void createQRCode(QRCodeGenerator.ECCLevel level,string pattern)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(pattern, level);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            qrCodeImage.Save(_saveWay, ImageFormat.Jpeg);
        }
    }
}
