using System;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace VAA.CommonComponents
{
    /// <summary>
    /// Email Helper and various email template
    /// </summary>
    public class EmailHelper
    {


        public static string NotificationMailTemplate =

       @"<p>
    	Hi [FIRSTNAME],</p>
        <p>
	        This is to notify you about below Menu change:</p>

        <p><b>Menu Code:</b>[MENUCODE]</p>
        <p><b>Menu Name :</b>[MENUNAME]</p>
        <p><b>From :</b>[FROMUSER]</p>
        <p><b>Mesasge :</b>[MESSAGE]</p>

        
        <p>&nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";


        public static string AppointmentNotificationTemplate =

       @"<p>
    	Hi [FIRSTNAME],</p>
        <p>
	        This is to notify you about below Emma task/appointment scheduled for you:</p>

        <p><b>Task :</b>[SUBJECT]</p>
        <p><b>Description :</b>[DESCRIPTION]</p>
        <p><b>Start Time :</b>[STARTTIME]</p>
        <p><b>End Time :</b>[ENDTIME]</p>

        
        <p>&nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";


        public static string UpdateAppointmentNotificationTemplate =

       @"<p>
    	Hi [FIRSTNAME],</p>
        <p>
	        This is to notify you about update in below Emma task/appointment scheduled for you:</p>

        <p><b>Task :</b>[SUBJECT]</p>
        <p><b>Description :</b>[DESCRIPTION]</p>
        <p><b>Start Time :</b>[STARTTIME]</p>
        <p><b>End Time :</b>[ENDTIME]</p>

        
        <p>&nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";

        public static string MissingBaseItemEmailTemplate =

       @"<p>
    	Hi,</p>
        <p>
	        The service plan contains below base items which are not present in database:</p>
        <p>
	        [BASEITEMS]</p>
        <p>
      
        <p>
	        Please login to portal and add these base items. After this please upload service again! </p>
        <p>
	        &nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";


        public static string UploadCompleteEmailTemplate =

       @"<p>
    	Hi,</p>
        <p>
	        This is to notify that below service plan upload is completed successfully:</p>
        <p>
	        [SERVICEPLANNAME]</p>
        <p>
      
        <p>
	        The menu is ready to review. Please login to portal and review the menu created!</p>
        <p>
	        &nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";


        public static string PackingTicketPdfGenerationCompleteEmailTemplate =

       @"<p>
    	Hi,</p>
        <p>
	        This is to notify that Packing Ticket PDF generation is completed successfully for below Order ID:</p>
        <p>
	        [ORDERID]</p>
        <p>
      
        <p>
	        Please login to server PDF folder or use FTP to access these Packing Tickets PDF.</p>
        <p>
	        &nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";

        public static string PackingTicketExcelGenerationCompleteEmailTemplate =

       @"<p>
    	Hi,</p>
        <p>
	        This is to notify that Packing Ticket Excel data generation is completed successfully for below Order ID:</p>
        <p>
	        [ORDERID]</p>
        <p>
      
        <p>
	        Please click the Download Box Ticket (Excel) to access the Packing Ticket Excel.</p>
        <p>
	        &nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";


        public static string OrderPdfGenerationCompleteEmailTemplate =

     @"<p>
    	Hi,</p>
        <p>
	        This is to notify that PDF generation is completed successfully for below Order ID:</p>
        <p>
	        [ORDERID]</p>
        <p>
      
        <p>
	        Please login to server PDF folder or use FTP to access these PDF.</p>
        <p>
	        &nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";


        public static string FlightScheduleUploadTemplate =
             @"<p>
    	Hi,</p>
        <p>
	        This is to notify you that Flight Schedule upload has been completed successfully.</p>
        
        <p>
	        &nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";


         public static string ReorderOption1EmailTemplate =

    @"<p>
    	Hi,</p>
        <p>
	        This is to notify you that Reorder-straight Reprint is completed successfully.</p>
        
        <p>
	        The Order has been moved to Current Orders. Please check the Current Orders List.</p> 

        <p>
	        &nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";

         public static string ReorderOption2EmailTemplate =

    @"<p>
    	Hi,</p>
        <p>
	        This is to notify you that Reorder with schedule update is completed successfully.</p>
        
        <p> The Order has been moved to Live Orders. Please check the LiveOrders page.</p> 

        <p>
	        &nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";




          public static string ReorderOption3EmailTemplate =

    @"<p>
    	Hi,</p>
        <p>
	        This is to notify you that Reorder with menu update is completed successfully.</p>
        
        <p>
	        Number of menus moved to LiveOrder: [MOVEDTOLIVEORDER] </p>

  <p>
	        Number of menus recreated for ReOrder: [RECREAREDFORREORDER] </p>

        <p>
	        &nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";



        public static string PdfGenerationCompleteForCritetiaEmailTemplate =

    @"<p>
    	Hi,</p>
        <p>
	        This is to notify you that PDF generation is completed successfully.</p>
        
        <p>
	        Please login to server PDF folder or use FTP to access these PDF.</p>
        <p>
	        &nbsp;</p>
        <p>
	        Kind Regards,</p>
        <p>
	        Emma Team</p>
        <p>
	        <span style='color:#696969;'><em><span style='font-size: 12px;'>Please note this is an automated response email, please do not reply to this email address.</span></em></span>";



        public static void SendMenuChangeNotification(string email, string firstname, string menucode, string menuname, string fromuser, string message)
        {
            //send email
            var defaultMessage = EmailHelper.NotificationMailTemplate;
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, firstname, "\\[FIRSTNAME\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, menucode, "\\[MENUCODE\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, menuname, "\\[MENUNAME\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, fromuser, "\\[FROMUSER\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, message, "\\[MESSAGE\\]");

            EmailHelper.SendMail(email, "ESPAdmin@espcolour.co.uk", "EMMA- Menu Change Notification", defaultMessage);
        }

        public static void SendAppointmentNotification(string email, string firstname, string subject, string description, string startdatetime, string enddatetime)
        {

            //send email
            var defaultMessage = EmailHelper.AppointmentNotificationTemplate;
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, firstname, "\\[FIRSTNAME\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, subject, "\\[SUBJECT\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, description, "\\[DESCRIPTION\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, startdatetime, "\\[STARTTIME\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, enddatetime, "\\[ENDTIME\\]");

            EmailHelper.SendMail(email, "ESPAdmin@espcolour.co.uk", "EMMA- Task/Appointment Notification", defaultMessage);

        }

        public static void SendUpdateAppointmentNotification(string email, string firstname, string subject, string description, string startdatetime, string enddatetime)
        {

            //send email
            var defaultMessage = EmailHelper.UpdateAppointmentNotificationTemplate;
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, firstname, "\\[FIRSTNAME\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, subject, "\\[SUBJECT\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, description, "\\[DESCRIPTION\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, startdatetime, "\\[STARTTIME\\]");
            defaultMessage = EmailHelper.ConvertMail2(defaultMessage, enddatetime, "\\[ENDTIME\\]");

            EmailHelper.SendMail(email, "ESPAdmin@espcolour.co.uk", "EMMA- Task/Appointment Update Notification", defaultMessage);

        }

        public static string ForgotPasswordNotification(string name, string email)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<table style='font-family:calibri; font-size:14px;'><tr><td>");
            sb.Append("Hi " + name + ",");
            sb.Append("</td></tr>");
            sb.Append("<tr><td></td></tr>");
            sb.Append("<tr><td>");
            sb.Append("You have requested to reset your password, Please find below temporary password for your EMMA account.");
            sb.Append("</td></tr>");
            sb.Append("<tr><td></td></tr>");
            sb.Append("<tr><td>");

            string newPassword = GeneratePassword();
            //Generate password
            sb.Append("Password : " + newPassword);
            sb.Append("</td></tr>");
            sb.Append("<tr><td></td></tr>");

            sb.Append("<tr><td>");

            sb.Append("Please do remember to change your password from 'My Account' after you login first time using this temporary password.");

            sb.Append("</td></tr>");


            sb.Append("<tr><td>");
            sb.Append("<tr><td><br /<br />Kind Regards,</td></tr>");
            sb.Append("<tr><td>");
            sb.Append("EMMA Team");
            sb.Append("</td></tr>");



            sb.Append("<tr><td style='font-family:calibri; font-size:11px;'>____________________________________________________________________________________________________</td></tr>");
            sb.Append("<tr><td style='font-family:calibri; font-size:11px;'>**Please do not reply to this automated message, reply to this message is not Monitored .</td></tr>");
            sb.Append("</table>");
            sb.Append("</body>");
            sb.Append("</html>");
            sb.AppendLine();
            string bodymessgaetosend = sb.ToString();
            SendMail(email, "info@espweb2print.co.uk", "Emma Login Credentials", bodymessgaetosend);

            return newPassword;
        }

        public static string GeneratePassword()
        {
            int length = 8;
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string res = "";
            Random rnd = new Random();
            while (0 < length--)
                res += valid[rnd.Next(valid.Length)];
            return res;
        }

        public static void SendMail(string eto, string efrom, string subject, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(eto) || string.IsNullOrEmpty(efrom))
                    return;
                var priority = MailPriority.Normal;

                var mailer = new MailMessage("info@espweb2print.co.uk", eto, subject, message);
                var smtp = new SmtpClient("espremote.espcolour.co.uk");
                mailer.IsBodyHtml = true;
                mailer.Priority = priority;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = null;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mailer);

            }
            catch (Exception ex)
            {
                //LOG IT TO A LOG FILE
            }

        }

        public static string ConvertMail2(string defaultMessage, string missingCode, string toReplace)
        {
            defaultMessage = Regex.Replace(defaultMessage, toReplace, missingCode);

            return defaultMessage;
        }
    }
}
