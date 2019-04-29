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
    public class EmailAndDBBackupJob : Quartz.IJob
    {
        public void Execute(Quartz.IJobExecutionContext context)
        {
            //This method creates a new email object, prepares an HTML table to be appended to the email, and then sends the email
            //to the admin on a weekly basis
            //
            using (var message = new System.Net.Mail.MailMessage("mmw5709@psu.edu", "mmw5709@psu.edu"))
            {
                var requests = new List<Tbl_Requests>();
                using (PennStateDB db = new PennStateDB())
                {
                    requests = db.Tbl_Requests.ToList();

                    //Build an HTML table onto a string
                    //
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

                //The admins PSU Harrisburg Email Login credentials must be entered appropriately here.
                //
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
            // PS C:\  Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
            //Then Run
            //Import-Module SQLPS

            StringBuilder script = new StringBuilder();

            //Assign the correct paths to where the database backup files will be backed up
            //Make sure the SQL Server backup retrieval path is also correct
            script.Append("function validpath($path){\n");
            script.Append("$retval = Test-Path $path\n");
            script.Append("return $retval}\n");
            script.Append("$basepath = 'E:'\n");
            script.Append("$comdate = Get-Date\n");
            script.Append("$BUDate = Get-Date -Format o | foreach {$_ -replace ':', '.'}\n");
            script.Append("$Scriptpath = $basepath+'\\PowerShell'\n");
            script.Append("$logfile = $Scriptpath + '\\PennStateDB_LogFile.txt'\n");
            script.Append("$SQLSrvInst = '(localdb)\\MSSQLLocalDB'\n");
            script.Append("$BUScriptPBB = $Scriptpath + '\\PennStateDB_Backup.sql'\n");
            script.Append("$SQKBAKpath = 'C:\\Program Files\\Microsoft SQL Server\\MSSQL14.MSSQLSERVER\\MSSQL\\Backup\\'\n");

            //Write to the log file in ~\PowerShell\ that the backup process has started
            //
            script.Append("$comment = '`nBu started at'\n");
            script.Append("$comment += $comdate\n");
            script.Append("Add-Content $logfile $comment\n");

            //Create the directory for the backups, if not already created
            //
            script.Append("$outpath = $basepath + '\\PennStateSQLbackups'\n");
            script.Append("$foundout = validpath($outpath)\n");
            script.Append("If(!$foundout){\n");
            script.Append("New-Item -ItemType directory -Path $outpath}\n");

            //Create the directory for the onedrive path, if not already created
            //
            script.Append("$Onedriveoutpath = $basepath + '\\PennStateSQLbackups\\Onedrive'\n");
            script.Append("$foundout = validpath($Onedriveoutpath)\n");
            script.Append("If (!$foundout){\n");
            script.Append("New-Item -ItemType directory -Path $Onedriveoutpath}\n");

            //Append a unique date and time to the new backup directory
            //
            script.Append("$outpathDBfiles = $outpath + '\\SQLBU_' + $BUDate\n");
            script.Append("New-Item -ItemType directory -Path $outpathDBfiles\n");

            //Run the backup SQL Script that is located in the ~\PowerShell directory
            //
            script.Append("Invoke-Sqlcmd -ServerInstance $SQLSrvInst -Database PennStateDB -inputfile $BUScriptPBB\n");

            //Save the master database file within the backup directory
            //
            script.Append("$DBName= 'master'\n");
            script.Append("$WrkSQLPath = $SQKBAKpath + $DBName + '.bak'\n");
            script.Append("$foundfile = validpath($WrkSQLPath)\n");
            script.Append("If ($foundfile){\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $outpathDBfiles -Force -Recurse\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $Onedriveoutpath -Force -Recurse}\n");

            //Save the model database file within the backup directory
            //
            script.Append("$DBName= 'model'\n");
            script.Append("$WrkSQLPath = $SQKBAKpath + $DBName + '.bak'\n");
            script.Append("$foundfile = validpath($WrkSQLPath)\n");
            script.Append("If($foundfile){\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $outpathDBfiles -Force -Recurse\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $Onedriveoutpath -Force -Recurse}\n");

            //Save the msdb database file within the backup directory
            //
            script.Append("$DBName = 'msdb'\n");
            script.Append("$WrkSQLPath = $SQKBAKpath + $DBName + '.bak'\n");
            script.Append("$foundfile = validpath($WrkSQLPath)\n");
            script.Append("If($foundfile){\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $outpathDBfiles -Force -Recurse\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $Onedriveoutpath -Force -Recurse}\n");

            //Finally, Save the primary database to the backup directory
            script.Append("$DBName = 'PennStateDB'\n");
            script.Append("$WrkSQLPath = $SQKBAKpath + $DBName + '.bak'\n");
            script.Append("$foundfile = validpath($WrkSQLPath)\n");
            script.Append("If($foundfile){\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $outpathDBfiles -Force -Recurse\n");
            script.Append("Copy-Item $WrkSQLPath -Destination $Onedriveoutpath -Force -Recurse}\n");

            //Write to the log file that the backup process has completed
            //
            script.Append("$comment = '`nBu ended at'\n");
            script.Append("$comdate = Get-Date\n");
            script.Append("Add-Content $logfile $comment\n");

            var scrString = script.ToString();
            //RunScript(scrString);
        }


        //Function to run the PowerShell script
        //
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

    //This is the automated job scheduler where you can change the day and time for report emails and database backups to be sent
    //
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<EmailAndDBBackupJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithSchedule(CronScheduleBuilder
                .WeeklyOnDayAndHourAndMinute(DayOfWeek.Monday, 15, 19)
                .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")))
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}