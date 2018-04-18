using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Transportlaget;
using Library;
using Linklaget;
using Library;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";
        Transport t1;


		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
            t1 = new Transport(BUFSIZE, APP);
		    var filePath = "";
		    long fileSize = 0;
		    var srvBuffer = new byte[BUFSIZE];
		    var bufferSize = 0;

		    while (true)
		    {
		        bufferSize = t1.receive(ref srvBuffer); // wait for client input

		        var filename = ((new UTF8Encoding()).GetString(srvBuffer)).Substring(0, bufferSize);
                filename = filename.Replace("\0", string.Empty);

		        Console.WriteLine($"Server looking for file {filename}");


		        fileSize = LIB.check_File_Exists(filename);
                sendFile(filename, fileSize, t1);
		    }

		}

        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name='fileName'>
        /// File name.
        /// </param>
        /// <param name='fileSize'>
        /// File size.
        /// </param>
        /// <param name='tl'>
        /// Tl.
        /// </param>
        private void sendFile(string fileName, long fileSize, Transport transport)
		{
            transport.send(Encoding.UTF8.GetBytes(fileSize.ToString()), fileSize.ToString().Length);

		    var chunk = new byte[BUFSIZE];

		    var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

		    var index = 0;
		    var bytesRead = 0;

            while (index < fileSize)
		    {		        
		        bytesRead = stream.Read(chunk, 0, BUFSIZE); // læser 1000 bytes
		        transport.send(chunk, bytesRead);           // Kommer aldrig videre....
                index += bytesRead;
		        
		        Console.WriteLine("\r" + String.Format("{0,13:P6} {1,15}", ((index / 1.0) / (long)(fileSize)), index));

            }

		    Console.WriteLine("Succes on sending file!");
            stream.Close();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
             new file_server();
		}
	}
}