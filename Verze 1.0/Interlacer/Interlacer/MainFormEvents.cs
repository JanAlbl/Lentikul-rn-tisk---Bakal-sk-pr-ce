using GfxlibWrapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            revertList();
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
            updateAllComponents();
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
            updateAllComponents();
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
            updateAllComponents();
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
            updateAllComponents();
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
            updateAllComponents();
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
            updateAllComponents();
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
            changeMaxLineThickness();
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
            drawLineThickness();
        }
        /// <summary>
        /// Nastaví směr prokládání na horizontální
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void horizontalRadiobutton_CheckedChanged(object sender, EventArgs e)
        {
            projectData.GetInterlacingData().SetDirection(Direction.Horizontal);
            drawLineThickness();
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

            loadConfigurationFile(filename);
            /*try
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
            }*/
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
            clearList();
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
            addPicToList();
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
            interlace();
        }

        /// <summary>
        /// KeyListener pro pro list s obrázky.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureListViewEx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                removePictures();
            }
            else if (e.KeyCode == Keys.Add || e.KeyCode == Keys.D1)
            {
                addPicToList();
            }
            else if (e.Control && e.KeyCode == Keys.Down)
            {
                movePicturesDown();
            }
            else if (e.Control && e.KeyCode == Keys.Up)
            {
                movePicturesUp();
            }
            else if (e.Control && e.KeyCode == Keys.A)
            {
                selectAllPictures();
            }
            else if (e.Control && e.KeyCode == Keys.D)
            {
                copyPictures(1);
            }
            else if (e.Control && e.KeyCode == Keys.Q)
            {
                replacePicture();
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                sortListView();
            }
            else if (e.Control && e.KeyCode == Keys.R)
            {
                revertList();
            }
            else if (e.Control && e.KeyCode == Keys.E)
            {
                clearList();
            }
        }

        private void interlaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            interlace();
        }

        private void wholeDriveTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNodeInherited node = (TreeNodeInherited)e.Node;
            if (node.isPopulated)
                return;

            DirectoryInfo root = new DirectoryInfo(node.FullPath);

            populateDirectory(root, node);
        }

        private void wholeDriveTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        // Set the target drop effect to the effect  
        // specified in the ItemDrag event handler. 
        /* private void wholeDriveTree_DragEnter(object sender, DragEventArgs e)
         {
             e.Effect = e.AllowedEffect;
         }

         // Select the node under the mouse pointer to indicate the  
         // expected drop location. 
         private void wholeDriveTree_DragOver(object sender, DragEventArgs e)
         {
             // Retrieve the client coordinates of the mouse position.
             Point targetPoint = wholeDriveTree.PointToClient(new Point(e.X, e.Y));

             // Select the node at the mouse position.
             wholeDriveTree.SelectedNode = wholeDriveTree.GetNodeAt(targetPoint);
         }

         private void wholeDriveTree_DragDrop(object sender, DragEventArgs e)
         {
             // Retrieve the client coordinates of the drop location.
             Point targetPoint = wholeDriveTree.PointToClient(new Point(e.X, e.Y));

             // Retrieve the node at the drop location.
             TreeNodeInherited targetNode = (TreeNodeInherited)wholeDriveTree.GetNodeAt(targetPoint);

             if (targetNode.isDirectory == false)
                 return;

             // Retrieve the node that was dragged.
             TreeNodeInherited draggedNode = (TreeNodeInherited)e.Data.GetData(typeof(TreeNodeInherited));
             if (draggedNode == null)
             {
                 e.Effect = DragDropEffects.None;
                 return;
             }

             // Confirm that the node at the drop location is not  
             // the dragged node or a descendant of the dragged node. 
             if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
             {
                 // If it is a move operation, remove the node from its current  
                 // location and add it to the node at the drop location. 
                 if (e.Effect == DragDropEffects.Move)
                 {
                     draggedNode.Remove();
                     targetNode.Nodes.Add(draggedNode);
                 }

                 // If it is a copy operation, clone the dragged node  
                 // and add it to the node at the drop location. 
                 else if (e.Effect == DragDropEffects.Copy)
                 {
                     targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
                 }

                 // repaint
                 //wholeDriveTree.Invalidate();

                 // Expand the node at the location  
                 // to show the dropped node.
                 targetNode.Expand();
             }
         }

         // Determine whether one node is a parent  
         // or ancestor of a second node. 
         private bool ContainsNode(TreeNode node1, TreeNode node2)
         {
             // Check the parent node of the second node. 
             if (node2.Parent == null) return false;
             if (node2.Parent.Equals(node1)) return true;

             // If the parent node is not null or equal to the first node,  
             // call the ContainsNode method recursively using the parent of  
             // the second node. 
             return ContainsNode(node1, node2.Parent);
         } */


        private void interlaceProgressBarFlowLayout_Paint(object sender, PaintEventArgs e)
        {
            interlaceProgressBar.Size = new Size(interlaceProgressBarFlowLayout.Size.Width - 5, 29);
        }

        private void fillButton_Click(object sender, EventArgs e)
        {
            fillList();
            updateAllComponents();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            clipForm = new ClipForm();
            clipForm.Show();
        }

        private void expandCollapse_Click(object sender, EventArgs e)
        {
            int itemCount = pictureListViewEx.Items.Count;

            if (isExpanded)
                collapseList(itemCount);
            else
                expandList(itemCount);
        }
    }
}
