using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Nekki;
using UnityEngine;

public static class AdvLog
{
	public enum LogLevel
	{
		Log = 0,
		Warn = 1,
		Error = 2
	}

	private static string _filePath;

	public static void Init()
	{
		if (!Application.isEditor)
		{
			Debug.unityLogger.logEnabled = NekkiUtils.IsDebug;
		}
		if (NekkiUtils.IsDebug && _filePath.IsNullOrEmpty() && !NekkiUtils.IsWebPlayer)
		{
			Application.logMessageReceived += ApplicationLogSubsctiption;
			_filePath = Path.Combine(GlobalPath.LogsPath, string.Format("EditorLog_{0}.log", NekkiUtils.CurrentTimeAsString));
		}
	}

	public static void EmailLog(string mailAddres, string displayName)
	{
		if (NekkiUtils.IsDebug && !_filePath.IsNullOrEmpty() && !NekkiUtils.IsWebPlayer)
		{
			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress("logs@nekkimobile.ru", displayName);
			mailMessage.To.Add(mailAddres);
			mailMessage.Attachments.Add(new Attachment(_filePath));
			mailMessage.Subject = string.Format("Log from [{0}:{1}:{2}] {3}", SystemInfo.deviceModel, SystemInfo.deviceName, SystemInfo.deviceType, SystemInfo.deviceUniqueIdentifier);
			mailMessage.Body = "See log in attachment";
			SmtpClient smtpClient = new SmtpClient("mail.nekkimobile.ru");
			smtpClient.Port = 587;
			smtpClient.Credentials = new NetworkCredential("logs@nekkimobile.ru", "o99hSASo");
			smtpClient.EnableSsl = true;
			ServicePointManager.ServerCertificateValidationCallback = (object P_0, X509Certificate P_1, X509Chain P_2, SslPolicyErrors P_3) => true;
			smtpClient.Send(mailMessage);
			Debug.Log("Success EmailLog");
		}
	}

	private static void ApplicationLogSubsctiption(string message, string stacktrace, LogType type)
	{
		LogMessage(type.ToString(), message, stacktrace);
	}

	private static void LogMessage(string type, string message, string stacktrace)
	{
		if (NekkiUtils.IsDebug && !message.IsNullOrEmpty() && !NekkiUtils.IsWebPlayer)
		{
			string arg = DateTime.Now.ToString("hh:mm:ss");
			string text = string.Format("UNITY-{0} [Time:{1}] [Frame:{2}]", type.ToUpper(), arg, Time.frameCount) + "\n";
			text = text + message + "\n";
			text = text + stacktrace + "\n";
			FilesUtil.AppendFileText(_filePath, text);
		}
	}
}
