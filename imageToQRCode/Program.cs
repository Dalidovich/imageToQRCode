using imgToQRCode;

namespace imageToQRCode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var way = @"C:\Users\Ilya\Downloads\ahigaoFinaly.png";
            ImgToQRCodeConverter imgConverter = new ImgToQRCodeConverter(way,"qesda.png");
            imgConverter.createQRCodeAutomatically();
            //imgConverter.createQRCode();
            //imgConverter.test();
        }
    }
}