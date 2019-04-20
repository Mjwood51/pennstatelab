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
                .WeeklyOnDayAndHourAndMinute(DayOfWeek.Monday, 6, 00)
                .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")))
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}