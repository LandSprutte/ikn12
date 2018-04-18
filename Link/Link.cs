using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.IO.Ports;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
    /// <summary>
    /// Link.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// The DELIMITE for slip protocol.
        /// </summary>
        const byte DELIMITER = (byte) 'A';

        /// <summary>
        /// The buffer for link.
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// The serial port.
        /// </summary>
        SerialPort serialPort;

        /// <summary>
        /// client and server Ports
        /// </summary>
        private string servPort = "COM2";

        private string clientPort = "COM3";

        /// <summary>
        /// Initializes a new instance of the <see cref="link"/> class.
        /// </summary>
        public Link(int BUFSIZE, string APP)
        {
            // Create a new SerialPort object with default settings.
            if (APP.Equals("FILE_SERVER"))
            {
                serialPort = new SerialPort(servPort, 115200, Parity.None, 8, StopBits.One);
                Console.WriteLine("server connected");

            }
            else
            {
                serialPort = new SerialPort(clientPort, 115200, Parity.None, 8, StopBits.One);
                Console.WriteLine("Client Connected");
            }

            if (!serialPort.IsOpen)
                serialPort.Open();

            buffer = new byte[(BUFSIZE * 2)];

            // Uncomment the next line to use timeout
            //serialPort.ReadTimeout = 2000;

            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }

        /// <summary>
        /// Send the specified buf and size.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public void send(byte[] buf, int size)
        {
            var i = 0;
            // start framing
            var tempList = new List<byte>(1)
            {
                DELIMITER 
            };

            for (; i < size;)
            {
                switch (buf[i])
                {
                    case DELIMITER:
                        tempList.Add((byte)'B');
                        tempList.Add((byte)'C');
                        break;

                    case (byte)'B':
                        tempList.Add((byte)'B');
                        tempList.Add((byte)'D');
                        break;

                    default:
                        tempList.Add(buf[i]);
                        break;
                }
                ++i;
            }

            // End Frame
            tempList.Add(DELIMITER);

            // Write to serialport
            serialPort.Write(tempList.ToArray(), 0, tempList.Count);
        }

        /// <summary>
        /// Receive the specified buf and size.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public int receive(ref byte[] buf)
        {
            var i = 0;

            while (true)
            {
                if (serialPort.ReadByte() == DELIMITER)
                    break;
            }

            var rcvByte = (byte)serialPort.ReadByte();

            while (rcvByte != DELIMITER)
            {
                if (rcvByte == (byte)'B')
                {
                    var newByte = serialPort.ReadByte();

                    switch (newByte)
                    {
                        case (byte)'C':
                            buf[i++] = (byte)'A';
                            break;
                        case (byte)'D':
                            buf[i++] = (byte)'B';
                            break;
                        default:
                            return 0;
                    }
                }
                else
                    buf[i++] = rcvByte;

                rcvByte = (byte)serialPort.ReadByte();
            }
            return i;
        }
	}
}
