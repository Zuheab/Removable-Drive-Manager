using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Drive_Manager
{
    // This is my simple college project in c#
    public partial class USBMenu : Form
    {
        private const int WM_DEVICECHANGE = 0X219;
        private const int WM_DEVICEARRIVAL = 0X8000;
        private const int WM_DEVICEREMOVECOMPLETE = 0X8004;
        private const int WM_DEVICETYP_VOLUME = 0X00000002;
        private USB Drive;
        private Stack locations;
        protected override void WndProc(ref Message m)
        {// Detects the presence and absence of Removable Drive
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    switch ((int)m.WParam)
                    {
                        case WM_DEVICEARRIVAL:
                            richTextBox1.Text = "New Device Connected";
                            LoadDrives();
                            break;
                        case WM_DEVICEREMOVECOMPLETE:
                            LoadDrives();
                            richTextBox1.Text = "Device Removed";
                            break;
                    }
                    break;

            }
        }


        public USBMenu()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        { // LoadDrives
            locations = new Stack();
            LoadDrives();
            
            Drive = new USB(GetDrive());
        }

        private void LoadDrives()
        { // Count the removable drives and load it to the ListBox
            // If the number of drive is 1, then load its contents to the ListView
            // if number of drive is > 1, then display all the removable drives in the ListBox
            listView1.Items.Clear();
            RemovableDrivesList.Items.Clear();
            int NumberOfRemovableDrives = 0;
            DriveInfo[] Drives = DriveInfo.GetDrives();
            for (int i = 0; i < Drives.Count(); i++)
            {
                if (Drives[i].DriveType == DriveType.Removable)
                {
                    NumberOfRemovableDrives += 1;
                }
            }

            foreach (DriveInfo Drive in Drives)
            {
                if (Drive.DriveType == DriveType.Removable && Drive.IsReady == true)
                {
                    if (NumberOfRemovableDrives == 1)
                    {
                        String location = Drive.Name;
                        locations.Push(location);
                        var USB = new DirectoryInfo(location);
                        foreach (DirectoryInfo Folder in USB.GetDirectories())
                        {
                            ListViewItem item = new ListViewItem();
                            item.Text = Folder.Name;
                            item.SubItems.Add(Folder.FullName);
                            item.ImageIndex = 0;
                            listView1.Items.Add(item);
                        }
                        foreach (FileInfo File in USB.GetFiles())
                        {
                            ListViewItem item = new ListViewItem();
                            item.Text = File.Name;
                            item.SubItems.Add(File.FullName);

                            String extension = File.Extension;

                            if (!imageList1.Images.Keys.Contains(extension))
                            {
                                imageList1.Images.Add(extension, Icon.ExtractAssociatedIcon(File.FullName));
                            }

                            int index = imageList1.Images.Keys.IndexOf(extension);
                            item.ImageIndex = index;
                            listView1.Items.Add(item);
                        }
                    }
                    else if (NumberOfRemovableDrives > 1)
                    {
                        RemovableDrivesList.Items.Add(Drive.Name);
                    }
                }
            }

        }

        private void GetBrowsedData()
        {
            FolderBrowserDialog FDB = new FolderBrowserDialog();

            String location = "";

            if (FDB.ShowDialog() == DialogResult.OK)
            {
                listView1.Items.Clear();
                location = FDB.SelectedPath;
                locations.Push(location);
                GetContents(location);
            }
        }

        private void GetContents(String location)
        {// Enter in the folder
            
            Drive = new USB(location);

            var Dir = new DirectoryInfo(location);
            
            foreach (var folder in Dir.GetDirectories())
            {
                // MessageBox.Show(folder.Name);
                ListViewItem item = new ListViewItem();
                item.Text = folder.Name;
                item.ImageIndex = 0;
                item.SubItems.Add(folder.FullName);
                listView1.Items.Add(item);
            }

            foreach (FileInfo File in Dir.GetFiles())
            {
                String extension = File.Extension;
                if (!imageList1.Images.Keys.Contains(extension))
                {
                    imageList1.Images.Add(extension, Icon.ExtractAssociatedIcon(File.FullName));
                }
                int index = imageList1.Images.Keys.IndexOf(extension);
                ListViewItem item = new ListViewItem();
                item.Text = File.Name;
                item.ImageIndex = index;
                item.SubItems.Add(File.FullName);
                listView1.Items.Add(item);
            }
        }

        private void BrowseBtn_Click(object sender, EventArgs e)
        {
            GetBrowsedData();
        }

        private void Hide_Click(object sender, EventArgs e)
        {   //Hide Files
            richTextBox1.Clear();

            String USBDrive = GetDrive();
            List<String> usbFile = new List<String>();
            String[] Folders = Directory.GetDirectories(USBDrive);
            String command = "/c attrib +s +h +r ";
            foreach (String folder in Folders)
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = command + folder;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

            }

            Process processFiles = new Process();
            processFiles.StartInfo.FileName = "cmd.exe";
            processFiles.StartInfo.Arguments = command + USBDrive + "*.*"; // how to apply dos commands on files that have space in their names
            processFiles.StartInfo.CreateNoWindow = true;
            processFiles.StartInfo.UseShellExecute = false;
            processFiles.StartInfo.RedirectStandardOutput = true;
            processFiles.Start();
            richTextBox1.Text = "Hiding Process Complete";

        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void SelectTable()
        {

            if (listView1.SelectedItems.Count > 0)
            {
                String path = listView1.SelectedItems[0].SubItems[1].Text;
                var Folders = new DirectoryInfo(path);
                Drive = new USB(path);
                try
                {
                    listView1.Items.Clear();
                    foreach (DirectoryInfo folder in Folders.GetDirectories())
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = folder.Name;
                        item.SubItems.Add(folder.FullName);
                        item.ImageIndex = 0;
                        listView1.Items.Add(item);
                    }
                    foreach (FileInfo file in Folders.GetFiles())
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = file.Name;
                        item.SubItems.Add(file.FullName);

                        String extension = file.Extension;

                        if (!imageList1.Images.Keys.Contains(extension))
                        {
                            imageList1.Images.Add(extension, Icon.ExtractAssociatedIcon(file.FullName));
                        }

                        int index = imageList1.Images.Keys.IndexOf(extension);
                        item.ImageIndex = index;
                        listView1.Items.Add(item);
                    }
                }
                catch (Exception ex)
                { MessageBox.Show(ex.Message.ToString(), "Error"); }

            }
        }


        private void unHideBtn_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            String USBDrive = GetDrive();

            String[] Folders = Directory.GetDirectories(USBDrive);
            String command = "/c attrib -r -s -h /s /d ";
            foreach (String folder in Folders)
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = command + folder;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

            }

            Process processFiles = new Process();
            processFiles.StartInfo.FileName = "cmd.exe";
            processFiles.StartInfo.Arguments = command + USBDrive + "*.*";
            processFiles.StartInfo.CreateNoWindow = true;
            processFiles.StartInfo.UseShellExecute = false;
            processFiles.StartInfo.RedirectStandardOutput = true;
            processFiles.Start();
            richTextBox1.Text = "Revealing Process Complete";

        }

        private void FormatBtn_Click(object sender, EventArgs e)
        { // Format the USB Drive
            richTextBox1.Clear();
            String USBDrive = GetDrive();
            Char forbiddenCharacter = '\\';
            USBDrive = USBDrive.TrimEnd(forbiddenCharacter);
            String cmd = "/c format /y /fs:NTFS " + USBDrive + " /V:Nemesis /q /x";
            Process process = new Process
                ();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = cmd;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start
                ();
            richTextBox1.Text = process.StandardOutput.ReadToEnd();
            LoadDrives();
        }

        private void ViewBtn_Click(object sender, EventArgs e)
        {
            //Drive.DriveName
            Process.Start(Drive.DriveName);
        }

        private String GetDrive()
        {// Get all removable drives

            DriveInfo[] Drives = DriveInfo.GetDrives();
            String driveName = "";
            foreach (DriveInfo drive in Drives)
            {
                if (drive.IsReady && drive.DriveType == DriveType.Removable)
                    driveName = drive.Name;

            }
            return driveName;
        }

        private void RemovableDrivesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RemovableDrivesList.SelectedItems.Count > 0)
            {
                String SelectedDrive = RemovableDrivesList.SelectedItems[0].ToString();

                // MessageBox.Show(SelectedDrive);
                locations.Push(SelectedDrive);
                GetContents(SelectedDrive);
            }
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            String path = Drive.DriveName;
            //String path = path.Replace();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            String location = listView1.SelectedItems[0].SubItems[1].Text;
            FileInfo file = new FileInfo(location);
            DirectoryInfo dir = new DirectoryInfo(location);
            if (file.Exists)
            {
                Process.Start(file.FullName);
            }
            else if(dir.Exists)
            {
                locations.Push(dir.FullName);
                listView1.Items.Clear();
                GetContents(location);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!locations.hasJustOne())
            {
                locations.Pop();
                if (!locations.IsEmpty())
                {
                    String lastLocation = locations.Top();
                    listView1.Items.Clear();
                    GetContents(lastLocation);
                }
            }
            else {
                LoadDrives();
            }
        }
    }
}
