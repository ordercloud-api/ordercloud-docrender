using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OrderCloud.DocRender
{
	public class MpowerClient : TcpClient
	{
		private readonly string hostName;
		private readonly int hostPort;
		
		public MpowerClient(string mpowerHostName, int mpowerHostPort)
		{
			hostName = mpowerHostName;
			hostPort = mpowerHostPort;
		}

		// SubmitToMpower takes the command packet and an out string for the response
		public async Task<JobRenderResponse>SubmitRenderJobAsync(Dictionary<string, string> DocVars)
		{
			var jobResponse = new JobRenderResponse {DateQueued = DateTime.Now, Success = false, ServerJobResponse = ""};
			try
			{
				this.Connect(hostName, hostPort);
			}
			catch (SocketException ex)
			{
				jobResponse.ErrorMessage = "Mpower doesn't appear to be running on: " + hostName + " port: " + hostPort + ". Error: " + ex.Message; ;
				return jobResponse;
			}
			Socket sock = this.Client;
			if (this.Active)
			{
				sock = this.Client;
			}
			else
			{
				jobResponse.ErrorMessage = "The Mpower socket has unexpectedly closed";
				return jobResponse;
			}

			try
			{
				Byte[] dataSend = BuildCommandPacket(DocVars);
				NetworkStream stream = this.GetStream();

				await stream.WriteAsync(dataSend, 0, dataSend.Length);
				sock.Shutdown(SocketShutdown.Send); // we're done sending
													//read response
				Byte[] dataRecv = new Byte[4096];
				while (null != sock && sock.Poll(-1, SelectMode.SelectRead))
				{
					int bytes = await stream.ReadAsync(dataRecv, 0, dataRecv.Length);
					if (bytes > 0)
						jobResponse.ServerJobResponse += System.Text.Encoding.ASCII.GetString(dataRecv, 0, bytes).Trim('\0');
					else
						break;  // Mpower disconnected the socket
				}
				sock.Shutdown(SocketShutdown.Both);
				sock.Close();
			}
			catch (Exception ex)
			{
				jobResponse.ErrorMessage = "Communication error with Mpower: " + ex.Message;
				return jobResponse;
			}
			if (string.IsNullOrEmpty(jobResponse.ServerJobResponse))
			{
				jobResponse.ServerJobResponse = "No response from mpower server";
				return jobResponse;
			}
			this.Close();
			jobResponse.Success = true;
			return jobResponse;
		}
		
		private byte[] BuildCommandPacket(Dictionary<string, string> DocVars)
		{
			var commandName = "SumitJob";
			XmlDocument xml = new XmlDocument();

			xml.LoadXml(@"<?pf_command version=""1.0""?><pfjob:job_command xmlns:pfjob=""http://www.pageflex.com/schemas"" name=""" + commandName + @"""><pfjob:job_variables /><pfjob:doc_variables /></pfjob:job_command>");

			XmlNamespaceManager nsm = new XmlNamespaceManager(xml.NameTable);
			nsm.AddNamespace("pfjob", "http://www.pageflex.com/schemas");
			XmlNode doc = xml.SelectSingleNode("pfjob:job_command/pfjob:doc_variables", nsm);
			XmlNode job = xml.SelectSingleNode("pfjob:job_command/pfjob:job_variables", nsm);
			
			void AddVar(string name, string value, XmlNode node)
			{
				XmlNode spec = xml.CreateElement("pfjob", "var", "http://www.pageflex.com/schemas");
				spec.InnerText = value;
				XmlAttribute nameAttribute = xml.CreateAttribute("name");
				nameAttribute.InnerText = name;
				spec.Attributes.Append(nameAttribute);
				node.AppendChild(spec);
			}

			AddVar("JobName", "preview|proof|prod", job);
			AddVar("_sys_QueueName", "jobqueuname", job);
			AddVar("_sys_ClientJobMonitor", "True", job);
			AddVar("ProjectName", "projectid and path", job);
			AddVar("_sys_FinalOutputDir", @"\\finalserveroutput\mpoweroutput", job);
			//AddVar("_sys_JobDataSourceFile", "a datasource for this job not the project", job);

			foreach (var name in DocVars.Keys)
			{
				AddVar(name, DocVars[name], doc);
			}

			return System.Text.Encoding.Unicode.GetBytes(xml.OuterXml);
		}
	}
}
