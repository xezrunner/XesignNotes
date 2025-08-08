using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace XesignNotes.App.Engine
{
    public class NoteManager
    {
        public string CreateNoteFileExtension(NoteColor color)
        {
            return ".xesignnote_" + color.ToString();
        }

        /// <summary>
        /// AppDirectory\Configuration\...
        /// </summary>
        public string GetPathForNoteFile(string file)
        {
            // Configuration directory path
            string configDir = AppDomain.CurrentDomain.BaseDirectory + @"Notes\";

            // If the configuration directory doesn't exist, create it.
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            return configDir + file;
        }

        public List<string> GetNotes() 
            => Directory.GetFiles(GetPathForNoteFile(""))
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToList();

        public void CreateNoteFile(string name, NoteColor color)
        {
            string file = GetPathForNoteFile(name + CreateNoteFileExtension(color));
            var document = new FlowDocument();

            var range = new TextRange(document.ContentStart, document.ContentEnd);
            var fStream = new FileStream(file, FileMode.Create);
            range.Save(fStream, DataFormats.XamlPackage);
            fStream.Close();
        }

        /// <summary>
        /// Returns the exact file.
        /// </summary>
        string FindFile(string noteName)
        {
            var file = "";
            foreach (string _file in Directory.GetFiles(GetPathForNoteFile("")))
            {
                if (_file.Contains(noteName))
                    file = _file;
            }
            if (file == "")
            {
                CreateNoteFile(noteName, NoteColor.Monochrome);
                return FindFile(noteName);
            }

            return file;
        }
        
        public List<string> SearchFiles(string query)
        {
            List<string> _list = new List<string>();

            foreach (string _file in Directory.GetFiles(GetPathForNoteFile("")).Select(Path.GetFileNameWithoutExtension))
            {
                if (_file.Contains(query))
                    _list.Add(_file);
            }

            return _list;
        }
        
        /// <summary>
        /// Deletes all user notes.
        /// </summary>
        public void DeleteAllNotes()
        {
            foreach (string file in Directory.GetFiles(GetPathForNoteFile("")))
            {
                File.Delete(file);
            }
        }

        public void LoadDocument(string noteName, RichTextBox textbox)
        {
            string file = FindFile(noteName);

            var range = new TextRange(textbox.Document.ContentStart, textbox.Document.ContentEnd);
            var fStream = new FileStream(file, FileMode.OpenOrCreate);
            range.Load(fStream, DataFormats.XamlPackage);
            fStream.Close();
        }

        public void SaveDocument(string noteName, RichTextBox textbox)
        {
            string file = FindFile(noteName);

            var range = new TextRange(textbox.Document.ContentStart, textbox.Document.ContentEnd);
            var fStream = new FileStream(file, FileMode.Create);
            range.Save(fStream, DataFormats.XamlPackage);
            fStream.Close();
        }

        public void DeleteDocument(string noteName)
        {
            File.Delete(FindFile(noteName));
        }
    }
}
