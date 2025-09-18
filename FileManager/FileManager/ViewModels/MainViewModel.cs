using FileManager.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FileManager.Commands;


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

        public ICommand LoadDrivesCommand { get; }
        public ICommand LoadChildrenCommand { get; }
        public ICommand OpenFileCommand { get; }

        public MainViewModel()
        {
            RootItems = new ObservableCollection<Drive>();
            LoadDrivesCommand = new RelayCommand(LoadDrives);
            LoadChildrenCommand = new RelayCommand(LoadChildren, CanLoadChildren);
            OpenFileCommand = new RelayCommand(OpenFile, CanOpenFile);

            // Загружаем диски при создании ViewModel
            LoadDrives(null);
        }

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
                    // Handle access denied
                    Console.WriteLine($"Access denied: {item.FullPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading children: {ex.Message}");
                }
            }
        }

        private bool CanLoadChildren(object parameter)
        {
            // Можно загружать дочерние элементы только если выбран объект
            return SelectedObject != null;
        }

        private void OpenFile(object parameter)
        {
            // Заглушка для реализации открытия файла
            Console.WriteLine("OpenFile command executed");
        }

        private bool CanOpenFile(object parameter)
        {
            // Можно открывать только файлы
            return SelectedObject is System.IO.File;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}