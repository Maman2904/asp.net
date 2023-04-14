using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace tufol.Helpers
{
    public class GlobalFunction
    {
        public void LogError(string title, string message)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");

            try
            {
                if(!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Log")) { Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Log"); }
                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());
                GuidString = rgx.Replace(GuidString, "");
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"Log/" + title + "_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd HHmmss") + "_" + GuidString, message);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        public string GenerateRandomChar(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public string[] ZipArray(string[] string1, string[] string2 ) {
            string [] result = new string[string1.Length];
            // string[] string1 = {"8200", "8300"}
            // string[] string2 = {"2002", "2005" };
            string[] string3 = {};
            Console.WriteLine("string2 type: "+string2);
            if (string2 == null) {
                string[] nullFiller = new string[string1.Length];
                for (int i = 0; i < string1.Length; ++i) {
                    nullFiller[i] = "null";
                }
                string3 = nullFiller;
            } else {
                string3 = string2;
            }
            // foreach (var n in string3){
            //     Console.WriteLine("haloo karakter: "+n);
            // }

            var compact = string1.Zip(string3, (first, second) => first + "," + second);

            // foreach (var item in compact)
            //     Console.WriteLine(item);
            return compact.ToArray();;
        }


        public string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        public String Base64Encode(String plainText)
        {
            if ( String.IsNullOrEmpty(plainText) )
            {
                return null;
            } else
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }
        }
        public String Base64Decode(String base64EncodedData)
        {
            if (String.IsNullOrEmpty(base64EncodedData))
            {
                return null;
            }
            else
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
        }

        public String GetAccountGroupId(int vendor_type_id, string country_id )
        {
            if (country_id == "ID")
                return "a";
            else if ((vendor_type_id == 14 && country_id == "MY") || vendor_type_id == 4 || vendor_type_id == 5 || vendor_type_id == 10 || vendor_type_id == 12 )
                return "b";
            else
                return "c";
        }
        public String SplitSpace(string sapace){
            string[] kode = sapace.Split(" ");
            return kode[0];
        }

        public String Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse( charArray );
            return new string( charArray );
        }

        public String ClearFormatTaxInvoiceNumber(string tax_invoice_number){
            string clear_format = tax_invoice_number.Replace(".", "");
            string format_tax_invoice_number = clear_format.Replace("-", "");
            return format_tax_invoice_number;
        }

        public String FormatTaxInvoiceNumber(string tax_invoice_number, bool is_rpa){
            string tax_invoice_number_1 = tax_invoice_number.Substring(0, 3);
            string tax_invoice_number_2 = tax_invoice_number.Substring(3, 3);
            string tax_invoice_number_3 = tax_invoice_number.Substring(6, 2);
            string tax_invoice_number_4 = tax_invoice_number.Substring(8, 8);

            string format_tax_invoice_number = "";
            if(is_rpa) 
                format_tax_invoice_number = tax_invoice_number_1 + tax_invoice_number_2 + "-" + tax_invoice_number_3 + "." + tax_invoice_number_4;
            else
                format_tax_invoice_number = tax_invoice_number_1 + "." + tax_invoice_number_2 + "-" + tax_invoice_number_3 + "." + tax_invoice_number_4;
            return format_tax_invoice_number;
        }

        public String ClearFormatCurrency(string currency){
            string currency_replace_dot = String.IsNullOrEmpty(currency) ? "0,00" : currency.ToString().Replace(".", "");
            string format_currency = currency_replace_dot.ToString().Replace(",", ".");
            return format_currency;
        }
    }
}