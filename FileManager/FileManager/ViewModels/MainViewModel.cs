using FileManager.Commands;
using FileManager.Models;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace FileManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private Hashtable openFiles_openFileTime = new Hashtable();

        private static System.Timers.Timer aTimer;

        string filePath = "test.txt";
        public MainViewModel()
        {
            RootItems = new ObservableCollection<Drive>();
            LoadDrivesCommand = new RelayCommand(LoadDrives);
            LoadChildrenCommand = new RelayCommand(LoadChildren);
            OpenFileCommand = new RelayCommand(OpenFile);
            SelectItemCommand = new RelayCommand(SelectItem);
            MouseDoubleClickCommand = new RelayCommand(MouseDoubleClick);
            LoadDrives(null);

            SetTimer();
        }
        private void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
            // Теперь этот метод может обращаться к нестатическому полю
            var keysToRemove = new List<object>();

            foreach (DictionaryEntry entry in openFiles_openFileTime)
            {
                if (entry.Value is DateTime value)
                {
                    // Correct way to check if more than 10 seconds have passed
                    if (DateTime.Now - value > TimeSpan.FromSeconds(10))
                    {
                        keysToRemove.Add(entry.Key);
                    }
                }
            }

            // Remove all collected keys
            foreach (var key in keysToRemove)
            {
                openFiles_openFileTime.Remove(key);
            }
            
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (DictionaryEntry entry in openFiles_openFileTime)
                {
                    writer.WriteLine($"{entry.Key}={entry.Value}");
                }
            }
        }

        private ObservableCollection<Drive> _rootItems;
        private FileSystemObjectModel _selectedObject;

        public ObservableCollection<Drive> RootItems
        {
            get => _rootItems;
            set { _rootItems = value; 
                OnPropertyChanged(); }
        }


        private String _selectedFilePreviewText;
        public String SelectedFilePreviewText
        {
            get { return _selectedFilePreviewText; }
            set
            {
                _selectedFilePreviewText = value;
                OnPropertyChanged();
            }
        }

        public FileSystemObjectModel SelectedObject
        {
            get => _selectedObject;
            set { _selectedObject = value; 
                OnPropertyChanged(); }
        }

        private String _driveInfo;
        public String SelectedDriveInfo
        {
            get => _driveInfo;
            set { _driveInfo = value; OnPropertyChanged(); }
        }

        private String _directiryInfo;
        public String SelectedDirectoryInfo
        {
            get => _directiryInfo;
            set { _directiryInfo = value; OnPropertyChanged(); }
        }

        public ICommand LoadDrivesCommand { get; }
        public ICommand LoadChildrenCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand SelectItemCommand { get; }
        public ICommand MouseDoubleClickCommand { get; }
        private void InfoUpdate(object parameter)
        {
            String RootDirectoryLetter = SelectedObject.FullPath[0].ToString();
            Models.Drive RootDirectory = new Models.Drive(RootDirectoryLetter);
            SelectedDriveInfo =
                    "Информация о диске " + RootDirectory.FullPath + "\n" +
                "Объем диска: " + (RootDirectory.SpaceOverall / 1024) / 1024 / 1024 + " Гб \n" +
                "Свободное пространство: " + (RootDirectory.SpaceLeft / 1024) / 1024 / 1024 + " Гб \n"
                ;

            if (SelectedObject is Models.Directory directory)
            {
                SelectedDirectoryInfo =
                    "Информация о директории\n" +
                    "Директория: " + directory.FullPath + "\n" +
                    "Время создания: " + Convert.ToDateTime(directory.CreateDate) + "\n" +
                    "Корневой каталог: " + RootDirectory.FullPath + "\n"
                    ;
            }
            else if (SelectedObject is Models.File file)
            {
                SelectedDirectoryInfo =
                    "Информация о файле\n" +
                    "Имя файла: " + file.FullPath + "\n"
                    ;
            }
            else if (SelectedObject is Models.Drive drive)
            {
                SelectedDirectoryInfo =
                    "Информация о диске\n" +
                    "Объем диска: " + (drive.SpaceOverall / 1024) / 1024 / 1024 + " Гб \n" +
                    "Свободное пространство: " + (drive.SpaceLeft / 1024) / 1024 / 1024 + " Гб \n"
                    ;
            }
        }
        private void PreviewText(object parameter)
        {
            if (SelectedObject is Models.File file)
            {
                try
                {
                    if (Path.GetExtension(file.FullPath).ToLower() == ".txt")
                    {
                        SelectedFilePreviewText = System.IO.File.ReadAllText(file.FullPath);
                    }
                    else
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(file.FullPath);
                        SelectedFilePreviewText = Encoding.UTF8.GetString(bytes.Take(500).ToArray());
                    }
                }
                catch (Exception ex) {
                    SelectedFilePreviewText = "Something isn't right";
                }
            }
            else {
                SelectedFilePreviewText = "Select File";
            }
        }
        private void MouseDoubleClick(object parameter)
        {
            if (SelectedObject is Models.File file) {
                try {
                    ProcessStartInfo startInfo = new ProcessStartInfo(file.FullPath);
                    startInfo.UseShellExecute = true;
                    System.Diagnostics.Process.Start(startInfo);
                    openFiles_openFileTime.Add(file.Name, DateTime.Now);
                }       
                catch { 
                
                }
            }
        }
        private void SelectItem(object parameter)
        {
            InfoUpdate(null);
            PreviewText(null);
        }
        private String GetRootDirectory(String path) => path[0].ToString();
        
        private void LoadDrives(object parameter)
        {
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady)
                    {
                        RootItems.Add(
                        new Drive
                        {
                            Name = drive.Name,
                            FullPath = drive.Name,
                            SpaceOverall = drive.TotalSize,
                            SpaceLeft = drive.AvailableFreeSpace
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки дисков: {ex.Message}");
            }
        }

        private void LoadChildren(object parameter)
        {
            if (parameter is FileSystemObjectModel item &&
                System.IO.Directory.Exists(item.FullPath))
            {
                try
                {
                    // Load directories
                    string[] dirs = System.IO.Directory.GetDirectories(item.FullPath);
                    foreach (string dirPath in dirs)
                    {
                        var dirInfo = new System.IO.DirectoryInfo(dirPath);
                        item.Children.Add(new Models.Directory
                        {
                            Name = dirInfo.Name,
                            FullPath = dirPath,
                            CreateDate = dirInfo.CreationTime,
                        });
                    }

                    // Load files
                    string[] files = System.IO.Directory.GetFiles(item.FullPath);
                    foreach (string filePath in files)
                    {
                        var fileInfo = new System.IO.FileInfo(filePath);
                        item.Children.Add(new Models.File
                        {
                            Name = fileInfo.Name,
                            FullPath = filePath,
                            CreateDate = fileInfo.CreationTime,
                            Size = fileInfo.Length
                        });
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Access denied: {item.FullPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading children: {ex.Message}");
                }
            }
        }

        private void OpenFile(object parameter) =>
            // Заглушка для реализации открытия файла
            Console.WriteLine("OpenFile command executed");

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}