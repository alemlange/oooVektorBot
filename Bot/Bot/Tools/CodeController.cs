using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using ZXing;

namespace Bot.Tools
{
    public class CodeController
    {
        public static string ReadCode(string fileName)
        {
            try
            {
                var barcodeReader = new BarcodeReader();

                var barcodeBitmap = (Bitmap)Bitmap.FromFile(fileName);
                var barcodeResult = barcodeReader.Decode(barcodeBitmap);

                return barcodeResult.Text;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}