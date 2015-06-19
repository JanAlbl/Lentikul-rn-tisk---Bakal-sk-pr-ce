using GfxlibWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Interlacer
{
    /// <summary>
    /// trida hlavniho formulare, obsahuje vsechny metody udalosti na uzivatelske akce
    /// </summary>
    public partial class MainForm : Form
    {
        private void Form1_Load(object sender, EventArgs e)
        {         
            DoubleBuffered = true;
        }

        /// <summary>
        /// Metoda vyvolana pri kliknuti na tlacitko pro otoceni listu obrazku.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reverseButton_Click(object sender, EventArgs e)
        {
            int count = pictureListViewEx.Items.Count;
            for (int i = 0; i < count / 2; i++)
            {
                String tmp;
                for (int j = 1; j < pictureListViewEx.Items[i].SubItems.Count; j++)
                {
                    tmp = pictureListViewEx.Items[i].SubItems[j].Text;
                    pictureListViewEx.Items[i].SubItems[j].Text = pictureListViewEx.Items[count - i - 1].SubItems[j].Text;
                    pictureListViewEx.Items[count - i - 1].SubItems[j].Text = tmp;
                }
            }
        }

        /// <summary>
        /// Metoda vyvolaná při změně šířky obrázku.
        /// Ve třídě InterlacingData se nastaví nově nastavená šířka a všem komponentám se obnoví jejich hodnoty.
        /// Hodnoty se obnoví, protože šířka má vliv například na celkovou šířku v pixelech, takže je potřeba ji znova přepočítat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void widthNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetWidth(Convert.ToDouble(widthNumeric.Value));
            updateAllComponents();
        }

        /// <summary>
        /// Metoda vyvolaná při změně výšky obrázku.
        /// Ve třídě InterlacingData se nastaví nově nastavená šířka a všem komponentám se obnoví jejich hodnoty.
        /// Hodnoty se obnoví, protože šířka má vliv například na celkovou šířku v pixelech, takže je potřeba ji znova přepočítat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void heightNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetHeight(Convert.ToDouble(heightNumeric.Value));
            updateAllComponents();
        }

        /// <summary>
        /// Metoda vyvolaná zaškrtnutím/odškrtnutím checkboxu pro zachování poměru stran.
        /// Ve třídě InterlacingData se nastaví proměnná keepAspectRatio na true/false.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keepRatioCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().KeepAspectRatio(keepRatioCheckbox.Checked);
        }

        /// <summary>
        /// Timer, který se zapne po přetáhnutí položky/položek v listView drag and dropem.
        /// Jednotlivým položkám se znova nastaví jejich pořadi a timer se vypne.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reorderTimer_Tick(object sender, EventArgs e)
        {
            reorder();
            reorderTimer.Stop();
        }

        /// <summary>
        /// nastavi nahled a informace o obrazku v dolnim pravem rohu formulare pri zmene vyberu radku v seznamu obrazku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureListViewEx_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (imagePreviewCheckBox.Checked)
                setPreview();
            setPictureInfo();
        }

        /// <summary>
        /// Metoda vyvolená při změně položky v comboboxu pro první interpolační algoritmus.
        /// Nastaví hodnotu výchozího filtru pro první Interpolaci
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void interpol1ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            FilterType filter = ((StringValuePair<FilterType>)c.SelectedItem).value;
            projectData.GetInterlacingData().SetInitialResizeFilter(filter);
        }

        /// <summary>
        /// Metoda vyvolená při změně položky v comboboxu pro druhý interpolační algoritmus.
        /// Nastaví hodnotu výchozího filtru pro druhou Interpolaci
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void interpol2ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            FilterType filter = ((StringValuePair<FilterType>)c.SelectedItem).value;
            projectData.GetInterlacingData().SetFinalResampleFilter(filter);
        }

        /// <summary>
        /// Metoda vyvolaná změnou hodnoty DPI.
        /// Ve tříde InterlacingData se přenastaví hodnota DPI na nově zadanou hodnotu a všechny komponenty se updatnou na nové hodnoty, 
        /// které DPI mohlo pozměnit. Například šířka a výška výsledného obrázku v pixelech.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dpiNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetPictureResolution(Convert.ToDouble(dpiNumeric.Value));
            updateAllComponents();
        }

        /// <summary>
        /// Metoda vyvolaná změnou hodnoty LPI.
        /// Ve tříde InterlacingData se přenastaví hodnota LPI na nově zadanou hodnotu a všechny komponenty se updatnou na nové hodnoty, 
        /// které LPI mohlo pozměnit. Například počet obrázků pod lentikulí.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lpiNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetLenticuleDensity(Convert.ToDouble(lpiNumeric.Value));
            updateAllComponents();
        }

        /// <summary>
        /// Metoda vyvolaná změnou hodnoty sířku rámečku u vodících čar.
        /// Ve třídě LineData se nastaví nově zadaná šířka rámečku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frameWidthNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetFrameWidth(Convert.ToDouble(frameWidthNumeric.Value));
        }

        /// <summary>
        /// Metoda vyvolaná změnou hodnoty odsazení vodících čar od obrázku.
        /// Ve třídě LineData se nastaví nově zadaná šířka rámečku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void indentNumeric_ValueChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetIndent(Convert.ToDouble(indentNumeric.Value));
        }

        /// <summary>
        /// Metoda vyvolaná změnou hodnoty checkboxu určujícího zda se mají vykreslit horní vodící čáry.
        /// Ve třídě LineData se nastaví true/false podle toho zda je checkbox zaškrtlý nebo ne.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void topLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetTop(topLineCheckBox.Checked);
        }

        /// <summary>
        /// Metoda vyvolaná změnou hodnoty checkboxu určujícího zda se mají vykreslit dolní vodící čáry.
        /// Ve třídě LineData se nastaví true/false podle toho zda je checkbox zaškrtlý nebo ne.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bottomLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetBottom(bottomLineCheckBox.Checked);
        }

        /// <summary>
        /// Metoda vyvolaná změnou hodnoty checkboxu určujícího zda se mají vykreslit levé vodící čáry.
        /// Ve třídě LineData se nastaví true/false podle toho zda je checkbox zaškrtlý nebo ne.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void leftLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetLeft(leftLineCheckBox.Checked);
        }

        /// <summary>
        /// Metoda vyvolaná změnou hodnoty checkboxu určujícího zda se mají vykreslit pravé vodící čáry.
        /// Ve třídě LineData se nastaví true/false podle toho zda je checkbox zaškrtlý nebo ne.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rightLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetRight(rightLineCheckBox.Checked);
        }

        /// <summary>
        /// Metoda vyvolaná při stisku radiobuttonu určujícího zda mají být vodící čáry na středu lentikule.
        /// Pokud je zaškrtlé, vodící čáry budou na středu lentikule.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void centerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetCenterPosition(true);
            drawLineThickness();
        }

        /// <summary>
        /// Metoda vyvolaná při stisku radiobuttonu určujícího zda mají být vodící čáry na okraji lentikule.
        /// Pokud je zaškrtlé, vodící čáry budou na okraji lentikule.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edgeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetLineData().SetCenterPosition(false);
            drawLineThickness();
        }

        /// <summary>
        /// Tlačítko, které otevře panel s výběrem barev pro vodící čáry obrázku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lineColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog lc = new ColorDialog();
            if (lc.ShowDialog() == DialogResult.OK)
            {
                lineColorButton.BackColor = lc.Color;
                projectData.GetLineData().SetLineColor(lc.Color);

                drawLineThickness();
            }
        }

        /// <summary>
        /// Tlačítko, které otevře panel s výběrem barev pro pozadí vodících čar obrázku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog bc = new ColorDialog();
            if (bc.ShowDialog() == DialogResult.OK)
            {
                backgroundColorButton.BackColor = bc.Color;
                projectData.GetLineData().SetBackgroundColor(bc.Color);

                drawLineThickness();
            }
        }

        /// <summary>
        /// Metoda vyvolaná posouváním jezdce na trackbaru určujícího šířku vodících čar.
        /// Změní hodnotu labelu určujícího aktuálně vybranou šířku čar.
        /// Ve třídě LineData nastaví nově zadanou šířku čar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lineThicknessTrackbar_ValueChanged(object sender, EventArgs e)
        {
            actualPicsUnderLenLabel.Text = Convert.ToString(lineThicknessTrackbar.Value);
            projectData.GetLineData().SetLineThickness(lineThicknessTrackbar.Value);

            drawLineThickness();
        }

        /// <summary>
        /// Nastaví směr prokládání na vertikální
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void verticalRadiobutton_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetDirection(Direction.Vertical);
        }
        /// <summary>
        /// Nastaví směr prokládání na horizontální
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void horizontalRadiobutton_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetDirection(Direction.Horizontal);
        }

        /// <summary>
        /// Pro označené položky seznamu vytvoří určitý počet kopií.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyPicButton_Click(object sender, EventArgs e)
        {
            copyPictures(Convert.ToInt32(copyCountNumeric.Value));
        }

        /// <summary>
        /// Metoda vyvolaná při zaškrtnutí/odškrtnutí checkboxu pro zobrazení náhledu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imagePreviewCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (imagePreviewCheckBox.Checked)
            {
                setPreview();
            }
            else
            {
                previewData.ShowDefaultImage();
            }
        }

        /// <summary>
        /// Metoda vyvolaná při tažení souboru přes hlavní formulář.
        /// Ověří se, zda přetahovaný soubor/soubory má/mají validní koncovku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                int lastIndnex = pictureListViewEx.Items.Count;
                foreach (string path in filePaths)
                {
                    if (isExtensionValid(path))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Pokud se s nějakou položkou seznamu pohne drag and dropem. Zapne se timer, který volá metodu na očíslování položek seznamu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureListViewEx_DragDrop(object sender, DragEventArgs e)
        {
            reorderTimer.Start();
        }

        /// <summary>
        /// Metoda, která se vyvolá při změne hodnoty počtu kopiií, které se mají udělat při stisku tlačítka na kopírování položek seznamu.
        /// Slouží k tomu, aby se vrátil focus na ty vybrané položky, které byli vybrané před zvednutím/zmenšením hodnoty komponenty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            var indeces = pictureListViewEx.SelectedIndices;
            pictureListViewEx.Focus();
            for (int i = 0; i < indeces.Count; i++)
            {
                pictureListViewEx.Items[indeces[i]].Selected = true;
            }
        }

        /// <summary>
        /// Metoda vyvolaná při zavření formuláře.
        /// Uloží nastavení jazyka, jednotek a rozlišení.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                settings.Save(settingsFilename);
            }
            catch { } //pri vyjimce se soubor proste neulozi
        }
        /// <summary>
        /// uloží projekt data do souboru pojmenovaný uživatelem s koncovkou .int
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ulozToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String filename;
            saveConfigDialog.Filter = "int|*.int";  // na vyber koncovaka int
            saveConfigDialog.AddExtension = true;
            if (saveConfigDialog.ShowDialog() == DialogResult.OK)
                filename = saveConfigDialog.FileName;
            else return;
            String[] split = filename.Split('.');    
            int lastIndex = split.Length - 1;
            if (split[lastIndex].Equals("int"))         // pokud už končí nepřidávam koncovku
                saveConfigDialog.AddExtension = false;
            else
                filename += ".int";
            try
            {
                projectData.Save(filename, getListFromPictureView());   // uložení konfirace s jménem souboru filename a listem obázků z formuláře
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        /// <summary>
        /// Metoda vyvolaná při stisku tlačítka pro seřazení seznamu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sortButton_Click(object sender, EventArgs e)
        {
            sortListView();
        }

        /// <summary>
        /// Metoda vyvolaná při stisku tlačítka pro nahrazení jednoho obrázku jiným.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceButton_Click(object sender, EventArgs e)
        {
            replacePicture();
        }
        /// <summary>
        /// Načte konfigurační soubor s koncovkou int a pokusí se ho nahrát do Mainformu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nactiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String filename;
            openConfigDialog.Filter = "int|*.int";
            openConfigDialog.AddExtension = true;
            if (openConfigDialog.ShowDialog() == DialogResult.OK)
                filename = openConfigDialog.FileName;
            else return;
            try
            {
                List<String> pathPics = projectData.Load(filename);     //načtu si cesty obrázků a v metode Load nastavím do LineData a Interlacing dat požadované data
                projectData.GetInterlacingData().SetUnits(((StringValuePair<Units>)settings.GetSelectedUnits()).value);     // nastavím jednotky, které jsou momentálně v mainformu nastaveny
                projectData.GetLineData().SetUnits(((StringValuePair<Units>)settings.GetSelectedUnits()).value);
                projectData.GetInterlacingData().SetResolutionUnits(((StringValuePair<Units>)settings.GetSelectedResolutionUnits()).value);
                updateAllComponents();      // updatuju celý mainform aby se provedli změny v gui
                setPictureViewFromList(pathPics);       // nastavím i cesty k novým obrázkům

                drawLineThickness();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        /// <summary>
        /// Metoda, vyvolaná stiskem tlačítka pro posun vybraného obrázku dolů.
        /// Pokud není vybraná poslední položka seznamu, posune vybraný obrázek o pozici níž.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moveDownButton_Click(object sender, EventArgs e)
        {
            movePicturesDown();
        }

        /// <summary>
        /// Metoda, vyvolaná stiskem tlačítka pro posun vybraného obrázku nahoru.
        /// Pokud není vybraná první položka seznamu, posune vybraný obrázek o pozici výš.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moveUpButton_Click(object sender, EventArgs e)
        {
            movePicturesUp();
        }

        /// <summary>
        /// Metoda, vyvolaná stiskem tlačítka pro odstranění vybraných položek ze seznamu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removePicButton_Click(object sender, EventArgs e)
        {
            removePictures();
        }

        /// <summary>
        /// Metoda, vyvolaná stiskem tlačítka pro odstranění všech položek seznamu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearAllButton_Click(object sender, EventArgs e)
        {
            pictureListViewEx.Items.Clear();
            order = 1;
            projectData.GetInterlacingData().SetWidth(0);
            projectData.GetInterlacingData().SetHeight(0);
            changeMaxLineThickness();
            updateAllComponents();
            previewData.ShowDefaultImage();
            drawLineThickness();
        }
        /// <summary>
        /// Metoda vyvolaná po dokončení akce drag and drop.    
        /// Pro každou cestu přetahovaného souboru vytvoříme položku v listu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                int lastIndnex = pictureListViewEx.Items.Count;
                foreach (string path in filePaths)
                {
                    ListViewItem item = new ListViewItem(new[] { Convert.ToString(order), path, getPicName(path), "" });
                    pictureListViewEx.Items.Add(item);
                    order++;
                }
                reorder();
                trySetValuesFromPictures(filePaths);
            }
        }

        /// <summary>
        /// Metoda vyvolaná při stisku tlačítka pro přidání obrázku.
        /// Otevře file dialog pro přidání nových obrázků.
        /// Je povolen multiselect takže se může vybrat několik obrázků najednou.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addPicButton_Click(object sender, EventArgs e)
        {
            addPicFileDialog.Multiselect = true;
            addPicFileDialog.Filter = stringOfInputExtensions;
            addPicFileDialog.FilterIndex = 1;
            DialogResult result = addPicFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] chosenPictures = addPicFileDialog.FileNames;
                for (int i = 0; i < chosenPictures.Length; i++)
                {
                    var indeces = pictureListViewEx.SelectedIndices;
                    int selectedIndex;
                    // Pokud je vybrano vic, tak posuneme ten nad tim pod vybrane
                    // Pokud neni v listu nic vybrano
                    if (indeces.Count == 0)
                    {
                        selectedIndex = Convert.ToInt32(order - 1);
                    }
                    // Pokud je v listu neco vybrano
                    else
                    {
                        selectedIndex = Convert.ToInt32(indeces[0]) + 1;
                    }
                    int numOfPics = chosenPictures.Count();
                    ListViewItem item = new ListViewItem(new[] { Convert.ToString(order), chosenPictures[i], getPicName(chosenPictures[i]), "" });
                    pictureListViewEx.Items.Insert(selectedIndex, item);
                    reorder();
                    pictureListViewEx.Focus();
                    pictureListViewEx.Items[selectedIndex].Selected = false;
                    changeMaxLineThickness();
                }
                trySetValuesFromPictures(chosenPictures);
            }

            drawLineThickness();
        }

        /// <summary>
        /// otevre formular pro nastaveni aplikace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsForm = new SettingsForm(this, settings);
            settingsForm.ShowDialog();
        }

        /// <summary>
        /// vyzada si zadani cesty a jmena vystupniho souboru a provede prolozeni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void interlaceButton_Click(object sender, EventArgs e)
        {
            String filename;
            savePicFileDialog.Filter = stringOfOutputExenstions;
            savePicFileDialog.AddExtension = true;
            if (savePicFileDialog.ShowDialog() == DialogResult.OK)
            {
                filename = savePicFileDialog.FileName;
            }
            else return;
            if (isExtensionValid(filename))
                savePicFileDialog.AddExtension = false;

            List<Picture> picList = harvestPicList();  //ziskani seznamu obrazku z listView
            if (picList.Count == 0)  //chyba pri prazdnem seznamu
            {
                MessageBox.Show(Localization.resourcesStrings.GetString("emptyListError"));
                return;
            }
            PictureContainer picCon = new PictureContainer(picList, projectData.GetInterlacingData(), projectData.GetLineData(), interlaceProgressBar);  //vytvoreni PictureContaineru
            try
            {
                if (!picCon.CheckPictures())  //kontrola konzistence velikosti jednotlivych obrazku
                {
                    /*pokud nejsou stejne velke, aplikace se zepta, zda ma obrazky oriznout a pokracovat*/
                    DialogResult dialogResult = MessageBox.Show(Localization.resourcesStrings.GetString("imageDimensionError"), "", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)  //pri odmitnuti je proces ukoncen
                        return;
                }
                picCon.Interlace();  //prolozeni
            }
            catch (PictureLoadFailureException ex)  //chyba pri nacitani souboru s obrazkem (pravdepodobne chybejici soubor)
            {
                MessageBox.Show(string.Format(Localization.resourcesStrings.GetString("fileNotFoundError"), ex.filename));
                interlaceProgressBar.Value = 0;
                return;
            }
            catch (PictureWrongFormatException ex)  //chyba pri chybnem formatu nacitaneho obrazku
            {
                MessageBox.Show(string.Format(Localization.resourcesStrings.GetString("wrongFormatError"), ex.filename));
                interlaceProgressBar.Value = 0;
                return;
            }
            catch (OutOfMemoryException)  //chyba nedostatku pameti
            {
                MessageBox.Show(Localization.resourcesStrings.GetString("memoryError"));
                interlaceProgressBar.Value = 0;
                return;
            }
            catch (PictureProcessException)  //chyba pri chybne nastavenych parametrech prokladani
            {
                MessageBox.Show(Localization.resourcesStrings.GetString("interlacingError"));
                interlaceProgressBar.Value = 0;
                return;
            }
            Picture result = picCon.GetResult();  //ziskani vysledneho obrazku
            try
            {
                result.Save(filename);  //ulozeni obrazku
            }
            catch (PictureSaveFailureException ex)  //chyba pri ukladani obrazku
            {
                MessageBox.Show(string.Format(Localization.resourcesStrings.GetString("imageSaveError"), ex.filename));
            }
            result.Destroy();  //dealokace obrazku
            MessageBox.Show(Localization.resourcesStrings.GetString("doneMessage"));
        }

        /// <summary>
        /// KeyListener pro pro list s obrázky.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureListViewEx_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete) {
                removePictures();
            } else if (e.Control && e.KeyCode == Keys.A) {
                selectAllPictures();
            } else if (e.Control && e.KeyCode == Keys.Down) {
                movePicturesDown();
            } else if (e.Control && e.KeyCode == Keys.Up) {
                movePicturesUp();
            } else if (e.Control && e.KeyCode == Keys.D) {
                copyPictures(1);
            }
        }
    }
}
