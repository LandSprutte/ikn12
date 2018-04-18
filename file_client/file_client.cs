using System;
using System.IO;
using System.Linq;
using System.Text;
using Transportlaget;
using Library;
using Linklaget;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
	    private file_client(String[] args)
	    {
	        var trans = new Transport(BUFSIZE, APP);
	        var byteTofind = Encoding.UTF8.GetBytes(args[0]);
            trans.send(byteTofind, args[0].Length);
            this.receiveFile(args[0], trans);
	    }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (String fileName, Transport transport)
		{
		    Console.WriteLine($"Trying to fetch file: {fileName}");

		    var rcvBuffer = new byte[BUFSIZE];
		    var rcvBytes = transport.receive(ref rcvBuffer);

            try
            {
                rcvBuffer[rcvBytes] = 0;
                long dateReceived = 0;
                
                var fileSize = long.Parse( (new UTF8Encoding()).GetString(rcvBuffer).Substring(0, rcvBytes));

                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    while ((long)dateReceived < fileSize)
		            {
		                var index = transport.receive(ref rcvBuffer);
		                dateReceived += index;
                        fs.Write(rcvBuffer, 0, index);     // write stream
                        fs.Flush();                        // clear stream.
		                Console.WriteLine("\r" + String.Format("{0,13:P2} {1,15}", ((dateReceived / 1.0) / (long)(fileSize)), dateReceived));
                    }
		        }

		        Console.WriteLine("Got File!");
		    }
		    catch (Exception)
		    {
		        Console.WriteLine($"Could not receive file: ");
		    }
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
		    new file_client(args);
		}
	}
}