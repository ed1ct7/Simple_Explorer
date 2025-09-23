using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace FileManager.Models
{
    public enum E_ObjectType
    {
        File,
        Directory,
        Drive
    }
    public abstract class FileSystemObjectModel
    {
        public FileSystemObjectModel()
        {
            Children = new ObservableCollection<FileSystemObjectModel>();
        }
		public override string ToString() => Name;
        public ObservableCollection<FileSystemObjectModel> Children { get; set; } 
		public abstract E_ObjectType ObjectType { get; }

		private string _name;
		public string Name
		{
			get => _name;
			set { _name = value; }
		}

		private string _fullpath;
		public string FullPath
		{
			get => _fullpath;
			set { _fullpath = value; }
		}

		private DateTime _createDate;
		public DateTime CreateDate
        {
			get => _createDate;
			set { _createDate = value; }
		}
    }
	public class File : FileSystemObjectModel
	{
		public override E_ObjectType ObjectType => E_ObjectType.File;

        private long _size; // Size in bytes

		public long Size
		{
			get => _size;
			set { _size = value; }
		}
	}
    public class Directory : FileSystemObjectModel
    {
        public override E_ObjectType ObjectType => E_ObjectType.Directory;

		private int _itemAmount;

		public int ItemAmount
		{
			get => _itemAmount;
			set { _itemAmount = value; }
		}

		private long _size; // Size in bytes

        public long Size
        {
            get => _size;
            set { _size = value; }
        }
    }
    public class Drive : FileSystemObjectModel
    {
		public Drive(String Letter)
		{
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var d in drives)
			{
				if (d.Name[0].ToString() == Letter)
				{
					this.SpaceLeft = d.AvailableFreeSpace;
					this.SpaceOverall = d.TotalSize;
					this.FullPath = d.Name;
					break;
				}
			}
        }
		public Drive() { }
        public override E_ObjectType ObjectType => E_ObjectType.Drive;

        private long _spaceLeft; // Size in bytes

        public long SpaceLeft
        {
            get => _spaceLeft;
            set { _spaceLeft = value; }
        }

		private long _spaceOverall;

		public long SpaceOverall
		{
			get => _spaceOverall;
			set { _spaceOverall = value; }
		}

	}
}
