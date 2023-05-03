using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SGHMedicalApi.Common
{

    public class EncryptDecrypt
    {
        public string Encrypt(string toEncrypt, bool useHashing)
        {

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = "$ghG$R0U!54R3aD#262P_#2dafQw24VB2";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = "$ghG$R0U!54R3aD#262P_#2dafQw24VB2";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public string EncryptNewPassword(string toEncrypt, bool useHashing)
        {
            //byte[] keyArray;
            //byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            //string key = "!QAZ@WSX#EDC$RFV%TGB^YHN&UJM*IK<(OL>)P:?";

            //if (useHashing)
            //{
            //    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            //    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //    hashmd5.Clear();
            //}
            //else
            //    keyArray = UTF8Encoding.UTF8.GetBytes(key);

            //TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //tdes.Key = keyArray;
            //tdes.Mode = CipherMode.ECB;
            //tdes.Padding = PaddingMode.PKCS7;

            //ICryptoTransform cTransform = tdes.CreateEncryptor();
            //byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            //tdes.Clear();
            //return Convert.ToBase64String(resultArray, 0, resultArray.Length);


            //Working 
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = "$ghG$R0U!54R3aD#262P_#2dafQw24VB2";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }


        public string DecryptNewPassword(string cipherString, bool useHashing)
        {
            
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = "$ghG$R0U!54R3aD#262P_#2dafQw24VB2";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public string DecryptPassword(string StringToDecrypt)
        {
            double dblCountLength;
            int intLengthChar;
            string strCurrentChar;
            double dblCurrentChar;
            int intCountChar;
            int intRandomSeed;
            int intBeforeMulti;
            int intAfterMulti;
            int intSubNinetyNine;
            int intInverseAsc;
            string decrypt = "";
            int leg = StringToDecrypt.Length;

            for (dblCountLength = 0; dblCountLength < leg; dblCountLength++)
            {
                intLengthChar = int.Parse(@StringToDecrypt.Substring((int)dblCountLength, 1));
                strCurrentChar = @StringToDecrypt.Substring((int)(dblCountLength + 1), intLengthChar);
                dblCurrentChar = 0;
                for (intCountChar = 0; intCountChar < strCurrentChar.Length; intCountChar++)
                {
                    dblCurrentChar = dblCurrentChar + (Convert.ToInt32(char.Parse(strCurrentChar.Substring(intCountChar, 1))) - 33) * (Math.Pow(93, (strCurrentChar.Length - (intCountChar + 1))));
                }

                intRandomSeed = int.Parse(dblCurrentChar.ToString().Substring(2, 2));
                intBeforeMulti = int.Parse(dblCurrentChar.ToString().Substring(0, 2) + dblCurrentChar.ToString().Substring(4, 2));
                intAfterMulti = intBeforeMulti / intRandomSeed;
                intSubNinetyNine = intAfterMulti - 99;
                intInverseAsc = 256 - intSubNinetyNine;
                decrypt += Convert.ToChar(intInverseAsc);
                dblCountLength = dblCountLength + intLengthChar;
            }
            return decrypt;
        }

        public string EnCryptPassword(string StringToEncrypt)
        {
            //  int intMousePointer;
            double dblCountLength;
            int intRandomNumber;
            string strCurrentChar;
            int intAscCurrentChar;
            int intInverseAsc;
            int intAddNinetyNine;
            int dblMultiRandom;
            double dblWithRandom;
            int intCountPower = 0;
            int intPower = 0;
            string strConvertToBase = "";
            string encresult = "";
            int intLowerBounds = 10;
            int intUpperBounds = 28;
            Random Rnd = new Random();

            for (dblCountLength = 0; dblCountLength < @StringToEncrypt.Length; dblCountLength++)
            {
                //Random

                // intRandomNumber = int.Parse((((17) * Rnd.Next()) + intLowerBounds).ToString());
                //intRandomNumber = (intUpperBounds - intLowerBounds + 1) * (Rnd.Next() + intLowerBounds);

                intRandomNumber = Rnd.Next(intLowerBounds, intUpperBounds) + intLowerBounds;
                //Current Char
                strCurrentChar = @StringToEncrypt.Substring(int.Parse(dblCountLength.ToString()), 1);
                //strCurrentChar = (@StringToEncrypt.Substring((int)dblCountLength, 1));
                // intAsc
                //char ascii = char.Parse(strCurrentChar);
                //intAscCurrentChar = (int)ascii;
                intAscCurrentChar = char.Parse(strCurrentChar);

                intInverseAsc = 256 - intAscCurrentChar;
                // add 99
                intAddNinetyNine = intInverseAsc + 99;
                // Multi Random
                dblMultiRandom = intAddNinetyNine * intRandomNumber;
                //With Random
                int dblMultiS = int.Parse(dblMultiRandom.ToString().Substring(0, 2));
                int intRandomNumberS = int.Parse(intRandomNumber.ToString());
                int dblMultiRandomS = int.Parse(dblMultiRandom.ToString().Substring(2, 2));
                string a = (dblMultiS.ToString()) + (intRandomNumberS.ToString()) + (dblMultiRandomS.ToString());
                dblWithRandom = double.Parse(a);
                //dblWithRandom = int.Parse(dblMultiRandom.ToString().Substring(1, 2) + intRandomNumber.ToString() + dblMultiRandom.ToString().Substring(3, 2));
                //dblWithRandom =
                //    int.Parse(dblMultiRandom.ToString().Substring(1, 2))
                //    + intRandomNumber
                //    + int.Parse(dblMultiRandom.ToString().Substring(3, 2));
                //For
                for (intCountPower = 0; intCountPower < 5; intCountPower++)
                {
                    if ((dblWithRandom / (Math.Pow(93, intCountPower)) >= 1))
                    {
                        intPower = intCountPower;
                    }

                    strConvertToBase = "";
                }

                for (intCountPower = intPower; intCountPower >= 0; intCountPower--)
                {

                    int GG = Convert.ToInt32((dblWithRandom / (Math.Pow(93, intCountPower) + 33)));

                    strConvertToBase = strConvertToBase + (char)GG;

                    dblWithRandom = dblWithRandom % (Math.Pow(93, intCountPower));
                }

                encresult = encresult + strConvertToBase.Length + strConvertToBase;
            }

            return encresult;
        }

        public class EncrypPass
        {
            public string EncPass { get; set; }
        }





    }




    public class EncryptDecrypt_NEW
    {
        public string Encrypt(string toEncrypt, bool useHashing)
        {

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = "$ghG$R0U!54R3aD#262P_#2dafQw24VB2";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = "$ghG$R0U!54R3aD#262P_#2dafQw24VB2";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public string EncryptNewPassword(string toEncrypt, bool useHashing)
        {
            //byte[] keyArray;
            //byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            //string key = "!QAZ@WSX#EDC$RFV%TGB^YHN&UJM*IK<(OL>)P:?";

            //if (useHashing)
            //{
            //    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            //    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //    hashmd5.Clear();
            //}
            //else
            //    keyArray = UTF8Encoding.UTF8.GetBytes(key);

            //TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //tdes.Key = keyArray;
            //tdes.Mode = CipherMode.ECB;
            //tdes.Padding = PaddingMode.PKCS7;

            //ICryptoTransform cTransform = tdes.CreateEncryptor();
            //byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            //tdes.Clear();
            //return Convert.ToBase64String(resultArray, 0, resultArray.Length);


            //Working 
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = "$ghG$R0U!54R3aD#262P_#2dafQw24VB2";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }


        public string DecryptNewPassword(string cipherString, bool useHashing)
        {

            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = "$ghG$R0U!54R3aD#262P_#2dafQw24VB2";

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public string DecryptPassword(string StringToDecrypt)
        {
            double dblCountLength;
            int intLengthChar;
            string strCurrentChar;
            double dblCurrentChar;
            int intCountChar;
            int intRandomSeed;
            int intBeforeMulti;
            int intAfterMulti;
            int intSubNinetyNine;
            int intInverseAsc;
            string decrypt = "";
            int leg = StringToDecrypt.Length;

            for (dblCountLength = 0; dblCountLength < leg; dblCountLength++)
            {
                intLengthChar = int.Parse(@StringToDecrypt.Substring((int)dblCountLength, 1));
                strCurrentChar = @StringToDecrypt.Substring((int)(dblCountLength + 1), intLengthChar);
                dblCurrentChar = 0;
                for (intCountChar = 0; intCountChar < strCurrentChar.Length; intCountChar++)
                {
                    dblCurrentChar = dblCurrentChar + (Convert.ToInt32(char.Parse(strCurrentChar.Substring(intCountChar, 1))) - 33) * (Math.Pow(93, (strCurrentChar.Length - (intCountChar + 1))));
                }

                intRandomSeed = int.Parse(dblCurrentChar.ToString().Substring(2, 2));
                intBeforeMulti = int.Parse(dblCurrentChar.ToString().Substring(0, 2) + dblCurrentChar.ToString().Substring(4, 2));
                intAfterMulti = intBeforeMulti / intRandomSeed;
                intSubNinetyNine = intAfterMulti - 99;
                intInverseAsc = 256 - intSubNinetyNine;
                decrypt += Convert.ToChar(intInverseAsc);
                dblCountLength = dblCountLength + intLengthChar;
            }
            return decrypt;
        }

        public string EnCryptPassword(string StringToEncrypt)
        {
            //  int intMousePointer;
            double dblCountLength;
            int intRandomNumber;
            string strCurrentChar;
            int intAscCurrentChar;
            int intInverseAsc;
            int intAddNinetyNine;
            int dblMultiRandom;
            double dblWithRandom;
            int intCountPower = 0;
            int intPower = 0;
            string strConvertToBase = "";
            string encresult = "";
            int intLowerBounds = 10;
            int intUpperBounds = 28;
            Random Rnd = new Random();

            for (dblCountLength = 0; dblCountLength < @StringToEncrypt.Length; dblCountLength++)
            {
                //Random

                // intRandomNumber = int.Parse((((17) * Rnd.Next()) + intLowerBounds).ToString());
                //intRandomNumber = (intUpperBounds - intLowerBounds + 1) * (Rnd.Next() + intLowerBounds);

                intRandomNumber = Rnd.Next(intLowerBounds, intUpperBounds) + intLowerBounds;
                //Current Char
                strCurrentChar = @StringToEncrypt.Substring(int.Parse(dblCountLength.ToString()), 1);
                //strCurrentChar = (@StringToEncrypt.Substring((int)dblCountLength, 1));
                // intAsc
                //char ascii = char.Parse(strCurrentChar);
                //intAscCurrentChar = (int)ascii;
                intAscCurrentChar = char.Parse(strCurrentChar);

                intInverseAsc = 256 - intAscCurrentChar;
                // add 99
                intAddNinetyNine = intInverseAsc + 99;
                // Multi Random
                dblMultiRandom = intAddNinetyNine * intRandomNumber;
                //With Random
                int dblMultiS = int.Parse(dblMultiRandom.ToString().Substring(0, 2));
                int intRandomNumberS = int.Parse(intRandomNumber.ToString());
                int dblMultiRandomS = int.Parse(dblMultiRandom.ToString().Substring(2, 2));
                string a = (dblMultiS.ToString()) + (intRandomNumberS.ToString()) + (dblMultiRandomS.ToString());
                dblWithRandom = double.Parse(a);
                //dblWithRandom = int.Parse(dblMultiRandom.ToString().Substring(1, 2) + intRandomNumber.ToString() + dblMultiRandom.ToString().Substring(3, 2));
                //dblWithRandom =
                //    int.Parse(dblMultiRandom.ToString().Substring(1, 2))
                //    + intRandomNumber
                //    + int.Parse(dblMultiRandom.ToString().Substring(3, 2));
                //For
                for (intCountPower = 0; intCountPower < 5; intCountPower++)
                {
                    if ((dblWithRandom / (Math.Pow(93, intCountPower)) >= 1))
                    {
                        intPower = intCountPower;
                    }

                    strConvertToBase = "";
                }

                for (intCountPower = intPower; intCountPower >= 0; intCountPower--)
                {

                    int GG = Convert.ToInt32((dblWithRandom / (Math.Pow(93, intCountPower) + 33)));

                    strConvertToBase = strConvertToBase + (char)GG;

                    dblWithRandom = dblWithRandom % (Math.Pow(93, intCountPower));
                }

                encresult = encresult + strConvertToBase.Length + strConvertToBase;
            }

            return encresult;
        }

        public class EncrypPass
        {
            public string EncPass { get; set; }
        }





    }


}