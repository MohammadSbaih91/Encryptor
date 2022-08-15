using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Encryptor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            textBox2.Text = EncryptText(textBox1.Text);
            textBox3.Text = DecryptText(textBox2.Text);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = DecryptText(textBox2.Text);
            textBox3.Text = DecryptText(textBox2.Text);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox3.Text;
            textBox2.Text = EncryptText(textBox2.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetLoading(true);
            //this.Invoke((MethodInvoker)delegate
            //{
            OpenFileDialog opnfd = new OpenFileDialog();
            opnfd.Filter = GetImageFilter();
            if (opnfd.ShowDialog() == DialogResult.OK)
            {
                Image image = new Bitmap(opnfd.FileName);
                //Thread.Sleep(4000);
                //ImageConverter Class convert Image object to Byte Array.
                byte[] bytes = (byte[])(new ImageConverter()).ConvertTo(image, typeof(byte[]));

                pictureBox1.Image = Image.FromStream(new MemoryStream(bytes));


                textBox2.Text = Convert.ToBase64String(EncryptFile(bytes));

                textBox3.Text = Convert.ToBase64String(DecryptFile(Convert.FromBase64String(textBox2.Text)));
            }
            SetLoading(false);
            //});
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetLoading(true);
            string file = "C:\\Users\\SobeihMohammadSalehM\\OneDrive - Swarco AG\\Desktop\\image to test\\cbimageEnc.png";
            OpenFileDialog opnfd = new OpenFileDialog();
            if (opnfd.ShowDialog() == DialogResult.OK)
            {
                file = opnfd.FileName;
                byte[] bytesToBeDecrypted = File.ReadAllBytes(file);
                byte[] DecImageBytes = DecryptFile(bytesToBeDecrypted);

                textBox1.Text = Convert.ToBase64String(DecImageBytes);
                textBox2.Text = Convert.ToBase64String(bytesToBeDecrypted);
                textBox3.Text = Convert.ToBase64String(DecImageBytes);
                pictureBox1.Image = ByteToImage(DecImageBytes);
            }


            SetLoading(false);
        }
        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            SetLoading(true);
            //this.Invoke((MethodInvoker)delegate
            //{
            OpenFileDialog opnfd = new OpenFileDialog();
            opnfd.Filter = GetImageFilter();
            if (opnfd.ShowDialog() == DialogResult.OK)
            {
                Image image = new Bitmap(opnfd.FileName);
                //Thread.Sleep(4000);
                //ImageConverter Class convert Image object to Byte Array.
                byte[] bytes = (byte[])(new ImageConverter()).ConvertTo(image, typeof(byte[]));

                pictureBox1.Image = Image.FromStream(new MemoryStream(bytes));


                textBox2.Text = Convert.ToBase64String(EncryptFile(bytes));

                textBox3.Text = Convert.ToBase64String(DecryptFile(Convert.FromBase64String(textBox2.Text)));
            }
            SetLoading(false);
        }
        #region Enc-Dec
        public string EncryptText(string input)
        {
            /// <summary>
            /// The passphrase to encrypt data.
            /// </summary>
            string PassphraseAdapterToEncrypt = "fjn'][43]2{24sdf//.s,>";
            /// <summary>
            /// The salt to encrypt data.
            /// </summary>
            string AdapterSalt = "241fa86763b85341";
            // Get the bytes of the string
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(PassphraseAdapterToEncrypt);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string result = Convert.ToBase64String(bytesEncrypted);

            return result;
        }
        public string DecryptText(string input)
        {
            /// <summary>
            /// The passphrase to encrypt data.
            /// </summary>
            string PassphraseAdapterToEncrypt = "fjn'][43]2{24sdf//.s,>";
            /// <summary>
            /// The salt to encrypt data.
            /// </summary>
            string AdapterSalt = "241fa86763b85341";
            // Get the bytes of the string
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(PassphraseAdapterToEncrypt);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }
        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            /// <summary>
            /// The passphrase to encrypt data.
            /// </summary>
            string PassphraseAdapterToEncrypt = "fjn'][43]2{24sdf//.s,>";
            /// <summary>
            /// The salt to encrypt data.
            /// </summary>
            string AdapterSalt = "241fa86763b85341";
            byte[] encryptedBytes = null;
            string salt = AdapterSalt;

            var saltBytes = Encoding.UTF8.GetBytes(salt);

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            /// <summary>
            /// The passphrase to encrypt data.
            /// </summary>
            string PassphraseAdapterToEncrypt = "fjn'][43]2{24sdf//.s,>";
            /// <summary>
            /// The salt to encrypt data.
            /// </summary>
            string AdapterSalt = "241fa86763b85341";
            byte[] decryptedBytes = null;
            string salt = AdapterSalt;

            var saltBytes = Encoding.UTF8.GetBytes(salt);
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        public byte[] EncryptFile(byte[] bytesToBeEncrypted)
        {
            /// <summary>
            /// The passphrase to encrypt data.
            /// </summary>
            string PassphraseAdapterToEncrypt = "fjn'][43]2{24sdf//.s,>";
            /// <summary>
            /// The salt to encrypt data.
            /// </summary>
            string AdapterSalt = "241fa86763b85341";
            byte[] passwordBytes = Encoding.UTF8.GetBytes(PassphraseAdapterToEncrypt);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            return bytesEncrypted;
        }
        public byte[] DecryptFile(byte[] bytesToBeDecrypted)
        {
            /// <summary>
            /// The passphrase to encrypt data.
            /// </summary>
            string PassphraseAdapterToEncrypt = "fjn'][43]2{24sdf//.s,>";
            /// <summary>
            /// The salt to encrypt data.
            /// </summary>
            string AdapterSalt = "241fa86763b85341";
            byte[] passwordBytes = Encoding.UTF8.GetBytes(PassphraseAdapterToEncrypt);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            return bytesDecrypted;
        }


        public static byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Get the Filter string for all supported image types.
        /// This can be used directly to the FileDialog class Filter Property.
        /// </summary>
        /// <returns></returns>
        public string GetImageFilter()
        {
            StringBuilder allImageExtensions = new StringBuilder();
            string separator = "";
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            Dictionary<string, string> images = new Dictionary<string, string>();
            foreach (ImageCodecInfo codec in codecs)
            {
                allImageExtensions.Append(separator);
                allImageExtensions.Append(codec.FilenameExtension);
                separator = ";";
                images.Add(string.Format("{0} Files: ({1})", codec.FormatDescription, codec.FilenameExtension),
                           codec.FilenameExtension);
            }
            StringBuilder sb = new StringBuilder();
            if (allImageExtensions.Length > 0)
            {
                sb.AppendFormat("{0}|{1}", "All Images", allImageExtensions.ToString());
            }
            images.Add("All Files", "*.*");
            foreach (KeyValuePair<string, string> image in images)
            {
                sb.AppendFormat("|{0}|{1}", image.Key, image.Value);
            }
            return sb.ToString();
        }


        public static byte[] streamToByteArray(Stream input)
        {
            MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }












        #endregion
        private void SetLoading(bool displayLoader)
        {
            if (displayLoader)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    pictureBox2.Visible = true;
                    this.Cursor = Cursors.WaitCursor;
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    pictureBox2.Visible = false;
                    this.Cursor = Cursors.Default;
                });
            }
        }

    }
}
