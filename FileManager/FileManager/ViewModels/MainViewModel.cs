using FileManager.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FileManager.Commands;
using System.Windows;
using System.Diagnostics;

namespace FileManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Drive> _rootItems;
        private FileSystemObjectModel _selectedObject;

        public ObservableCollection<Drive> RootItems
        {
            get => _rootItems;
            set { _rootItems = value; 
                OnPropertyChanged(); }
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

        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }


        public ICommand LoadDrivesCommand { get; }
        public ICommand LoadChildrenCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand SelectItemCommand { get; }
        public ICommand MouseDoubleClickCommand { get; }
        public MainViewModel()
        {
            RootItems = new ObservableCollection<Drive>();
            LoadDrivesCommand = new RelayCommand(LoadDrives);
            LoadChildrenCommand = new RelayCommand(LoadChildren);
            OpenFileCommand = new RelayCommand(OpenFile);
            SelectItemCommand = new RelayCommand(SelectItem);
            MouseDoubleClickCommand = new RelayCommand(MouseDoubleClick);
            LoadDrives(null);
        }
        private void InfoUpdate(object parameter)
        {
            String RootDirectoryLetter = SelectedObject.FullPath[0].ToString();
            Models.Drive RootDirectory = new Models.Drive(RootDirectoryLetter);
            SelectedDriveInfo =
                "Объем диска: " + RootDirectory.SpaceOverall / 8 + "Мб \n" +
                "Свободное пространство: " + RootDirectory.SpaceLeft / 8 + "Мб \n" +
                "Корневой каталог: " + RootDirectory.FullPath + "\n"
                ;

            if (SelectedObject is Models.Directory directory)
            {
                SelectedDirectoryInfo =
                    "Директория: " + directory.FullPath + "\n" +
                    "Время создания: " + Convert.ToDateTime(directory.CreateDate) + "\n" +
                    "Корневой каталог: " + RootDirectory.FullPath + "\n"
                    ;
            }
            else if (SelectedObject is Models.File file)
            {
                SelectedDirectoryInfo =
                    "Имя файла: " + file.FullPath + "\n"
                    ;
            }
            else if (SelectedObject is Models.Drive drive)
            {
                SelectedDirectoryInfo =
                    "Объем диска: " + drive.SpaceOverall / 8 + "Мб \n" +
                    "Свободное пространство: " + drive.SpaceLeft / 8 + "Мб \n"
                    ;
            }
        }
        private void MouseDoubleClick(object parameter)
        {
            if (SelectedObject is Models.File file) {
                try {
                    ProcessStartInfo startInfo = new ProcessStartInfo(file.FullPath);
                    startInfo.UseShellExecute = true;
                    System.Diagnostics.Process.Start(startInfo);
                }       
                catch { 
                
                }
            }
        }
        private void SelectItem(object parameter)
        {
            InfoUpdate(null);
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