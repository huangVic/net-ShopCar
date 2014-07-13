using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopCar.Class
{
    public class wfdb
    {


        public string tos(object data)
        {
            if (Convert.IsDBNull(data))
            {
                return "";
            }
            else
            {
                if (data == null) 
                {
                    return "";
                }
                else { 
                   return data.ToString().Trim();
                }
            }
        
        }


        public int toi(object data)
        {
            if (Convert.IsDBNull(data))
            {
                return 0;
            }
            else
            {
                if (data == null)
                {
                    return 0;
                }
                else
                {
                    if (tos(data) == "")
                    {
                        return 0;
                    }
                    else 
                    { 
                      return Convert.ToInt32(data);
                    }
                }
               
            }

        }

        // base64 編碼
        public string base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }

        // base64 解碼
        public string base64Decode(string data)
        {
            System.Diagnostics.Debug.WriteLine(" >>>> base64Decode: " + data);
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }
    }
}