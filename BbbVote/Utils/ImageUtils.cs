using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
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
                var path = Path.Combine(_captchaPath, imagesFromCaptchaName[indexImageRepo]);
                var imageFromNameBit = new Bitmap(path);
                for (int indexImageCaptcha = 0; indexImageCaptcha < imagesFromCaptcha.Length; indexImageCaptcha++)
                {
                    if (imageFromNameBit.Size != imagesFromCaptcha[indexImageCaptcha].Size)
                        continue;

                    var hash1 = GetHash(imageFromNameBit);
                    var hash2 = GetHash(imagesFromCaptcha[indexImageCaptcha]);
                    bool different = false;
                    for (int i = 0; i < hash1.Length && i < hash2.Length; i++)
                    {
                        if (hash1[i] != hash2[i])
                        {
                            different = true;
                            break;
                        }
                    }
                    if (!different)
                        return indexImageCaptcha;
                }
            }
            return -1;
        }

        private static string[] GetImagesFromCaptchaName(string captchaName)
        {
            captchaName = RemoveDiacritics(captchaName);
            var directory = new DirectoryInfo(_captchaPath);
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

        private static Bitmap[] CutImageIntoFive(string path)
        {
            Bitmap[] bitmaps = new Bitmap[5];

            Bitmap source = new Bitmap(path);
            for (int i = 0; i < 5; i++)
            {
                Rectangle section = new Rectangle(new Point(i * 53, 0), new Size(53, 53));

                var bitmap = CropImage(source, section);
                SaveFile(bitmap, out string pathFile, i.ToString());

                bitmaps[i] = bitmap;
            }
            return bitmaps;
        }

        private static void SaveFile(Bitmap bitmap, out string pathFile, string indice = "")
        {
            string nomeArquivo = string.Format("{0:yyMMddhhmmss}", DateTime.Now) + indice + ".png";
            pathFile = Path.Combine(_captchaPath, nomeArquivo);
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