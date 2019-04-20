using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Quartz;
using System.Net.Mail;
using Quartz.Impl;
using System.Text;
using PennState.Models;
using System.Web.Mail;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PennState.App_Start
{
    public class EmailJob : Quartz.IJob
    {
        public void Execute(Quartz.IJobExecutionContext context)
        {
            using (var message = new System.Net.Mail.MailMessage("mmw5709@psu.edu", "mmw5709@psu.edu"))
            {
                var requests = new List<Tbl_Requests>();
                using (PennStateDB db = new PennStateDB())
                {
                    requests = db.Tbl_Requests.ToList();


                    StringBuilder builder = new StringBuilder();
                    builder.Append("<table>");
                    builder.Append("<thead><tr>");
                    builder.Append("<th style='font-size:14px;padding-left:30px'>Item Name</th>");
                    builder.Append("<th style='font-size:14px;padding-left:30px'>Quantity</th>");
                    builder.Append("<th style='font-size:14px;padding-left:30px'>Unit Price</th>");
                    builder.Append("<th style='font-size:14px;padding-left:30px'>Message</th>");
                    builder.Append("<th style='font-size:14px;padding-left:30px'>User</th>");
                    builder.Append("<th style='font-size:14px;padding-left:30px'>Total Price</th></tr></thead>");
                    builder.Append("</tr></thead>");
                    builder.Append("<tbody>");
                    if (requests != null)
                    {
                        foreach (var item in requests)
                        {

                            builder.Append("<tr style='height:50px'>");
                            builder.Append("<td style='font-size:14px;padding-left:30px'>" + item.ItemName + "</td>");
                            builder.Append("<td style='font-size:14px;padding-left:30px'>" + item.Quantity + "</td>");
                            builder.Append("<td style='font-size:14px;padding-left:30px'>" + item.UnitPrice + "</td>");
                            builder.Append("<td style='font-size:14px;padding-left:30px'>" + item.Message + "</td>");
                            builder.Append("<td style='font-size:14px;padding-left:30px'>" + item.Tbl_Users.FirstName + " " + item.Tbl_Users.LastName + "</td>");
                            builder.Append("<td style='font-size:14px;padding-left:30px'>" + item.TotalPrice + "</td>");
                            builder.Append("</tr>");
                        }
                    }
                    builder.Append("</tbody></table>");
                    message.Subject = "Weekly Penn State Lab Inventory Requests Report: " + DateTime.Now;
                    message.IsBodyHtml = true;
                    message.Body = builder.ToString();
                }
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = "authsmtp.psu.edu",
                    Port = 587,
                    Credentials = new NetworkCredential("mmw5709@psu.edu.com", "Dodgerfan42")
                })
                {
                    client.Send(message);
                }
            }
            //Make sure to run this command in powershell when you start up the server
            // PS C:\WINDOWS\system32>  Set-ExecutionPolicy RemoteSigned
            //Then Run
            //Import-Module SQLPS
            StringBuilder script = new StringBuilder();
            script.Append("function validpath($path){\n");
            script.Append("$retval = Test-Path $path\n");
            script.Append("return $retval}\n");
            script.Append("$basepath = 'C:\\Users\\Mark W'\n");
            script.Append("$comdate = Get-Date\n");
            script.Append("$BUDate = Get-Date -Format o | foreach {$_ -replace ':', '.'}\n");
            script.Append("$Scriptpath = $basepath+'\\PowerShell'\n");
            script.Append("$logfile = $Scriptpath + '\\PennStateDB_LogFile.txt'\n");
            script.Append("$SQLSrvInst = '(localdb)\\MSSQLLocalDB'\n");
            script.Append("$BUScriptPBB = $Scriptpath + '\\PennStateDB_Backup.sql'\n");
            script.Append("$SQKBAKpath = 'C:\\Program Files\\Microsoft SQL Server\\MSSQL14.MSSQLSERVER\\MSSQL\\Backup\\'\n");
            script.Append("$comment = '`nBu started at'\n");
            script.Append("$comment += $comdate\n");
            script.Append("Add-Content $logfile $comment\n");
            script.Append("$outpath = $basepath + '\\PennStateSQLbackups'\n");
            script.Append("$foundout = validpath($outpath)\n");
            script.Append("If(!$foundout){\n");
            script.Append("New-Item -ItemType directory -Path $outpath}\n");
            script.Append("$Onedriveoutpath = $basepath + '\\PennStateSQLbackups\\Onedrive'\n");
            script.Append("$foundout = validpath($Onedriveoutpath)\n");
            script.Append("If (!$foundout){\n");
            script.Append("New-Item -ItemType directory -Path $Onedriveoutpath}\n");
            script.Append("$outpathDBfiles = $outpath + '\\SQLBU_' + $BUDate\n");
            script.Append("New-Item -ItemType directory -Path $outpathDBfiles\n");
            script.Append("Invoke-Sqlcmd -ServerInstance $SQLSrvInst -Database PennStateDB -inputfile $BUScriptPBB\n");
            script.Append("$DBName= 'master'\n");
            script.Append("$WrkSQLPath = $SQKBAKpath + $DBName + '.bak'\n");
            script.Append("$foundfile = validpath($WrkSQLPath)\n");
            script.Append("If ($foundfile){\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $outpathDBfiles -Force -Recurse\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $Onedriveoutpath -Force -Recurse}\n");
            script.Append("$DBName= 'model'\n");
            script.Append("$WrkSQLPath = $SQKBAKpath + $DBName + '.bak'\n");
            script.Append("$foundfile = validpath($WrkSQLPath)\n");
            script.Append("If($foundfile){\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $outpathDBfiles -Force -Recurse\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $Onedriveoutpath -Force -Recurse}\n");
            script.Append("$DBName = 'msdb'\n");
            script.Append("$WrkSQLPath = $SQKBAKpath + $DBName + '.bak'\n");
            script.Append("$foundfile = validpath($WrkSQLPath)\n");
            script.Append("If($foundfile){\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $outpathDBfiles -Force -Recurse\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $Onedriveoutpath -Force -Recurse}\n");
            script.Append("$DBName = 'PennStateDB'\n");
            script.Append("$WrkSQLPath = $SQKBAKpath + $DBName + '.bak'\n");
            script.Append("$foundfile = validpath($WrkSQLPath)\n");
            script.Append("If($foundfile){\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $outpathDBfiles -Force -Recurse\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $Onedriveoutpath -Force -Recurse}\n");
            script.Append("$comment = '`nBu ended at'\n");
            script.Append("$comdate = Get-Date\n");
            script.Append("Add-Content $logfile $comment\n");





            var scrString = script.ToString();
            RunScript(scrString);
        }

        private void RunScript(string scriptText)
        {
            // create Powershell runspace

            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it

            runspace.Open();

            // create a pipeline and feed it the script text

            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(scriptText);

            // add an extra command to transform the script
            // output objects into nicely formatted strings

            // remove this line to get the actual objects
            // that the script returns. For example, the script

            // "Get-Process" returns a collection
            // of System.Diagnostics.Process instances.

            // execute the script

            var results = pipeline.Invoke();

            // close the runspace

            runspace.Close();

            // convert the script result into a single string

            
        }
    }

    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithSchedule(CronScheduleBuilder
                .WeeklyOnDayAndHourAndMinute(DayOfWeek.Saturday, 17, 41)
                .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")))
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}