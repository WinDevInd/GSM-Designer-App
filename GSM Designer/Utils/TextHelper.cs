using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSM_Designer.Utils
{
    public class TextHelper
    {
        public static string TextToPositiveDouble(string text)
        {
            double result;
            text = text.Trim();
            if (double.TryParse(text, out result))
            {
                result = result > 0 ? result : result * -1;
            }
            return result != 0 ? result.ToString() : "";
        }
    }
}
