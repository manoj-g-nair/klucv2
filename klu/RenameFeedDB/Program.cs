using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RenameFeedDB
{
    class Program
    {  
        /// <summary>
        /// This is a small helper program to rename the pictured of the FEED Database.
        /// Since we need unique names for batch classification, it's easier to move all
        /// pictures into one directory.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string feed = "C:/Users/Konrad/Pictures/Face Databases/FEED Database/feedtum";
            
            DirectoryInfo root = new DirectoryInfo(feed);

            if (root.Exists)
            {
                foreach (DirectoryInfo emotionDir in root.GetDirectories())
                {
                    foreach (DirectoryInfo emotionSubDir in emotionDir.GetDirectories())
                    {
                        foreach (FileInfo picture in emotionSubDir.GetFiles())
                        {
                            string newPath = emotionDir.FullName + "/" + emotionDir.Name + "_" + emotionSubDir.Name + "_" + picture.Name;

                            //Console.WriteLine(picture.FullName + " -> " + newPath);
                            picture.MoveTo(newPath);                           
                        }

                        // Delete the subdirectory
                        emotionSubDir.Delete();
                    }
                }
            }

            Console.WriteLine("Done!");
        }
    }
}
