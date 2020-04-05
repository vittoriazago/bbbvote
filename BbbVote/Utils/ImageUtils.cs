using OpenQA.Selenium;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BbbVote.Page
{
    internal class ImageUtils
    {
        public static string _captchaPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "captchas");
        internal static int GetImageIndex(IWebDriver driver, string captchaName, string xpathImage)
        {
            string path = SaveImage(driver, xpathImage);
            var images = CutImageIntoFive(path);

            int indice = CompareImages(images, captchaName);

            return indice;
        }

        private static int CompareImages(Bitmap[] imagesFromCaptcha, string captchaName)
        {
            string[] imagesFromCaptchaName = GetImagesFromCaptchaName(captchaName);
            for (int indexImageRepo = 0; indexImageRepo < imagesFromCaptchaName.Length; indexImageRepo++)
            {
                var path = Path.Combine(_captchaPath, "tratado", imagesFromCaptchaName[indexImageRepo]);
                var imageFromNameBit = new Bitmap(path);
                for (int indexImageCaptcha = 0; indexImageCaptcha < imagesFromCaptcha.Length; indexImageCaptcha++)
                {
                    if (imageFromNameBit.Size != imagesFromCaptcha[indexImageCaptcha].Size)
                        continue;

                    var hash1 = GetHash(imageFromNameBit);
                    var hash2 = GetHash(imagesFromCaptcha[indexImageCaptcha]);
                    int similiarity = 0;
                    for (int i = 0; i < hash1.Length && i < hash2.Length; i++)
                    {
                        if (hash1[i] == hash2[i])
                            similiarity++;
                    }
                    var percent = similiarity * 100 / Math.Min(hash1.Length, hash2.Length);
                    if (percent > 70)
                        return indexImageCaptcha;
                }
            }
            return -1;
        }

        private static string[] GetImagesFromCaptchaName(string captchaName)
        {
            captchaName = RemoveDiacritics(captchaName);
            var directory = new DirectoryInfo(_captchaPath + "\\tratado");
            var files = directory.GetFiles("*.png");
            return files.Select(f => f.Name).Where(n => n.Contains(captchaName)).ToArray();
        }
        private static string RemoveDiacritics(string input)
        {
            input = Regex.Replace(input, @"\s", "");
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = input.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        }
        public static void ProcessaRetroativo()
        {
            var directory = new DirectoryInfo(_captchaPath);
            var files = directory.GetFiles("*.png");
            foreach (var file in files)
            {
                CutImageIntoFive(Path.Combine(_captchaPath, file.Name));
            }
        }
        public static Bitmap[] CutImageIntoFive(string path)
        {
            Bitmap[] bitmaps = new Bitmap[5];
            Bitmap source = new Bitmap(path);

            int[] array = RecuperaPosicoesQuebra(source);
            for (int indiceParPosicoes = 0, indiceBitMap = 0; 
                                indiceParPosicoes < array.Length; 
                                indiceParPosicoes += 2, indiceBitMap++)
            {
                var dif = array[indiceParPosicoes + 1] - array[indiceParPosicoes];
                if (dif == 0) continue;
                var section = new Rectangle(new Point(array[indiceParPosicoes], 0), new Size(dif, 53));

                var bitmap = CropImage(source, section);
                bitmap = RecuperaPosicoesQuebraAltura(bitmap);
                bitmap = RetiraLinhasPretas(bitmap);
                bitmap = RetiraColunasVazias(bitmap);

                SaveFile(bitmap, out string _, indiceParPosicoes.ToString(), "tratado");
                bitmaps[indiceBitMap] = bitmap;
            }
            return bitmaps;
        }

        private static Bitmap RetiraColunasVazias(Bitmap bitmap)
        {
            int indiceArray = 0;
            int[] corteLargura = new int[2];
            bool colunaAnteriorBranca = true;
            for (int x = 0; x < bitmap.Width; x++)
            {
                bool colunaBranca = true;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.R < 248
                        || pixelColor.G < 248
                        || pixelColor.B < 248)
                    {
                        colunaBranca = false;
                    }
                }
                if (colunaBranca != colunaAnteriorBranca
                     && indiceArray < 2)
                    corteLargura[indiceArray++] = x;

                colunaAnteriorBranca = colunaBranca;
            }
            if (indiceArray != 2)
                return bitmap;

            var dif = corteLargura[1] - corteLargura[0];
            var section = new Rectangle(new Point(corteLargura[0], 0), new Size(dif, bitmap.Height));

            bitmap = CropImage(bitmap, section);
            return bitmap;
        }

        private static Bitmap RetiraLinhasPretas(Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                bool linhaPreta = true;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.R > 0
                        || pixelColor.G > 0
                        || pixelColor.B > 0)
                    {
                        linhaPreta = false;
                    }
                }
                for (int x = 0; x < bitmap.Width && linhaPreta; x++)
                    bitmap.SetPixel(x, y, Color.White);
            }
            return bitmap;
        }

        private static Bitmap RecuperaPosicoesQuebraAltura(Bitmap bitmap)
        {
            var indiceArray = 0;
            bool linhaAnteriorBranca = true;
            int[] corteAltura = new int[2];
            for (int y = 0; y < bitmap.Height; y++)
            {
                bool linhaBranca = true;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.R < 248
                        && pixelColor.G < 248
                        && pixelColor.B < 248)
                    {
                        linhaBranca = false;
                    }
                }
                if (linhaBranca != linhaAnteriorBranca
                    && indiceArray < 2)
                    corteAltura[indiceArray++] = y;

                linhaAnteriorBranca = linhaBranca;
            }

            var difAltura = corteAltura[1] - corteAltura[0];
            if (difAltura < 1) return bitmap;
            var section = new Rectangle(new Point(0, corteAltura[0]), new Size(bitmap.Width, difAltura));
            bitmap = CropImage(bitmap, section);
            return bitmap;
        }

        private static int[] RecuperaPosicoesQuebra(Bitmap source)
        {
            int indiceArray = 0;
            int[] array = new int[10];
            bool colunaAnteriorBranca = true;
            for (int x = 0; x < source.Width; x++)
            {
                bool colunaBranca = true;
                for (int y = 0; y < source.Height; y++)
                {
                    Color pixelColor = source.GetPixel(x, y);
                    if (pixelColor.R < 248
                        || pixelColor.G < 248
                        || pixelColor.B < 248)
                    {
                        colunaBranca = false;
                    }
                }
                if (colunaBranca != colunaAnteriorBranca
                     && indiceArray < 10)
                    array[indiceArray++] = x;

                colunaAnteriorBranca = colunaBranca;
            }
            return array;
        }

        private static void SaveFile(Bitmap bitmap, out string pathFile,
            string indice = "", 
            string folder = "")
        {
            string nomeArquivo = string.Format("{0:yyMMddhhmmss}", DateTime.Now) + indice + ".png";
            pathFile = Path.Combine(_captchaPath, folder, nomeArquivo);
            bitmap.Save(pathFile, ImageFormat.Png);
        }

        private static string SaveImage(IWebDriver driver, string xpathImage)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var base64string = js.ExecuteScript($@"
                                var img = document.evaluate('{xpathImage}', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
                                return img.src;
                                ") as string;

            var base64 = base64string.Split(',').Last();
            var stream = new MemoryStream(Convert.FromBase64String(base64));
            var bitmap = new Bitmap(stream);

            SaveFile(bitmap, out string pathFile);
            return pathFile;
        }

        private static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            if (source.Width < section.Width
                || source.Height < section.Height)
                return source;
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        public static bool[] GetHash(Bitmap bmpSource)
        {
            bool[] lResult = new bool[5];

            Bitmap bmpMin = new Bitmap(bmpSource, new Size(53, 53));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Append(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }

    }
}