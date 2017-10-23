using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using ZXing;
using System.Text.RegularExpressions;

namespace Bot.Tools
{
    public class CodeController
    {
        protected static Regex regex = new Regex(@"r[0-9]{3}t[0-9]{3}"); // Проверка по маске. Но это не точно!

        public static string ReadCode(string fileName)
        {
            try
            {
                var barcodeReader = new BarcodeReader();

                var barcodeBitmap = (Bitmap)Bitmap.FromFile(fileName);
                var barcodeResult = barcodeReader.Decode(barcodeBitmap);

                barcodeBitmap.Dispose();

                if (regex.IsMatch(barcodeResult.Text))
                {
                    return barcodeResult.Text;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}