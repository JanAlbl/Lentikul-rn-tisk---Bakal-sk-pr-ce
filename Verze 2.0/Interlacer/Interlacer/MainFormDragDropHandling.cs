using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interlacer
{
    public partial class MainForm : Form
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // PPPPP  I  CCCCC  TTTTTTT  U    U  RRRRRRR  EEEEEE  RRRRRRR  EEEEEE      L        I   SSSSSS  TTTTTTT      V       V  I  EEEEEE  V   V   V    V //
        // P   P  I  C         T     U    U  R     R  E       R     R  E           L        I  S           T          V     V   I  E        V    VV    V  // 
        // PPPPP  I  C         T     U    U  RRRRRRR  EEEEEE  RRRRRRR  EEEEEE      L        I   SSSSS      T           V   V    I  EEEEEE    V   VV   V   //
        // P      I  C         T     U    U  RRRR     E       RRRR     E           L        I        S     T            V V     I  E          V V  V V    //
        // P      I  CCCCC     T     UUUUUU  R   RRR  EEEEEE  R   RRR  EEEEEE      LLLLLLL  I  SSSSSS      T             V      I  EEEEEE      V    V     //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // ------------------------------------EVENTY------------------------------------ //

        /// <summary>
        /// Metoda vyvolaná při začátku táhnutí itemu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureListViewEx_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }

        }

        /// <summary>
        /// Metoda vyvolaná vstupem kurzoru na list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureListViewEx_DragEnter(object sender, DragEventArgs e)
        {
            TreeNodeInherited draggedNode = (TreeNodeInherited)e.Data.GetData(typeof(TreeNodeInherited));
            ListViewItem draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                foreach (string path in filePaths)
                {
                    FileAttributes attr = File.GetAttributes(path);
                    if (isExtensionValid(path) || ((attr & FileAttributes.Directory) == FileAttributes.Directory))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }
                }
            }
            else if (draggedNode != null && (draggedNode.isImage == true || draggedNode.isDirectory == true))
            {
                e.Effect = DragDropEffects.Move;
            }
            else if (draggedItem != null)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// Metoda vyvolaná při puštění tlačítka neboli ukončení drag and dropu.
        /// Na konci se zapne timer pro očíslování pořadí položek listu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureListViewEx_DragDrop(object sender, DragEventArgs e)
        {
            TreeNodeInherited draggedNode = (TreeNodeInherited)e.Data.GetData(typeof(TreeNodeInherited));
            ListViewItem draggedItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                droppedItemsAreFiles(e);
            }
            else if (draggedNode != null)
            {
                FileAttributes attr = File.GetAttributes(draggedNode.FullPath);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    if (draggedNode.isPopulated == false)
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(draggedNode.FullPath);
                        populateDirectory(dirInfo, draggedNode);
                    }

                    //tady predelat aby se dobre posouvali
                    for (int i = 0; i < draggedNode.Nodes.Count; i++)
                    {
                        droppedItemIsTreeNode(e, (TreeNodeInherited)draggedNode.Nodes[i]);
                    }
                }
                else
                {
                    droppedItemIsTreeNode(e, draggedNode);
                }

            }
            else if (draggedItem != null)
            {
                droppedItemIsListItem(e);
            }

            pictureListViewEx.InsertionMark.Index = -1;
            reorderTimer.Start();
        }

        /// <summary>
        /// Metoda vyvolaná přejetím kurzorem myši přes jednotlivé položky seznamu při drag and dropu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureListViewEx_DragOver(object sender, DragEventArgs e)
        {
            Point targetPoint = pictureListViewEx.PointToClient(new Point(e.X, e.Y));
            int index = pictureListViewEx.Items.Count - 1;

            ListViewItem targetItem = pictureListViewEx.GetItemAt(targetPoint.X, targetPoint.Y);

            if (targetItem == null)
            {
                pictureListViewEx.InsertionMark.AppearsAfterItem = true;
            }
            else
            {
                pictureListViewEx.InsertionMark.AppearsAfterItem = false;
            }

            pictureListViewEx.InsertionMark.Index = pictureListViewEx.InsertionMark.NearestIndex(targetPoint);
        }


        // ------------------------------------METODY------------------------------------ //

        /// <summary>
        /// Metoda, která řeší drag and drop na listu pokud tažená data jsou položky listu.
        /// </summary>
        /// <param name="e"></param>
        private void droppedItemIsListItem(DragEventArgs e)
        {
            Point targetPoint = pictureListViewEx.PointToClient(new Point(e.X, e.Y));
            ListViewItem targetItem = pictureListViewEx.GetItemAt(targetPoint.X, targetPoint.Y);
            int targetIndex = pictureListViewEx.Items.Count;
            int indexShift = 1;

            if (targetItem != null)
                targetIndex = targetItem.Index;

            var indeces = pictureListViewEx.SelectedIndices;
            int[] selectedIndexes = new int[indeces.Count];

            for (int i = 0; i < indeces.Count; i++)
                selectedIndexes[i] = indeces[i];

            ListViewItem[] draggedItems = new ListViewItem[selectedIndexes.Length];
            for (int i = 0; i < draggedItems.Length; i++)
            {
                draggedItems[i] = new ListViewItem();
                for (int j = 0; j < pictureListViewEx.Items[selectedIndexes[i]].SubItems.Count; j++)
                {
                    draggedItems[i].SubItems.Add("");
                    draggedItems[i].SubItems[j].Text = pictureListViewEx.Items[selectedIndexes[i]].SubItems[j].Text;
                }
            }

            int[] newIndexes = new int[draggedItems.Length];
            for (int i = 0; i < newIndexes.Length; i++)
            {
                if (targetIndex > selectedIndexes[i])
                {
                    if (i >= 1)
                        newIndexes[i] = newIndexes[i - 1] + 1 - indexShift;
                    else
                        newIndexes[i] = targetIndex - indexShift;
                }
                else
                {
                    if (i >= 1)
                        newIndexes[i] = newIndexes[i - 1] + 1;
                    else
                        newIndexes[i] = targetIndex;
                }
            }

            int[] indexesToRemove = new int[draggedItems.Length];
            for (int i = 0; i < indexesToRemove.Length; i++)
            {
                if (targetIndex > selectedIndexes[i])
                {
                    if (i == 0)
                        indexesToRemove[i] = selectedIndexes[i];
                    else
                        indexesToRemove[i] = selectedIndexes[i] - i;
                }
                else
                {
                    indexesToRemove[i] = selectedIndexes[i];
                }
            }

            for (int i = 0; i < newIndexes.Length; i++)
            {
                pictureListViewEx.Items[indexesToRemove[i]].Remove();
                pictureListViewEx.Items.Insert(newIndexes[i], draggedItems[i]);
            }
        }

        /// <summary>
        /// Metoda, která řeší drag and drop na listu pokud tažená data jsou soubory například z total cmd, plochy atd.
        /// </summary>
        /// <param name="e"></param>
        private void droppedItemsAreFiles(DragEventArgs e)
        {
            Point targetPoint = pictureListViewEx.PointToClient(new Point(e.X, e.Y));
            ListViewItem targetItem = pictureListViewEx.GetItemAt(targetPoint.X, targetPoint.Y);
            int targetIndex = pictureListViewEx.Items.Count;

            if (targetItem != null)
                targetIndex = targetItem.Index;

            string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
            foreach (string path in filePaths)
            {
                FileAttributes attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    string[] paths = Directory.GetFiles(path);
                    for (int i = 0; i < paths.Length; i++)
                    {
                        if (isExtensionValid(paths[i]))
                        {
                            ListViewItem item = new ListViewItem(new[] { Convert.ToString(order), paths[i], getPicName(paths[i]), "" });
                            pictureListViewEx.Items.Insert(targetIndex++, item);
                        }
                    }
                }
                else
                {
                    ListViewItem item = new ListViewItem(new[] { Convert.ToString(order), path, getPicName(path), "" });
                    pictureListViewEx.Items.Insert(targetIndex++, item);
                }
            }
            trySetValuesFromPictures(filePaths);
        }

        /// <summary>
        /// Metoda, která řeší drag and drop na listu pokud tažená data jsou uzly stromu.
        /// Pokud je cesta v uzlu cesta k obrázku na disku, přidá položku do listu.k
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="draggedNode"></param>
        private void droppedItemIsTreeNode(DragEventArgs e, TreeNodeInherited draggedNode)
        {
            Point targetPoint = pictureListViewEx.PointToClient(new Point(e.X, e.Y));
            ListViewItem targetItem = pictureListViewEx.GetItemAt(targetPoint.X, targetPoint.Y);
            int targetIndex = pictureListViewEx.Items.Count;

            if (targetItem != null)
                targetIndex = targetItem.Index;

            //rozdelit podle koncovky
            if (draggedNode.isImage)
            {
                ListViewItem newItem = new ListViewItem(new[] { Convert.ToString(order), draggedNode.FullPath.Replace("\\\\", "\\"), getPicName(draggedNode.FullPath.Replace("\\\\", "\\")), "" });
                pictureListViewEx.Items.Insert(targetIndex, newItem);
            }
        }





        ///////////////////////////////////////////////////////////////////
        // MM   MM    AA    I  N   N      FFFFF   OOO   RRRRRRR  MM   MM //
        // M M M M   A  A   I  NN  N      F      O   O  R     R  M M M M //
        // M  M  M  AAAAAA  I  N N N      FFFFF  O   O  RRRRRRR  M  M  M //
        // M     M  A    A  I  N  NN      F      O   O  RRRR     M     M //
        // M     M  A    A  I  N   N      F       OOO   R   RRR  M     M //
        ///////////////////////////////////////////////////////////////////

        /// <summary>
        /// Metoda vyvolaná při tažení souboru přes hlavní formulář.
        /// Ověří se, zda přetahovaný soubor/soubory má/mají validní koncovku.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            TreeNodeInherited draggedNode = (TreeNodeInherited)e.Data.GetData(typeof(TreeNodeInherited));

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                foreach (string path in filePaths)
                {
                    if (Path.GetExtension(path).Equals(configurationExtension))
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }
                }
            }
            else if (draggedNode != null && Path.GetExtension(draggedNode.Name).Equals(configurationExtension))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// Metoda vyvolaná po dokončení akce drag and drop.    
        /// Pro každou cestu přetahovaného souboru vytvoříme položku v listu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            TreeNodeInherited draggedNode = (TreeNodeInherited)e.Data.GetData(typeof(TreeNodeInherited));
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                foreach (string path in filePaths)
                {
                    if (Path.GetExtension(path).Equals(configurationExtension))
                    {
                        loadConfigurationFile(path);
                    }
                }
            }
            else if (draggedNode != null)
            {
                if (Path.GetExtension(draggedNode.Name).Equals(configurationExtension))
                {
                    loadConfigurationFile(draggedNode.FullPath);
                }
            }
        }


        /////////////////////////////////////
    }
}
