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
        public Bitmap image;
        public string imageWay;
        public byte[] imageBytes;
        private string saveWay;
        public string SaveWay
        {
            get
            {
                return saveWay;
            }
            set
            {
                if (!value.EndsWith(".png"))
                {
                    saveWay = value + ".png";
                }
                else
                {
                    saveWay = value;
                }
            }
        }


        public ImgToQRCodeConverter(string imageWay, string saveWay = "myQRCode.png")
        {
            this.image = new Bitmap(imageWay);
            this.SaveWay = saveWay;
            this.imageWay = imageWay;
            readAsync();
        }
        public string imgToBase64()
        {
            string base64String = Convert.ToBase64String(imageBytes.ToArray());
            return base64String;
        }
        private async Task readAsync()
        {
            using (FileStream fstream = File.OpenRead(imageWay))
            {
                imageBytes = new byte[fstream.Length];
                for (int i = 0; i < fstream.Length; i++)
                {
                    imageBytes[i] = (byte)fstream.ReadByte();
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
        public string createQRCodeCustom(QRCodeGenerator.ECCLevel level)
        {
            var pattern = createPattern();
            if(pattern.Length > _ECCLevelBytesDictionary[level])
                return "error: big picture";
            createQRCode(level, pattern);
            return pattern;
        }
        public string createQRCodeAutomatically()
        {
            var pattern = createPattern();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();            
            if (pattern.Length > _ECCLevelBytesDictionary[QRCodeGenerator.ECCLevel.L])
                return "error: big picture";
            createQRCode((QRCodeGenerator.ECCLevel)_ECCLevelBytesDictionary.Where(x => x.Value > pattern.Length).Last().Key, pattern);
            return pattern;
        }
        private void createQRCode(QRCodeGenerator.ECCLevel level,string pattern)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(pattern, level);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            qrCodeImage.Save(saveWay, ImageFormat.Jpeg);
        }
    }
}
