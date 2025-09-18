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

        private void LoadChildren(FileSystemObjectModel parameter)
        {
            if (System.IO.Directory.Exists(parameter.FullPath))
            {
                string[] dirs = System.IO.Directory.GetDirectories(parameter.FullPath);
                foreach (string s in dirs)
                {
                    parameter.Children.Add(
                    new Models.Directory
                    {
                        Name = s,
                        FullPath = s
                    });
                }
                string[] files = System.IO.Directory.GetFiles(parameter.FullPath);
                foreach (string s in files)
                {
                    parameter.Children.Add(
                    new Models.File
                    {
                        Name = s,
                        FullPath = s
                    });
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