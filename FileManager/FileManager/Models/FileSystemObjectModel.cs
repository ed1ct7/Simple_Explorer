using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public override string ToString()
        {
            return Name;
        }
        public ObservableCollection<FileSystemObjectModel> Children { get; set; } 
		public abstract E_ObjectType ObjectType { get; }

		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _fullpath;

		public string FullPath
		{
			get { return _fullpath; }
			set { _fullpath = value; }
		}

		private DateTime _createDate;

		public DateTime CreateDate
        {
			get { return _createDate; }
			set { _createDate = value; }
		}

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }
    }
	public class File : FileSystemObjectModel
	{
		public override E_ObjectType ObjectType => E_ObjectType.File;

        private long _size; // Size in bytes

		public long Size
		{
			get { return _size; }
			set { _size = value; }
		}
	}
    public class Directory : FileSystemObjectModel
    {
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }
        public override E_ObjectType ObjectType => E_ObjectType.Directory;

		private int _itemAmount;

		public int ItemAmount
		{
			get { return _itemAmount; }
			set { _itemAmount = value; }
		}

		private long _size; // Size in bytes

        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }
    }
    public class Drive : FileSystemObjectModel
    {
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }
        public override E_ObjectType ObjectType => E_ObjectType.Drive;

        private long _spaceLeft; // Size in bytes

        public long SpaceLeft
        {
            get { return _spaceLeft; }
            set { _spaceLeft = value; }
        }

		private long _spaceOverall;

		public long SpaceOverall
		{
			get { return _spaceOverall; }
			set { _spaceOverall = value; }
		}

	}
}
