using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interlacer
{
    /// <summary>
    /// trida, ktera uchovava nactene obrazky pro nahledy a stara se o jejich vykresleni
    /// </summary>
    public class PreviewData
    {
        /// <summary>
        /// slovnik, ktery jako klice obsahuje cesty k souborum a jako hodnoty instance tridy Image
        /// </summary>
        private Dictionary<String, Image> images = new Dictionary<String, Image>();
        /// <summary>
        /// instance PictureBoxu, do ktere budou obrazky vykresleny
        /// </summary>
        private PictureBox pictureBox;
        /// <summary>
        /// defaultni obrazek
        /// </summary>
        private Image defaultImage;

        /// <summary>
        /// nastavi PictureBox, do ktereho budou obrazky vykreslovany a defaultni obrazek
        /// </summary>
        /// <param name="pictureBox">instance PictureBoxu</param>
        /// <param name="defaultImage">defaultni obrazek</param>
        public PreviewData(PictureBox pictureBox, Image defaultImage)
        {
            this.pictureBox = pictureBox;
            this.defaultImage = defaultImage;
        }

        /// <summary>
        /// vykresli do PictureBoxu obrazek nacteny ze zadane cesty
        /// </summary>
        /// <param name="path">cesta k obrazku</param>
        public void Show(String path)
        {
            if (images.ContainsKey(path))  //pokud obrazek jiz je ve slovniku
            {
                pictureBox.Image = images[path];  //je zobrazen
            }
            else  //pokud obrazek jeste neni ve slovniku
            {
                int baseSize = 300;
                Image image = Image.FromFile(path);  //nacteni obrazku
                double ratio = image.Width / (double)image.Height;  //zjisteni pomeru stran
                int w = baseSize;
                int h = (int)(baseSize / ratio);
                Bitmap bmp = new Bitmap(image, new Size(w, h));  //nastaveni sirky na 300 a vysky podle pomeru
                images.Add(path, bmp);  //pridani obrazku do slovniku
                pictureBox.Image = bmp;  //zobrazeni obrazku
            }
        }

        /// <summary>
        /// vykresli do PictureBoxu defaultni obrazek
        /// </summary>
        public void ShowDefaultImage()
        {
            pictureBox.Image = defaultImage;
        }
    }
}
