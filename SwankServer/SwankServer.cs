using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

using Yalla;
using Yalla.Evaluator;
using Yalla.Parser;
using Yalla.Tokenizer;
using Environment = Yalla.Evaluator.Environment;

namespace SwankServer
{
	public class SwankServer
	{
		private StreamReader reader;
		private StreamWriter writer;
		private int messageCounter;
		
		public SwankServer()
		{
			TcpListener listener = new TcpListener(new IPAddress(new byte[] { 127, 0, 0, 1 }), 4005);
			listener.Start(10);
			
			var socket = listener.AcceptSocket();
			
			var stream = new NetworkStream(socket);
			
			reader = new StreamReader(stream);
			
			writer = new StreamWriter(stream);
		}
		
		public void Run()
		{			
			var evaluator = new Evaluator(new Parser(new Tokenizer()), new Environment(), null, null);
            var prettyPrinter = new PrettyPrinter();
			
			// wireshark filter: tcp.port eq 4005 and data
						
			try
			{
				
				while(true)
				{
					var countChars = new char[6];
					
					int c = reader.ReadBlock(countChars, 0, 6);
					
					if (c == 6)
					{
						int count = int.Parse(new string(countChars), NumberStyles.HexNumber);
						
						var buffer = new char[count];
						
						reader.ReadBlock(buffer, 0, count);
						
						var sbuf = new String(buffer);
						
						messageCounter++;
						
						if (sbuf.Contains("swank:connection-info"))
						{					
							var message1 = CreateMessage("(:indentation-update ((\"tommy\" . 1)))");
							writer.Write(message1);
							
							var message = CreateMessage("(:return (:ok (:pid \"2229\" :style :spawn " + 
							                                          ":lisp-implementation (:type \"Yalla\" :name \"yalla\" :version \"0.1.0\") " + 
							                                          ":package (:name \"yalla.user\" :prompt \"yalla.user\") " + 
							                                          ":version \"20xx\")) 1)");
							writer.Write(message);
							writer.Flush();
						}
						else if (sbuf.Contains("swank:create-repl"))
						{
							var message = CreateMessage("(:return (:ok (\"yalla.user\" \"yalla.user\")) 2)");
							
							writer.Write(message);
							writer.Flush();
						}
						else if (sbuf.Contains("swank:operator-arglist"))
						{			
							SwankReturn();
						}
						else
						{
							// cannot handle multiline forms like (tommy \n aadss)
							var regex = new Regex("\\(swank:listener-eval \\\"(.*\n*?)\\\"\\) \"(.*)\" :repl-thread");
							
							var match = regex.Match(sbuf);							
							var msg = match.Groups[1].Value;
							
							try
							{                                
                                msg = Regex.Unescape(msg);
                                
							    var result = evaluator.Evaluate(msg);
								var returnString = prettyPrinter.EscapeString(prettyPrinter.PrettyPrint(result));
																
								SwankWriteResult(returnString);									
							}
							catch (Exception e)
							{
								SwankWrite(e.Message);
							}
								
													
							SwankReturn();
						}
					}
					else
					{
						throw new ArgumentOutOfRangeException("wtf?");
					}
				}
			}
			catch (Exception)
			{
				Console.Out.WriteLine("Disconnected");
			}	
		}
			
		private void SwankWrite(string msg)
		{
			var message1 = CreateMessage("(:write-string \"" + msg + "\")");
			
			writer.Write(message1);
			writer.Flush();
		}
				
		private void SwankWriteResult(string msg)
		{
			var message1 = CreateMessage("(:write-string \"" + msg + "\" :repl-result)");
			
			writer.Write(message1);
			writer.Flush();
		}
		
		private void SwankReturn(string val = "nil")
		{
			var message2 = CreateMessage("(:return (:ok " + val + ") " + messageCounter + ")");
			
            writer.Write(message2);
			writer.Flush();			
		}
		
		private static string CreateMessage(string message)
		{
			var stringHexLength = message.Length.ToString("X6");
			
			return stringHexLength + message;
		}
	}
}

