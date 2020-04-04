using OpenQA.Selenium;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace BbbVote.Page
{
    internal class ImageUtils
    {
        internal static object GetImage(IWebDriver driver, string textOfCaptcha, string xpathImage)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var base64string = js.ExecuteScript($@"
                                var c = document.createElement('canvas');
                                var ctx = c.getContext('2d');
                                var img = document.getElementByXpath('{xpathImage}');
                                c.height=img.naturalHeight;
                                c.width=img.naturalWidth;
                                ctx.drawImage(img, 0, 0,img.naturalWidth, img.naturalHeight);
                                var base64String = c.toDataURL();
                                return base64String;
                                ") as string;

            var base64 = base64string.Split(',').Last();
            using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
            {
                using (var bitmap = new Bitmap(stream))
                {
                    var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImageName.png");
                    bitmap.Save(filepath, ImageFormat.Png);
                }
            }

            return 1;
        }
    }
}