using OpenAI;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OpenAI.Models.Images;
using System; 

namespace TranslatorGame.Services
{
    public class ImageService 
    {
        private readonly OpenAiClient _client;
        public ImageService(OpenAiClient client)
        {
            _client = client; 
        }
        public async Task<BitmapImage> GetPictureAsync(string descriprtion)
        {
            if (string.IsNullOrEmpty(descriprtion)) 
                throw new NullReferenceException(nameof(descriprtion));
            if (string.IsNullOrWhiteSpace(descriprtion))
                throw new ArgumentNullException(nameof(descriprtion));

            var imgBytes = await _client
                .GenerateImageBytes(descriprtion, "guessWord", OpenAiImageSize._256);
            Bitmap bmp;
            BitmapImage btmImage;
            using (var ms = new MemoryStream(imgBytes))
            {
                bmp = new Bitmap(ms);
                btmImage = BitmapToImageSource(bmp);
            }
            return btmImage;
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            if (bitmap is null) throw new NullReferenceException(nameof(bitmap));
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
